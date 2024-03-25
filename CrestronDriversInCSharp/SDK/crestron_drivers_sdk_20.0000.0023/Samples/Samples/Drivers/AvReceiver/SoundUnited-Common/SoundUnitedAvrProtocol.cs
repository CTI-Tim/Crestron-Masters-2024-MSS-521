// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Crestron Electronics" file="SoundUnitedAvrProtocol.cs">
//   
// </copyright>
// 
// Note: This driver makes use of BuildCommand() instead of new CommandSet() in
// all places that can be called by single-zone framework code. This is because
// we want to set AllowIsQueueableOverride and AllowIsSendableOverride = true
// to let us override them, but in older frameworks these do not exist. Using
// BuildCommand() does require that the json file include those Allow*
// parameters for commands.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.RAD.Common;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Helpers;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.RADAVReceiver;
using DriverExtensionLibrary.Helpers;
using System.Text.RegularExpressions;
using Crestron.RAD.Drivers.AVReceivers.SoundUnited;
using Crestron.RAD.Ext.Util.Scaling;

namespace Crestron.RAD.Drivers.AVReceivers
{
    public class SoundUnitedAvrProtocol : AAVReceiverProtocol
    {
        private enum FrequencyTunningMethod
        {
            Unknown = 0,
            Automatic = 1,
            Manual = 2
        }

        private List<CommandSet> _pendingToneControlCommands;
        private List<CommandSet> _pendingFrequencyCommands;

        private AAvrZone _zone1;
        private AAvrZone _zone2;

        private int _toneMinValue = -6;
        private int _toneMaxValue = 6;

        /// <summary>
        /// Devices requires a setting to be changed depending on how the user wants to tune their radio.
        /// </summary>
        private FrequencyTunningMethod _frequencyTuningMode = FrequencyTunningMethod.Unknown;

        private AudioConnections _currentNetworkInput = AudioConnections.None;

        internal const string TuningModeCommandGroup = "tuningMode";
        internal const string TuningModeAuto = "TMANAUTO";
        internal const string TuningModeManual = "TMANMANUAL";
        internal const string ApiDelimiter = "\r";
        internal const string Zone1Header = "Z2";
        internal const string Zone2Header = "Z3";
        private const string MaxVolumePrefix = "MAX ";
        private const double MainZoneVolumeStep = 0.5;
        private const double ZoneVolumeStep = 1.0;

        private StringBuilder _responseBuffer = new StringBuilder();

        // Volume logic for each zone
        private SoundUnitedVolumeController _mainVolumeController;
        private SoundUnitedVolumeController _zone1VolumeController;
        private SoundUnitedVolumeController _zone2VolumeController;

        /// <summary>
        /// Wait between sending power commands.
        /// Testing shows 2100 works reliably while 2000 does not. 3000 is
        /// used to provide margin.
        /// </summary>
        private const uint _timeBetweenPowerCommands = 3000;

        /// <summary>
        /// The tick the last power command was sent that exited standby mode
        /// </summary>
        private uint _lastStandbyExitTick;

        private const string _delimiterPattern = "(\r)";

        private bool _disposed;

        /// <summary>
        /// Tracks when method Initialize completes.
        /// This is used to keep method Poll() from throwing an exception on a volume controller not being instantiated.
        /// </summary>
        private bool _initialized;

        private uint TimeBetweenCommandsOverride
        {
            get
            {
                uint? minOverride = AvrData.CrestronSerialDeviceApi.Api.Communication.TimeBetweenCommandsOverride;
                if (minOverride != null)
                {
                    return (uint)minOverride;
                }
                return AvrData.CrestronSerialDeviceApi.Api.Communication.TimeBetweenCommands;
            }
        }

        public SoundUnitedAvrProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
            _pendingToneControlCommands = new List<CommandSet>();
            _pendingFrequencyCommands = new List<CommandSet>();

            ResponseValidation = new SoundUnitedAvrResponseValidation(id, ValidatedData);
            ValidatedData.PowerOnPollingSequence = new[]
            {
                StandardCommandsEnum.PowerPoll,
                StandardCommandsEnum.InputPoll,
                StandardCommandsEnum.VolumePoll,
                StandardCommandsEnum.MutePoll,
                StandardCommandsEnum.TunerFrequencyPoll,
                StandardCommandsEnum.ToneBassPoll,
                StandardCommandsEnum.ToneTreblePoll,
                StandardCommandsEnum.ToneStatePoll,
                StandardCommandsEnum.SurroundModePoll
            };
        }

        private AAvrZone GetZone(CommonCommandGroupType zone)
        {
            AAvrZone avrZone = null;

            switch (zone)
            {
                case CommonCommandGroupType.AvrZone1:
                    avrZone = _zone1;
                    break;
                case CommonCommandGroupType.AvrZone2:
                    avrZone = _zone2;
                    break;
                default:
                    break;
            }

            return avrZone;
        }


