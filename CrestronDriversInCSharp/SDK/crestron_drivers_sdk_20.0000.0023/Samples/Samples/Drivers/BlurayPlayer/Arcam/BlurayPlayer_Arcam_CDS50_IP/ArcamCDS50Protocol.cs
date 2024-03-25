using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.DeviceTypes.BlurayPlayer;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.Common.Enums;
using Crestron.SimplSharp.CrestronSockets;
using Crestron.RAD.Common.BasicDriver;

namespace Crestron.RAD.Drivers.BlurayPlayers
{
    public class ArcamCDS50Protocol : ABlurayPlayerProtocol
    {
        internal bool initialized = false;

        public ArcamCDS50Protocol(ISerialTransport transportDriver, byte id)
            : base(transportDriver, id)
        {
            ResponseValidation = new ArcamCDS50ResponseValidation(Id, ValidatedData, this);
            ValidatedData.PowerOnPollingSequence = new[]
            {
                StandardCommandsEnum.PowerPoll,
                StandardCommandsEnum.PlayBackStatusPoll,
                StandardCommandsEnum.TrackPoll
            };
        }

        protected override void Poll()
        {
            Transport.Send("\u0021\u0001\u002C\u0001\u00F0\u000D", null);
        }

        public override void PowerOff()
        {
            // This device, over IP, won't respond to PowerPoll when we turn it off right away
            // We will set our own callback for this command, then use that callback to 
            // call the default cooldown logic from the framework, then tell applications
            // that the device is off. We will also ignore Power=On feedback during cooldown as it 
            // sometimes will say it is on still.

            var powerCommand = new CommandSet("PowerOff", "\x21\x01\x08\x02\x14\x0C\x0D", CommonCommandGroupType.Power,
                PowerOffCallback, false, CommandPriority.Highest, StandardCommandsEnum.PowerOff);
            SendCommand(powerCommand);
        }

        private void PowerOffCallback()
        {
            CoolDown();
            PowerIsOn = false;
            base.DeConstructPower("standby");
        }

        protected override bool PrepareStringThenSend(CommandSet commandSet)
        {
            if (!initialized)
            {
                Transport.Send("\u0021\u0001\u0008\u0002\u0014\u005A\u000D", null);
            }

            switch (commandSet.StandardCommand)
            {
                case StandardCommandsEnum.PowerPoll:
                case StandardCommandsEnum.PlayBackStatusPoll:
                case StandardCommandsEnum.TrackPoll:
                case StandardCommandsEnum.PowerOn:
                case StandardCommandsEnum.PowerOff:
                case StandardCommandsEnum.Power:
                    return base.PrepareStringThenSend(commandSet);
                default:
                    // Frequency of device's unsolicited feedbcak is very high
                    // Skip queue for non-polling commands, otherwise slower processors
                    // will get back up while handling all the responses on top of user commands
                    Transport.Send(commandSet.Command, null);
                    return true;
            }
        }

        public override void ArrowKey(ArrowDirections direction)
        {
            switch (direction)
            {
                case ArrowDirections.Down:
                    Transport.Send("\x21\x01\x08\x02\x14\x55\x0D", null);
                    break;
                case ArrowDirections.Left:
                    Transport.Send("\x21\x01\x08\x02\x14\x51\x0D", null);
                    break;
                case ArrowDirections.Right:
                    Transport.Send("\x21\x01\x08\x02\x14\x50\x0D", null);
                    break;
                case ArrowDirections.Up:
                    Transport.Send("\x21\x01\x08\x02\x14\x56\x0D", null);
                    break;
            }

        }

        public void SetDriverInitializedStatus(bool status)
        {
            initialized = status;
        }

        public void LogMessage(string message)
        {
            Log(message);
        }

        internal void DeconstructFeedback(ValidatedRxData data)
        {
            if (CoolingDown &&
                data.CommandGroup == CommonCommandGroupType.Power &&
                data.Data.Equals("on"))
            {
                data.Data = "standby";
                // Ignoreing Power=On feedback during cooldown
                // BlurayPlayer framework did not mark DeconstructPower as virtual, so
                // this must be done here
            }
            base.ChooseDeconstructMethod(data);
        }

        public override void Power()
        {
            switch (PowerIsOn)
            {
                case true:
                    {
                        PowerOff();
                        break;
                    }
                case false:
                    {
                        PowerOn();
                        break;
                    }
            }
        }
    }
}