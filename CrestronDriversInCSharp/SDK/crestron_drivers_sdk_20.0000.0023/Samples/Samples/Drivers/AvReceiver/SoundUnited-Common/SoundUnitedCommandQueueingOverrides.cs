using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;

namespace Crestron.RAD.Drivers.AVReceivers.SoundUnited
{
    /// <summary>
    /// This class reimplements the RADCommon Queueable/Sendable logic so that
    /// we can override it in the driver for our own purposes.
    /// We need to allow commands to be queued if a power-on command is in the
    /// queue (currently only implemented for zones in RADCommon), and we need
    /// to be able to delay sending other commands after powering the device
    /// on. RADCommon Warmup/Cooldown is per-zone, but for the SoundUnited AVR
    /// you have to wait at least 2.1s after powering on the first zone before
    /// you can send additional commands. Otherwise you can miss powering on
    /// the other zones.
    /// </summary>
    /// <remarks>
    /// Note that the names of variables and the structure of the code is
    /// intended to mirror the existing RADCommon implementation as closely
    /// as possible. The idea is to make it as straightforward as we can to
    /// see where the differences are.
    /// Everywhere that has been changed has a corresponding comment, except
    /// for the fact that function signatures accept a CommandSet parameter
    /// since these are no longer methods on a CommandSet object.
    /// </remarks>
    internal class CommandQueueingOverrides 
    {
        public static bool IsQueueable(CommandSet command, bool warmingUp, bool coolingDown, bool powerIsOn, bool supportsPowerFeedback, bool log, out string logMessage)
        {
            var isQueueable = false;
            logMessage = null;

            if (warmingUp)
            {
                isQueueable = _isQueueableDuringWarmup(command);
                if (!isQueueable && log)
                    logMessage = "Only Input/AudioInput/MediaService/Power command groups are allowed during WarmUp";
            }
            else if (coolingDown)
            {
                isQueueable = _isQueueableDuringCooldown(command);
                if (!isQueueable && log)
                    logMessage = "Only the Power command group is allowed during CoolDown";
            }
            else if (!powerIsOn)
            {
                isQueueable = supportsPowerFeedback ? _isQueueableWhenPowerIsOff(command) : true;
                if (!isQueueable && log)
                    logMessage = "Only the Power command group is allowed while power is off";
            }
            else
                isQueueable = true;

            return isQueueable;
        }

        public static bool IsSendable(CommandSet command, bool warmingUp, bool coolingDown, bool powerIsOn, bool supportsPowerFeedback, bool log, out string logMessage)
        {
            var isSendable = false;
            logMessage = null;

            if (warmingUp)
            {
                isSendable = _isSendableDuringWarmup(command);

                if (!isSendable && log)
                    logMessage = "Only the PowerPoll and PowerOn commands with no callback are allowed during WarmUp";
            }
            else if (coolingDown)
            {
                isSendable = _isSendableDuringCooldown(command);

                if (!isSendable && log)
                    logMessage = "Only PowerPoll is allowed during CoolDown";
            }
            else if (!powerIsOn)
            {
                isSendable = supportsPowerFeedback ? _isSendableWhenPowerIsOff(command) : true;

                if (!isSendable && log)
                    logMessage = "Only the Power command group is allowed while power is off";
            }
            else
                isSendable = true;

            return isSendable;
        }

        private static bool _isSendableDuringWarmup(CommandSet commandSet)
        {
            if (commandSet == null) { return false; }

            return commandSet.StandardCommand.Equals(StandardCommandsEnum.PowerPoll) ||
                (commandSet.StandardCommand.Equals(StandardCommandsEnum.PowerOn) &&
                commandSet.CallBack == null);
        }

        private static bool _isQueueableDuringWarmup(CommandSet commandSet)
        {
            if (commandSet == null) { return false; }

            // Replaced from RADCommon version because SubCommandGroupIdentifier is internal
            // Some CommandSets use SubCommandGroup to specify what is normally specified
            // in CommandGroup while using CommandGroup to be something more broad, such as
            // AVR Zones using their zone identifier as CommandGroup and AV switchers using
            // the extender CommandGroup.
#if false
            CommonCommandGroupType commandGroup = CommonCommandGroupType.Unknown;

            if (string.IsNullOrEmpty(commandSet.SubCommandGroupIdentifier))
                commandGroup = commandSet.CommandGroup;
            else
                commandGroup = commandSet.SubCommandGroup;
#endif
            // Replacement code (doesn't support AV switchers)
            CommonCommandGroupType commandGroup = commandSet.CommandGroup;
            if (commandGroup == CommonCommandGroupType.Unknown)
            {
                commandGroup = commandSet.SubCommandGroup;
            }
            // End replacement code

            return commandGroup.Equals(CommonCommandGroupType.Input) ||
                    commandGroup.Equals(CommonCommandGroupType.AudioInput) ||
                    commandGroup.Equals(CommonCommandGroupType.MediaService) ||
                    commandGroup.Equals(CommonCommandGroupType.Power);
#if false // Removed since we already can't support AV switchers due to internal fields above
                    || commandGroup.Equals(CommonCommandGroupType.AudioVideoSwitcher);
#endif
        }

        private static bool _isSendableDuringCooldown(CommandSet commandSet)
        {
            if (commandSet == null) { return false; }

            return commandSet.StandardCommand.Equals(StandardCommandsEnum.PowerPoll);
        }

        private static bool _isQueueableDuringCooldown(CommandSet commandSet)
        {
            if (commandSet == null) { return false; }

            return commandSet.CommandGroup.Equals(CommonCommandGroupType.Power);
        }

        private static bool _isSendableWhenPowerIsOff(CommandSet commandSet)
        {
            if (commandSet == null) { return false; }

            return commandSet.CommandGroup.Equals(CommonCommandGroupType.Power) ||
                commandSet.CommandGroup.Equals(CommonCommandGroupType.Connection);
        }

        private static bool _isQueueableWhenPowerIsOff(CommandSet commandSet)
        {
            if (commandSet == null) { return false; }

            return commandSet.CommandGroup.Equals(CommonCommandGroupType.Power) ||
                commandSet.CommandGroup.Equals(CommonCommandGroupType.Connection);
        }
    }
}