        public override void Initialize(object driverData)
        {
            base.Initialize(driverData);

            _zone1 = AvrData.CrestronSerialDeviceApi.Api.AvrZoneData.AvrZone[0];
            _zone2 = AvrData.CrestronSerialDeviceApi.Api.AvrZoneData.AvrZone[1];

            _zone1.PowerIsOnChanged += ZonePowerChanged;
            _zone2.PowerIsOnChanged += ZonePowerChanged;

            _mainVolumeController = new SoundUnitedVolumeController(MainZoneIsMuted, ChangeMainZoneVolume, MainZoneVolumeStep, TimeBetweenCommandsOverride);
            _zone1VolumeController = new SoundUnitedVolumeController(Zone1IsMuted, ChangeZone1Volume, ZoneVolumeStep, TimeBetweenCommandsOverride);
            _zone2VolumeController = new SoundUnitedVolumeController(Zone2IsMuted, ChangeZone2Volume, ZoneVolumeStep, TimeBetweenCommandsOverride);
            _mainVolumeController.VolumeLevel.PercentChanged += MainZoneVolPercentEventHandler;
            _zone1VolumeController.VolumeLevel.PercentChanged += Zone1VolPercentEventHandler;
            _zone2VolumeController.VolumeLevel.PercentChanged += Zone2VolPercentEventHandler;

            _initialized = true;
        }


        public override void Dispose()
        {
            if (!_disposed)
            {
                if (_zone1 != null)
                {
                    _zone1.PowerIsOnChanged -= ZonePowerChanged;
                }
                if (_zone2 != null)
                {
                    _zone2.PowerIsOnChanged -= ZonePowerChanged;
                }
                if (_mainVolumeController != null)
                {
                    _mainVolumeController.VolumeLevel.PercentChanged -= MainZoneVolPercentEventHandler;
                    _mainVolumeController.Dispose();
                }
                if (_zone1VolumeController != null)
                {
                    _zone1VolumeController.VolumeLevel.PercentChanged -= Zone1VolPercentEventHandler;
                    _zone1VolumeController.Dispose();
                }
                if (_zone2VolumeController != null)
                {
                    _zone2VolumeController.VolumeLevel.PercentChanged -= Zone2VolPercentEventHandler;
                    _zone2VolumeController.Dispose();
                }

                _disposed = true;
            }

            base.Dispose();
        }

        #region Event Handlers and Delegates for Helpers

        /// <summary>
        /// Mute getter for mute/volume controller delegate for main zone
        /// </summary>
        /// <returns>Whether or not the main zone is muted</returns>
        private bool MainZoneIsMuted()
        {
            return MuteIsOn;
        }
        
        /// <summary>
        /// Mute getter for mute/volume controller delegate for zone 1
        /// </summary>
        /// <returns>Whether or not the zone 1 is muted</returns>
        private bool Zone1IsMuted()
        {
            return _zone1.IsMuted;
        }

        /// <summary>
        /// Mute getter for mute/volume controller delegate for zone 2
        /// </summary>
        /// <returns>Whether or not the zone 2 is muted</returns>
        private bool Zone2IsMuted()
        {
            return _zone2.IsMuted;
        }

        private void MainZoneVolPercentEventHandler(object sender, LevelChangedEventArgs<uint> volume)
        {
            var stateObj = new Volume { MuteIsOn = MuteIsOn, VolumeIs = volume.Level };
            FireEvent(AvrStateObjects.Volume, stateObj);
        }

        private void Zone1VolPercentEventHandler(object sender, LevelChangedEventArgs<uint> volume)
        {
            _zone1.VolumePercent = volume.Level;
        }

        private void Zone2VolPercentEventHandler(object sender, LevelChangedEventArgs<uint> volume)
        {
            _zone2.VolumePercent = volume.Level;
        }

        private void ChangeMainZoneVolume(double volume)
        {
            // Input volume is already in correct scale, but we need to convert
            // 70.5 into 705, etc. So we need to multiply by 10 but round to
            // half-steps. So we multiply by 2, then round, then multiply by 5 
            uint newVolume = (uint)Math.Round(volume * 2) * 5;

            // Notify volume controller that we're sending a volume command
            // which means it needs to ensure we poll later even if this
            // command is not queued. Otherwise, fake feedback will leave the
            // application with the wrong volume value.
            _mainVolumeController.MuteVol.StartControllingVolume();

            // Send the volume command
            SendCommand(BuildCommand(
                StandardCommandsEnum.Vol,
                CommonCommandGroupType.Volume,
                CommandPriority.Normal,
                string.Format("SetVolume {0}", newVolume),
                string.Format("MV{0:000}", newVolume)));
        }

        private void ChangeZone1Volume(double volume)
        {
            ZoneChangeVolume(CommonCommandGroupType.AvrZone1, volume);
        }

        private void ChangeZone2Volume(double volume)
        {
            ZoneChangeVolume(CommonCommandGroupType.AvrZone2, volume);
        }

        private void ZoneChangeVolume(CommonCommandGroupType zone, double volume)
        {
            string header;

            switch (zone)
            {
                case CommonCommandGroupType.AvrZone1:
                    header = Zone1Header;
                    break;

                case CommonCommandGroupType.AvrZone2:
                    header = Zone2Header;
                    break;

                // Should never reach here, don't bother logging.
                default:
                    return;
            }

            // Input volume is already in correct scale, just need to cast it
            uint vol = (uint)volume;
            var command = new CommandSet(
                        string.Format("{0}SetVolume {1}", zone, vol),
                        string.Format("{0}{1:00}", header, vol),
                        zone,
                        null,
                        false,
                        CommandPriority.Normal,
                        StandardCommandsEnum.Vol)
            {
                AllowIsQueueableOverride = true,
                AllowIsSendableOverride = true,
                AllowRemoveCommandOverride = true
            };

            // Notify volume controller that we're sending a volume command
            // which means it needs to ensure we poll later even if this
            // command is not queued. Otherwise, fake feedback will leave the
            // application with the wrong volume value.
            MuteVolControllerForCommand(zone).StartControllingVolume();

            SendCommand(command);
        }

