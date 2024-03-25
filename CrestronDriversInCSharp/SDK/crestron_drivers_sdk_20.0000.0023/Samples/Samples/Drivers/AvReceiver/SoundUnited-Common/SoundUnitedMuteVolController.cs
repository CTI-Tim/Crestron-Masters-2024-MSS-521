using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Crestron.RAD.Drivers.AVReceivers.SoundUnited
{
    internal class SoundUnitedMuteVolController 
    {
        /// <summary>
        /// This prevents the driver from processing volume feedback while it is busying changing levels.
        /// This value will be used in the polling sequence to determine if a custom VolumePoll can be 
        /// sent to sync up with the device once volume isn't being controlled.
        /// 
        /// This is ignoring feedback because the device's feedback will lag behind what the value is ramping to 
        /// if volume operations are performed fast enough.
        /// </summary>
        private bool _controllingVolume;

        /// <summary>
        /// Set to CrestronEnvironment.TickCount when any volume level is set by the application.
        /// </summary>
        private uint _lastSetVolumeTick;

        /// <summary>
        /// The maximum number of ticks (ms) to wait before allowing volume feedback to be processed.
        /// </summary>
        private const uint _timeToIgnoreVolumeFeedback = 750;

        /// <summary>
        /// Wait between a mute command and a volume command. If volume is sent
        /// right after unmute, the device does not change volume. Testing
        /// shows at least 1s is required, so we use 2s to provide margin
        /// </summary>
        private const uint _timeBetweenMuteAndVol = 2000;

        /// <summary>
        /// The tick the last mute on/off command was sent
        /// </summary>
        private uint _lastMuteCommandSentTick;

        /// <summary>
        /// Delegate function for this controller to poll the present mute
        /// state. This is used to determine if a volume command was just
        /// sent while the device was muted and we need to treat it like
        /// an unmute command since the device does unmute itself.
        /// </summary>
        private Func<bool> IsMuted;

        public SoundUnitedMuteVolController(Func<bool> getMuteState)
        {
            IsMuted = getMuteState;
        }

        /// <summary>
        /// Must be called when a mute command is sent. It is intended to be
        /// called from the callback of a CommandSet.
        /// </summary>
        public void MuteCommandChanged()
        {
            // Remember when we last sent a Mute On/Mute Off command
            // The driver cannot send a volume command too quickly after
            _lastMuteCommandSentTick = (uint)CrestronEnvironment.TickCount;
        }

        /// <summary>
        /// Must be called when a volume command is sent. It is intended to be
        /// called from the callback of a CommandSet.
        /// </summary>
        public void VolCommandSent()
        {
            // If mute is on, this acts like an "Unmute" command.
            // The device does accept the first volume command in this state,
            // but it can miss another volume command sent right after it
            if (IsMuted())
            {
                MuteCommandChanged();
            }

            // Also track when last volume command was sent to determine when
            // to start accepting volume feedback from the device
            StartControllingVolume();
        }

        /// <summary>
        /// Must be called when a volume command is being passed to
        /// SendCommand(). This lets the controller know that fakee feedback
        /// for volume was sent out and we need to eventually poll for the
        /// actual device state. This function is necessary because we may
        /// send fake feedback for a command that the framework decides not to
        /// queue or send because we are warming up or because power is not on.
        /// </summary>
        public void StartControllingVolume()
        {
            _controllingVolume = true;
            _lastSetVolumeTick = (uint)CrestronEnvironment.TickCount;
        }

        /// <summary>
        /// Reports whether or not the device can accept a volume command.
        /// If this property is false, volume commands for this zone should not
        /// be sent to the device or they may be ignored.
        /// </summary>
        public bool CanSendVol
        {
            get
            {
                return (uint)CrestronEnvironment.TickCount - _lastMuteCommandSentTick >= _timeBetweenMuteAndVol;
            }
        }

        /// <summary>
        /// Reports whether or not the device can accept a mute/unmute command. If
        /// If this property is false, mute/unmute commands for this zone should not
        /// be sent to the device. Sending them anyway can result in triggering AVR
        /// firmware bugs that cause the device to stop sending feedback until it
        /// powers off.
        /// </summary>
        public bool CanSendMute
        {
            get
            {
                // There's a bug in the AVR firmware that causes it to stop
                // sending responses if you send a mute too quickly after
                // unmuting due to a change in volume. So allow mute only if
                // the timeout has expired or if we are sure the unmute was
                // from a mute command or the previous volume command was
                // processed
                return !_controllingVolume || CanSendVol;
            }
        }

        /// <summary>
        /// Internal flag that reports whether or not the timeout has expired
        /// for volume control and we're ready to start accepting device
        /// feedback again
        /// </summary>
        private bool ShouldAcceptVolumeFeedback
        {
            get
            {
                // The !_controllingVolume is only here to avoid a window
                // blanking out feedback where the tick count wraps around
                return !_controllingVolume || (uint)CrestronEnvironment.TickCount - _lastSetVolumeTick >= _timeToIgnoreVolumeFeedback;
            }
        }

        /// <summary>
        /// Check whether volume feedback should be accepted.
        /// NOTE: This has the side effect of assuming that if it returns true,
        /// the feedback is actually processed and that we don't need to do
        /// more volume polling. Modify the use of this function with caution.
        /// </summary>
        /// <returns>Whether or not the feedback should be processed.</returns>
        public bool VolumeFeedbackOk()
        {
            bool ok = ShouldAcceptVolumeFeedback;
            if (ok)
            {
                _controllingVolume = false;
            }
            return ok;
        }

        /// <summary>
        /// Whether or not we should poll for volume for this zone
        /// </summary>
        public bool NeedToPollVolume
        {
            get
            {
                // Need to poll volume if we're still controlling volume and
                // the feedback blackout period has expired.
                // _controllingVolume will get set back to false once we get
                // some feedback
                return _controllingVolume && ShouldAcceptVolumeFeedback;
            }
        }
    }
}