        #endregion
        #region Overrides to forward to helpers

        public override void PressVolumeUp()
        {
            _mainVolumeController.VolumeRamper.Start(true);
        }

        public override void PressVolumeDown()
        {
            _mainVolumeController.VolumeRamper.Start(false);
        }
        public override void ReleaseVolume()
        {
            _mainVolumeController.VolumeRamper.Stop();
        }

        public override void SetVolume(uint volume)
        {
            // This will trigger logic to determine what volume should be and
            // send the command
            _mainVolumeController.VolumeLevel.Percent = volume;
        }

        protected override void DeConstructVolume(string response)
        {
            double level;

            if (response.StartsWith(MaxVolumePrefix))
            {
                response = response.Substring(MaxVolumePrefix.Length);
                if (TryParseVolume(response, out level))
                {
                    // Pass along to controller
                    _mainVolumeController.UpdateScale(level);
                }
            }
            else if (_mainVolumeController.MuteVol.VolumeFeedbackOk())
            {
                if (TryParseVolume(response, out level))
                {
                    // Pass along to controller
                    _mainVolumeController.VolumeLevel.ProcessDeviceFeedback(level);
                }
            }
            else
            {
                if (EnableLogging)
                {
                    Log("Ignoring volume feedback while setting volume");
                }
            }
        }

        public override bool OverrideZoneSetVolume(CommonCommandGroupType zone, uint volume)
        {
            SoundUnitedVolumeController ctrl = VolumeControllerForCommand(zone);
            if (ctrl != _mainVolumeController)
            {
                ctrl.VolumeLevel.Percent = volume;
            }
            return true;
        }

        private void UpdateZoneVolumeResponse(ValidatedRxData data)
        {
            // Check if we should be ignoring volume feedback.
            // NOTE: Calling VolumeFeedbackOk() HAS SIDE EFFECTS so be
            // careful how you move this. It assumes that if it returns "true"
            // then you are processing the feedback and it uses that in the
            // logic to determine that polling can stop.
            if (!MuteVolControllerForCommand(data.CommandGroup).VolumeFeedbackOk())
            {
                if (EnableLogging)
                {
                    Log("Ignoring volume feedback while setting volume");
                }

                data.Ignore = true;
                return;
            }

            double level;
            if (TryParseVolume(data.Data, out level))
            {
                // Pass along to controller
                VolumeControllerForCommand(data.CommandGroup).VolumeLevel.ProcessDeviceFeedback(level);
            }
            else
            {
                if (EnableLogging)
                {
                    Log(string.Format("UpdateZoneVolumeResponse - Unable to parse volume response: {0}", data.Data));
                }
            }
        }

        public override bool OverrideZoneVolumeUp(CommonCommandGroupType zone, CommandAction action)
        {
            ZoneVolumeAction(zone, action, true);
            return true;
        }

        public override bool OverrideZoneVolumeDown(CommonCommandGroupType zone, CommandAction action)
        {
            ZoneVolumeAction(zone, action, false);
            return true;
        }

        private void ZoneVolumeAction(CommonCommandGroupType zone, CommandAction action, bool up)
        {
            SoundUnitedVolumeController ctrl = VolumeControllerForCommand(zone);
            if (ctrl != _mainVolumeController)
            {
                if (action == CommandAction.Hold)
                {
                    ctrl.VolumeRamper.Start(up);
                }
                else
                {
                    ctrl.VolumeRamper.Stop();
                }
            }
        }

        #endregion

        public override void DataHandler(string rx)
        {
            // Split all received messages so that ResponseValidator gets one response at a time
            // Handle case where response doesn't end in delimiter but contains it
            if (rx.EndsWith(ApiDelimiter))
            {
                _responseBuffer.Append(rx);
                var splitResponses = Regex.Split(_responseBuffer.ToString(), _delimiterPattern);
                _responseBuffer.Length = 0;

                for (int i = 0; i < splitResponses.Length; i++)
                    base.DataHandler(splitResponses[i]);
            }
            else
            {
                _responseBuffer.Append(rx);
            }
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            if (!string.IsNullOrEmpty(validatedData.CustomCommandGroup) &&
                validatedData.CustomCommandGroup.Equals(TuningModeCommandGroup))
            {
                DeConstructFrequencyMode(validatedData.Data);
            }
            else
            {
                if (validatedData.CommandGroup == CommonCommandGroupType.AvrZone1 ||
                    validatedData.CommandGroup == CommonCommandGroupType.AvrZone2)
                {
                    switch (validatedData.CustomCommandGroup)
                    {
                        case SoundUnitedAvrResponseValidation.ZoneVolumeCommandGroup:
                            // Handling scaling and if volume shoulld be ignored for the zone
                            UpdateZoneVolumeResponse(validatedData);
                            break;
                        case SoundUnitedAvrResponseValidation.ZoneInputCommandGroup:
                            // Making sure every input poll responses goes through audio and video feedback handling
                            AAvrZone zone = GetZone(validatedData.CommandGroup);
                            if (zone != null)
                            {
                                zone.DeconstructAudioInput(validatedData.Data);
                                zone.DeconstructVideoInput(validatedData.Data);
                            }
                            break;
                        default:
                            base.ChooseDeconstructMethod(validatedData);
                            break;
                    }
                }
                else
                {
                    base.ChooseDeconstructMethod(validatedData);
                }
            }
        }

        private void PowerOnCommandCallback()
        {
            // Only need to delay for a long time after turning on the first zone
            // that brings the device out of standby mode
            if (!PowerIsOn && !_zone1.PowerIsOn && !_zone2.PowerIsOn)
            {
                _lastStandbyExitTick = (uint)CrestronEnvironment.TickCount;
            }
        }

        /// <summary>
        /// Helper function to get the correct Mute/Volume controller based on the CommonCommandGroupType
        /// </summary>
        /// <param name="type">The enum value corresponding to the command/operation</param>
        /// <returns>AvrZoneN for the Nth mute controller, or the main zone one for all others</returns>
        private SoundUnitedMuteVolController MuteVolControllerForCommand(CommonCommandGroupType type)
        {
            return VolumeControllerForCommand(type).MuteVol;
        }

        private SoundUnitedVolumeController VolumeControllerForCommand(CommonCommandGroupType type)
        {
            // Main zone commands use the CommonCommandGroupType for the command
            // Ones for zones use the zone-specific group, so default to the main
            // zone and return the other zone controllers only for those specific
            // command groups
            SoundUnitedVolumeController controller = _mainVolumeController;
            switch (type)
            {
                case CommonCommandGroupType.AvrZone1:
                    controller = _zone1VolumeController;
                    break;

                case CommonCommandGroupType.AvrZone2:
                    controller = _zone2VolumeController;
                    break;

                default:
                    break;
            }

            return controller;
        }

        protected override bool PrepareStringThenSend(CommandSet commandSet)
        {
            if (!commandSet.CommandPrepared)
            {
                // Attach callback functions to certain commands
                switch (commandSet.StandardCommand)
                {
                    // Track when the last mute command was sent.
                    case StandardCommandsEnum.Mute:
                    case StandardCommandsEnum.MuteOff:
                    case StandardCommandsEnum.MuteOn:
                        {
                            // At the time of writing, the framework does not use this callback, but we
                            // use the chained calls anyway in case the framework later is updated
                            // to need the callback, too.
                            Action callback = MuteVolControllerForCommand(commandSet.CommandGroup).MuteCommandChanged;
                            commandSet.CallBack = ActionSequence.Chain(callback, commandSet.CallBack);
                        }
                        break;

                    // Track volume commands because if mute is on while they
                    // are sent, the device will unmute and may miss a second
                    // volume command immediately after it during the unmuting process
                    case StandardCommandsEnum.Vol:
                    case StandardCommandsEnum.VolPlus:
                    case StandardCommandsEnum.VolMinus:
                        {
                            // At the time of writing, the framework does not use this callback, but we
                            // use the chained calls anyway in case the framework later is updated
                            // to need the callback, too.
                            Action callback = MuteVolControllerForCommand(commandSet.CommandGroup).VolCommandSent;
                            commandSet.CallBack = ActionSequence.Chain(callback, commandSet.CallBack);
                        }
                        break;

                    // Also need to know when PowerOn commands are sent to
                    // appropriately space out power-on commands for zones
                    case StandardCommandsEnum.PowerOn:
                        {
                            // First use our PowerOnCommandCallback, then call the
                            // default callback from the framework to complete warmup
                            commandSet.CallBack = ActionSequence.Chain(PowerOnCommandCallback, commandSet.CallBack);
                        }
                        break;

                    default:
                        break;
                }

                commandSet.Command = commandSet.Command + ApiDelimiter;
                commandSet.CommandPrepared = true;
            }

            // If this is a Bass or Treble command, then tone control must first be enabled if disabled
            // If this is a tuner frequency up/down command, need to set it into the right mode for manual/auto
            switch (commandSet.StandardCommand)
            {
                case StandardCommandsEnum.ToneSetBass:
                case StandardCommandsEnum.ToneSetTreble:
                case StandardCommandsEnum.ToneBassReset:
                case StandardCommandsEnum.ToneTrebleReset:
                case StandardCommandsEnum.ToneTrebleLevelUp:
                case StandardCommandsEnum.ToneTrebleLevelDown:
                case StandardCommandsEnum.ToneBassLevelDown:
                case StandardCommandsEnum.ToneBassLevelUp:
                    if (!ToneControlIsOn)
                    {
                        // Hold off on tone control commands until tone control is enabled
                        _pendingToneControlCommands.Add(commandSet);

                        var toneControlCommand = BuildCommand(
                            StandardCommandsEnum.ToneControlOn, CommonCommandGroupType.Audio, CommandPriority.High);

                        if (toneControlCommand != null)
                            SendCommand(toneControlCommand);

                        // Prevent the base code from sending anything
                        return true;
                    }
                    break;
                case StandardCommandsEnum.TunerAutoFrequencyDown:
                case StandardCommandsEnum.TunerAutoFrequencyUp:
                    if (_frequencyTuningMode != FrequencyTunningMethod.Automatic)
                    {
                        // Hold off on frequency commands until auto frequency mode is enabled
                        _pendingFrequencyCommands.Add(commandSet);

                        // Use BuildCommand with NotAStandardCommand to allow
                        // setting AllowIsSendableOverride and AllowIsQueueableOverride
                        // without breaking compatibility with older frameworks
                        var cmd = BuildCommand(
                            StandardCommandsEnum.NotAStandardCommand,
                            CommonCommandGroupType.TunerFrequency,
                            CommandPriority.High,
                            "AutoFrequencyMode",
                            TuningModeAuto
                        );
                        cmd.CallBack = AutomaticModeFrequencySent;
                        SendCommand(cmd);

                        // Prevent the base code from sending anything
                        return true;
                    }
                    break;
                case StandardCommandsEnum.TunerFrequencyDown:
                case StandardCommandsEnum.TunerFrequencyUp:
                    if (_frequencyTuningMode != FrequencyTunningMethod.Manual)
                    {
                        // Hold off on frequency commands until auto frequency mode is enabled
                        _pendingFrequencyCommands.Add(commandSet);

                        // Use BuildCommand with NotAStandardCommand to allow
                        // setting AllowIsSendableOverride and AllowIsQueueableOverride
                        // without breaking compatibility with older frameworks
                        var cmd = BuildCommand(
                            StandardCommandsEnum.NotAStandardCommand,
                            CommonCommandGroupType.TunerFrequency,
                            CommandPriority.High,
                            "ManualFrequencyMode",
                            TuningModeManual
                        );
                        cmd.CallBack = ManualModeFrequencySent;
                        SendCommand(cmd);

                        // Prevent the base code from sending anything
                        return true;
                    }
                    break;
                default:
                    // Nothing extra to do for other commands
                    break;
            }

            return base.PrepareStringThenSend(commandSet);
        }

        // Override this to delay sending certain commands until the device is
        // ready to receive them
        protected override bool CanSendCommand(CommandSet commandSet)
        {
            string unused;
            bool canSend;

            switch (commandSet.CommandGroup)
            {
                case CommonCommandGroupType.AvrZone1:
                case CommonCommandGroupType.AvrZone2:
                case CommonCommandGroupType.AvrZone3:
                case CommonCommandGroupType.AvrZone4:
                case CommonCommandGroupType.AvrZone5:
                    // Base version only handles zones properly
                    canSend = base.CanSendCommand(commandSet);
                    break;

                default:
                    // Use default version for non-zone commands
                    bool SupportsPowerFeedback = false;
                    AvrData.CrestronSerialDeviceApi.DeviceSupport.TryGetValue(SupportedFeatureEnum.SupportsPowerFeedback, out SupportsPowerFeedback);
                    canSend = CommandQueueingOverrides.IsSendable(commandSet, WarmingUp, CoolingDown, PowerIsOn, SupportsPowerFeedback, false, out unused);
                    break;
            }

            if (canSend)
            {
                // Check if the command is sendable based on device-specific
                // workaround requirements
                switch (commandSet.StandardCommand)
                {
                    // Check if we can send mute commands
                    case StandardCommandsEnum.Mute:
                    case StandardCommandsEnum.MuteOff:
                    case StandardCommandsEnum.MuteOn:
                        canSend = MuteVolControllerForCommand(commandSet.CommandGroup).CanSendMute;
                        break;

                    // Check if volume commands are sendable
                    case StandardCommandsEnum.Vol:
                    case StandardCommandsEnum.VolPlus:
                    case StandardCommandsEnum.VolMinus:
                        canSend = MuteVolControllerForCommand(commandSet.CommandGroup).CanSendVol;
                        break;

                    // Added check to see if the driver already sent a power command and needs to hold off longer
                    // Zones may not power on reliably if different zones are being powered on at the same time.
                    case StandardCommandsEnum.PowerOn:
                        if ((uint)CrestronEnvironment.TickCount - _lastStandbyExitTick < _timeBetweenPowerCommands)
                        {
                            // It is too early to send it
                            canSend = false;
                        }
                        break;

                    default:
                        break;
                }
            }

            return canSend;
        }

        // Override this because the default implementation does not check if
        // a power-on command exists in the queue for the main zone, so the
        // default implementation will drop input-switch commands if the main
        // zone power-on command is queued since another zone was sent first
        // Also note that since we don't know which power-on command is in the
        // queue, we could potentally allow queueing an input command for the
        // main zone because an alternate zone's power on command is in the
        // queue. The only workaround would be a shadow copy of the "power on
        // command is in the queue" information which is just not worth it to
        // attempt to keep track of.
        protected override bool CanQueueCommand(CommandSet commandSet, bool powerOnCommandExistsInQueue)
        {
            string unused;

            switch (commandSet.CommandGroup)
            {
                case CommonCommandGroupType.AvrZone1:
                case CommonCommandGroupType.AvrZone2:
                case CommonCommandGroupType.AvrZone3:
                case CommonCommandGroupType.AvrZone4:
                case CommonCommandGroupType.AvrZone5:
                    return base.CanQueueCommand(commandSet, powerOnCommandExistsInQueue);

                default:
                    bool SupportsPowerFeedback = false;
                    AvrData.CrestronSerialDeviceApi.DeviceSupport.TryGetValue(SupportedFeatureEnum.SupportsPowerFeedback, out SupportsPowerFeedback);
                    // Note the difference from RADCommon: PowerIsOn || powerOnCommandExistsInQueue. Common only uses PowerIsOn
                    return CommandQueueingOverrides.IsQueueable(commandSet, WarmingUp, CoolingDown, PowerIsOn || powerOnCommandExistsInQueue, SupportsPowerFeedback, false, out unused);
            }
        }

        private void ManualModeFrequencySent()
        {
            // For cases where the tuning mode is unknown and it is already set to the target setting
            if (_frequencyTuningMode == FrequencyTunningMethod.Unknown)
                DeConstructFrequencyMode(TuningModeManual);
        }

        private void AutomaticModeFrequencySent()
        {
            // For cases where the tuning mode is unknown and it is already set to the target setting
            if (_frequencyTuningMode == FrequencyTunningMethod.Unknown)
                DeConstructFrequencyMode(TuningModeAuto);
        }

        private void PollMainVolume()
        {
            var cmd = BuildCommand(
                StandardCommandsEnum.NotAStandardCommand,
                CommonCommandGroupType.Volume,
                CommandPriority.Normal,
                "VolumePoll",
                "MV?"
            );
            SendCommand(cmd);
        }

        protected override void Poll()
        {
            if (!_initialized)
            {
                return;
            }

            // If the driver stopped controlling volume, then this will poll for necessary volume states
            // since feedback is ignored while controlling volume.
            // The main zone is always polled because changing the surround mode causes
            // the volume range to change *after a delay*. Polling from DeConstructSurroundMode
            // ends up getting the old range as a response.
            PollMainVolume();

            try
            {
                if (_zone1VolumeController.MuteVol.NeedToPollVolume)
                {
                    SendCommand(new CommandSet(
                    "VolumePoll",
                    "Z2?",
                    CommonCommandGroupType.AvrZone1,
                    null,
                    false,
                    CommandPriority.Normal,
                    StandardCommandsEnum.NotAStandardCommand)
                    {
                        AllowIsQueueableOverride = true,
                        AllowIsSendableOverride = true,
                        AllowRemoveCommandOverride = true,
                        SubCommandGroup = CommonCommandGroupType.Volume
                    });
                }

                if (_zone2VolumeController.MuteVol.NeedToPollVolume)
                {
                    SendCommand(new CommandSet(
                        "VolumePoll",
                        "Z3?",
                        CommonCommandGroupType.AvrZone2,
                        null,
                        false,
                        CommandPriority.Normal,
                        StandardCommandsEnum.NotAStandardCommand)
                        {
                            AllowIsQueueableOverride = true,
                            AllowIsSendableOverride = true,
                            AllowRemoveCommandOverride = true,
                            SubCommandGroup = CommonCommandGroupType.Volume,
                        });
                }
            }
            catch (MissingMemberException)
            {
                // This will be thrown in older frameworks where
                // AllowIsQueueableOverride and others do not exist.
                // However, in frameworks that support multizone, they
                // will. So we just are avoiding crashing in older
                // frameworks here.
            }
        }

        protected override void DeConstructPower(string response)
        {
            base.DeConstructPower(response);

            // If power is off, it means that the mute state will be off when the device is turned on again
            if (!PowerIsOn)
                DeConstructMute(ValidatedData.MuteFeedback.Feedback[StandardFeedback.MuteStatesFeedback.Off]);
        }

        private void ZonePowerChanged(object sender, Crestron.RAD.Common.Events.ValueEventArgs<bool> e)
        {
            // Same as main zone - turning off power to a zone while mute is enabled will disable mute on the zone
            var avrZone = sender as AAvrZone;

            if (avrZone != null && !e.Value)
                avrZone.IsMuted = false;
        }

        public override void DeConstructToneState(string response)
        {
            // This header differs from what the JSON contains
            // The base code will gather all PS responses here except for PSBAS and PSTRE
            if (response.Contains("TONE CTRL "))
            {
                response = response.Replace("TONE CTRL ", string.Empty);

                base.DeConstructToneState(response);

                // Send all 
                if (_pendingToneControlCommands.Count > 0 && ToneControlIsOn)
                {
                    for (int i = 0; i < _pendingToneControlCommands.Count; i++)
                        SendCommand(_pendingToneControlCommands[i]);

                    _pendingToneControlCommands.Clear();
                }
            }
        }

        public override void DeConstructTreble(string response)
        {
            response = response.Replace(
                ValidatedData.AudioFeedback.ToneControlFeedback.IndividualGroupHeaders[ToneControlType.Treble],
                string.Empty);

            int incomingTrebleLevel = int.MinValue;

            try
            {
                // Subtracting 50 from provided value for scaling. 
                incomingTrebleLevel = int.Parse(response) - 50;
            }
            catch
            {
                if (EnableLogging)
                    Log(string.Format("DeConstructTreble - Unable to parse int value from {0}", response));
            }

            if (incomingTrebleLevel != int.MinValue)
            {
                var percentageValue = MathFunctions.ScaleRangeToPercent(incomingTrebleLevel, _toneMinValue, _toneMaxValue);
                base.DeConstructTreble(Math.Ceiling(percentageValue).ToString());
            }
        }

        public override void DeConstructBass(string response)
        {
            response = response.Replace(
                ValidatedData.AudioFeedback.ToneControlFeedback.IndividualGroupHeaders[ToneControlType.Bass],
                string.Empty);

            int incomingBassLevel = int.MinValue;

            try
            {
                // Subtracting 50 from provided value for scaling. 
                incomingBassLevel = int.Parse(response) - 50;
            }
            catch
            {
                if (EnableLogging)
                    Log(string.Format("DeConstructBass - Unable to parse int value from {0}", response));
            }

            if (incomingBassLevel != int.MinValue)
            {
                var percentageValue = MathFunctions.ScaleRangeToPercent(incomingBassLevel, _toneMinValue, _toneMaxValue);
                base.DeConstructBass(Math.Ceiling(percentageValue).ToString());
            }
        }

        public override void SetBass(int level)
        {
            // Scale from percentage to device level
            float scaledValue = float.MinValue;
            try
            {
                // Adding 50 to use device's range
                scaledValue = MathFunctions.ScalePercentToRange(level, _toneMinValue, _toneMaxValue) + 50;
            }
            catch
            {
                if (EnableLogging)
                    Log(string.Format("SetBass - Unable to scale provided level to device's range: {0}", level));
            }

            if (scaledValue != float.MinValue)
            {
                var cmd = BuildCommand(
                    StandardCommandsEnum.ToneSetBass,
                    CommonCommandGroupType.Audio,
                    CommandPriority.Normal,
                    string.Format("SetBass {0}", level),
                    string.Format("PSBAS {0}", Math.Ceiling(scaledValue))
                );
                SendCommand(cmd);
            }
        }

        public override void SetTreble(int level)
        {
            // Scale from percentage to device level
            float scaledValue = float.MinValue;
            try
            {
                scaledValue = MathFunctions.ScalePercentToRange(level, _toneMinValue, _toneMaxValue) + 50;
            }
            catch
            {
                if (EnableLogging)
                    Log(string.Format("SetTreble - Unable to scale provided level to device's range: {0}", level));
            }

            if (scaledValue != float.MinValue)
            {
                var cmd = BuildCommand(
                    StandardCommandsEnum.ToneSetTreble,
                    CommonCommandGroupType.Audio,
                    CommandPriority.Normal,
                    string.Format("SetTreble {0}", level),
                    string.Format("PSTRE {0}", Math.Ceiling(scaledValue))
                );
                SendCommand(cmd);
            }
        }

        private void DeConstructFrequencyMode(string response)
        {
            if (response.Contains(TuningModeAuto))
                _frequencyTuningMode = FrequencyTunningMethod.Automatic;
            else if (response.Contains(TuningModeManual))
                _frequencyTuningMode = FrequencyTunningMethod.Manual;

            if (_pendingFrequencyCommands.Count > 0)
            {
                for (int i = 0; i < _pendingFrequencyCommands.Count; i++)
                    SendCommand(_pendingFrequencyCommands[i]);

                _pendingFrequencyCommands.Clear();
            }
        }

        public override void DeConstructFrequency(string response)
        {
            int parsedValue = int.MinValue;
            try
            {
                parsedValue = int.Parse(response);
            }
            catch
            {
                if (EnableLogging)
                    Log(string.Format("DeConstructFrequency - unable to determine frequency band: {0}", response));
            }

            if (parsedValue > 50000)
            {
                DeConstructTunerBand("AM");
                response = string.Format("{0} khz", parsedValue / 100);
            }
            else if (parsedValue < 50000 && parsedValue != 0)
            {
                DeConstructTunerBand("FM");
                if (response.Length > 5)
                {
                    var major = int.Parse(response.Substring(0, response.Length - 2));
                    var minor = int.Parse(response.Substring(response.Length - 2, 2));

                    response = string.Format("{0}.{1} Mhz",
                                major,
                                minor); ;
                }
            }

            base.DeConstructFrequency(response);
        }

        public override void SetSpecificTunerFrequency(string value)
        {
            int parsedValue = int.MinValue;
            try
            {
                parsedValue = int.Parse(value);
            }
            catch
            {
                if (EnableLogging)
                    Log(string.Format("SetSpecificTunerFrequency - invalid value specified: {0}", value));
            }

            if (parsedValue != int.MinValue)
            {
                string command = null;

                switch (FrequencyBandIs)
                {
                    case FrequencyBand.Am:
                        parsedValue = parsedValue * 100;
                        command = string.Format("TFAN{0}", parsedValue.ToString("000000"));
                        break;
                    case FrequencyBand.Fm:
                        if (parsedValue < 2000)
                            parsedValue = parsedValue * 10;
                        command = string.Format("TFAN{0}", parsedValue.ToString("000000"));
                        break;
                }

                if (!string.IsNullOrEmpty(command))
                {
                    var cmd = BuildCommand(
                        StandardCommandsEnum.TunerFrequency,
                        CommonCommandGroupType.TunerFrequency,
                        CommandPriority.Normal,
                        "Tuner Frequency",
                        command
                    );
                    SendCommand(cmd);
                }
            }
        }


        public override void DeConstructSurroundMode(string response)
        {
            switch (response)
            {
                case "AUTO":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Auto] = response;
                    break;
                case "MOVIE":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Movie] = response;
                    break;
                case "MUSIC":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Music] = response;
                    break;
                case "GAME":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Game] = response;
                    break;
                case "DIRECT":
                case "DSD DIRECT":
                case "M CH IN+DS":
                case "M CH IN+NEURAL:X":
                case "M CH IN+VIRTUAL:X":
                case "NEURAL:X":
                case "VIRTUAL:X":
                case "VIRTUAL":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Direct] = response;
                    break;
                case "PURE DIRECT":
                case "DSD PURE DIRECT":
                case "MULTI CH IN":
                case "MULTI CH IN 7.1":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.PureDirect] = response;
                    break;
                case "STEREO":
                case "MCH STEREO":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Stereo] = response;
                    break;
                case "DOLBY SURROUND":
                case "DOLBY ATMOS":
                case "DOLBY DIGITAL":
                case "DOLBY D+DS":
                case "DOLBY D+NEURAL:X":
                case "DOLBY D+":
                case "DOLBY D+ +DS":
                case "DOLBY D+ +NEURAL:X":
                case "DOLBY HD":
                case "DOLBY HD+DS":
                case "DOLBY HD+NEURAL:X":
                case "AURO3D":
                case "AURO2DSURR":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.DolbyDigital] = response;
                    break;
                case "DTS SURROUND":
                case "DTS ES DSCRT6.1":
                case "DTS ES MTRX6.1":
                case "DTS+DS":
                case "DTS96/24":
                case "DTS96 ES MTRX":
                case "DTS+NEURAL:X":
                case "DTS+VIRTUAL:X":
                case "DTS ES MTRX+NEURAL:X":
                case "DTS ES DSCRT+NEURAL:X":
                case "DTS HD":
                case "DTS HD MSTR":
                case "DTS HD+DS":
                case "DTS HD+NEURAL:X":
                case "DTS HD+VIRTUAL:X":
                case "DTS:X":
                case "DTS:X MSTR":
                case "DTS:X+VIRTUAL:X":
                case "DTS EXPRESS":
                case "DTS ES 8CH DSCRT":
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.DtsSurround] = response;
                    break;
                default:
                    ValidatedData.Feedback.SurroundModeFeedback.Feedback[SurroundModeType.Unknown] = response;
                    break;
            }
            base.DeConstructSurroundMode(response);
        }


        protected override void DeConstructInput(string response)
        {
            if (response.Contains("NET") &&
                _currentNetworkInput == AudioConnections.Bluetooth1)
            {
                base.DeConstructInput(response);
                DeConstructAudioInput("BT");
            }
            else
            {
                base.DeConstructInput(response);
                DeConstructAudioInput(response);
            }
        }

        private bool TryParseVolume(string input, out double volume)
        {
            volume = default(double);

            try
            {
                int parsedValue = int.Parse(input);

                // Handle half-steps (main zone only), device would report a value greater than 100 if there is a decimal
                // 35.5 comes in as 355. Values such as 3.5 come in as 35
                // 355 = 35.5
                // 035 = 3.5
                // 03 = 3
                // 005 = 0.5
                // 015 = 1.5
                // 000 = 0
                volume = parsedValue;
                if (input.Length != 2)
                {
                    volume /= 10;
                }

                return true;
            }
            catch
            {
                if (EnableLogging)
                {
                    Log(string.Format("DeConstructVolume - Unable to scale device level to percent range: {0}", input));
                }
                return false;
            }
        }

        public override bool OverrideZoneMuteOff(CommonCommandGroupType zone)
        {
            // Base code checks the protocol's mute state. Overriding this to work around that.
            AAvrZone avrZone = GetZone(zone);

            // Changing mute state incurs delays with the volume/mute holdoff
            // workaround logic on this device. So only send the command if
            // we need to (like the main zone does).
            if (avrZone != null && avrZone.IsMuted)
            {
                Commands command = null;
                if (avrZone.StandardCommands.TryGetValue(StandardCommandsEnum.MuteOff, out command))
                {
                    SendCommand(new CommandSet(
                        "MuteOff",
                        command.Command,
                        zone,
                        null,
                        false,
                        CommandPriority.Normal,
                        StandardCommandsEnum.MuteOff)
                        {
                            AllowIsQueueableOverride = true,
                            AllowIsSendableOverride = true,
                            AllowRemoveCommandOverride = true
                        });
                }
            }
            return true;
        }

        public override bool OverrideZoneMuteOn(CommonCommandGroupType zone)
        {
            // Base code checks the protocol's mute state. Overriding this to work around that.
            AAvrZone avrZone = GetZone(zone);

            // Changing mute state incurs delays with the volume/mute holdoff
            // workaround logic on this device. So only send the command if
            // we need to (like the main zone does).
            if (avrZone != null && !avrZone.IsMuted)
            {
                Commands command = null;
                if (avrZone.StandardCommands.TryGetValue(StandardCommandsEnum.MuteOn, out command))
                {
                    SendCommand(new CommandSet(
                        "MuteOn",
                        command.Command,
                        zone,
                        null,
                        false,
                        CommandPriority.Normal,
                        StandardCommandsEnum.MuteOn)
                        {
                            AllowIsQueueableOverride = true,
                            AllowIsSendableOverride = true,
                            AllowRemoveCommandOverride = true
                        });
                }
            }

            return true;
        }

        // Overridden from framework version because framework version will
        // fake feedback even when warming up. This feedback then gets sent
        // out with the next volume update and can result in the driver getting
        // out of sync with the device's mute state. The change here is just to
        // not fake feedback.
        public override void MuteOn()
        {
            if (!MuteIsOn)
            {
                CommandSet command = BuildCommand(StandardCommandsEnum.MuteOn, CommonCommandGroupType.Mute, CommandPriority.Normal, "Mute On");
                if (command != null)
                {
                    SendCommand(command);
                    MutePoll();
                }
            }
        }

        // Overridden for same reason as MuteOn(). See that comment.
        public override void MuteOff()
        {
            if (MuteIsOn)
            {
                CommandSet command = BuildCommand(StandardCommandsEnum.MuteOff, CommonCommandGroupType.Mute, CommandPriority.Normal, "Mute Off");
                if (command != null)
                {
                    SendCommand(command);
                    MutePoll();
                }
            }
        }

        // Overridden for same reason as MuteOn(). See that comment.
        public override void Mute()
        {

            CommandSet command = BuildCommand(StandardCommandsEnum.Mute, CommonCommandGroupType.Mute, CommandPriority.Normal);
            if (command != null)
            {
                SendCommand(command);
                MutePoll();
            }
        }
    }
}