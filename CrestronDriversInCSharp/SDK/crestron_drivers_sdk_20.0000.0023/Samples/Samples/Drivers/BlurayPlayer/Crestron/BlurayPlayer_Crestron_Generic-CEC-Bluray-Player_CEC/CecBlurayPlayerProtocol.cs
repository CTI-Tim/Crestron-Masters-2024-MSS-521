
namespace Crestron.RAD.Drivers.BlurayPlayers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Crestron.RAD.Common.BasicDriver;
    using Crestron.RAD.Common.Enums;
    using Crestron.RAD.Common.Transports;
    using Crestron.RAD.DeviceTypes.BlurayPlayer;
    using Crestron.SimplSharp;
    using System.Text;

    public enum CecVersion
    {
        NotRequested = 0x00,
        None = 0x01,
        Version1_3a = 0x04,
        Version1_4ab = 0x05,
        Version2_0 = 0x06
        // Reserved = 0x07, 0x40 - 0xFF
    }

    public class CecBlurayPlayerProtocol : ABlurayPlayerProtocol
    {
        internal bool Initialized;
        internal string LogicalAddress;
        internal uint CecVerValue;
        private CecVersion _cecVersionValue { get; set; }

        private readonly string _standbyFeedback = "\u0090\u0002";
        private readonly string _onToStandbyFeedback = "\u0090\u0003";
        private readonly string _standbyToOnFeedback = "\u0090\u0004";

        private ushort _pollTick = 0;
        private int _lastReceivedTick = 0;
        private const int _responseTimeoutTicks = 30000;

        public CecBlurayPlayerProtocol(ISerialTransport transportDriver)
            : base(transportDriver, 0x00)
        {
            ResponseValidation = new CecBlurayPlayerResponseValidator(Id, ValidatedData);
            ValidatedData.PowerOnPollingSequence = new[] 
            { 
                StandardCommandsEnum.PowerPoll
            };
            LogicalAddress = "\x04";                //default
            PollingInterval = 1000;
        }

        public override void Initialize(object driverData)
        {
            base.Initialize(driverData);
            ValidateResponse = DriverValidateResponse;
        }

        #region Helper Methods

        internal new void Log(string message)
        {
            if (message != null) base.Log(message);
        }

        private void AssignLogicalAddress(string result)
        {
            switch (result)
            {
                case "\x40":
                    {
                        LogicalAddress = "\x04";
                        break;
                    }
                case "\x80":
                    {
                        LogicalAddress = "\x08";
                        break;
                    }
                case "\xB0":
                    {
                        LogicalAddress = "\x0B";
                        break;
                    }
                case "\x4F":
                    {
                        LogicalAddress = "\x04";
                        break;
                    }
                case "\x8F":
                    {
                        LogicalAddress = "\x08";
                        break;
                    }
                case "\xBF":
                    {
                        LogicalAddress = "\x0B";
                        break;
                    }
                default:
                    {
                        Log("No response from device when polled for CEC version");
                        break;
                    }
            }
        }

        #endregion Helper Methods

        protected override bool PrepareStringThenSend(CommandSet commandSet)
        {
            if (!commandSet.CommandPrepared)
            {
                // No release on power commands, only a warmup/cooldown period
                // Release on all other 0x44 commands
                if (commandSet.Command.Contains("\x44") && commandSet.CommandGroup != CommonCommandGroupType.Power)
                    commandSet.CallBack = SendRelease;
                
                commandSet.Command = commandSet.Command.Replace("[address]", LogicalAddress);
                commandSet.CommandPrepared = true;
            }
            
            return base.PrepareStringThenSend(commandSet);
        }

        protected override void Poll()
        {
            // This will be invoked once every second

            // Find out if the device hasn't sent anything for 30 seconds or more
            if (IsConnected &&
                Math.Abs(CrestronEnvironment.TickCount - _lastReceivedTick) >= _responseTimeoutTicks)
            {
                if (EnableLogging)
                    Log("No communication frm device for over 30 seconds");

                ConnectionChanged(false);
            }

            string command = null;
            // Send a command to find out the logical address of the device
            switch (++_pollTick)
            {
                case 1:
                    command = "\x04\x9F";
                    break;
                case 2:
                    command = "\x08\x9F";
                    break;
                case 3:
                    command = "\x09\x9F";
                    break;
                case 4:
                    command = "\x0B\x9F";
                    break;
                case 15:
                    _pollTick = 0;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(command))
            {
                SendCommand(new CommandSet(
                    "AddressPoll",
                    command,
                    CommonCommandGroupType.Power,
                    null,
                    true,
                    CommandPriority.Low,
                    StandardCommandsEnum.PowerPoll));
            }
        }

        private void SendRelease()
        {
            // This would get invoked by send logic when the command that needed a release
            // assigned its callback to this method. Send the release with a special priority because it must
            // be the next command sent.
            SendCommand(new CommandSet(
                "Release",
                string.Format("{0}\x45", LogicalAddress),
                CommonCommandGroupType.Other,
                null,
                true,
                CommandPriority.Special,
                StandardCommandsEnum.NotAStandardCommand));
        }

        protected override void CoolDown()
        {
            // Base call needs PowerIsOn to be true
            // This driver doesn't advertise support for power feedback
            // Setting to true to ensure a cooldown event happens.
            PowerIsOn = true;
            base.CoolDown();
            PowerIsOn = false;
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            if (validatedData.CommandGroup == CommonCommandGroupType.Power)
            {
                SetSupportsPowerFeedback(validatedData.Data);
            }
            base.ChooseDeconstructMethod(validatedData);
        }

        private void SetSupportsPowerFeedback(string response)
        {
            if ((ValidatedData.PowerFeedback.Feedback.ContainsKey(StandardFeedback.PowerStatesFeedback.On))
                || (ValidatedData.PowerFeedback.Feedback.ContainsKey(StandardFeedback.PowerStatesFeedback.Off)))
            {
                bool containsKey;
                base.BlurayPlayerData.CrestronSerialDeviceApi.DeviceSupport.TryGetValue(SupportedFeatureEnum.SupportsPowerFeedback, out containsKey);
                if (containsKey)
                {
                    base.BlurayPlayerData.CrestronSerialDeviceApi.DeviceSupport[SupportedFeatureEnum.SupportsPowerFeedback] = true;
                }
                else
                {
                    base.BlurayPlayerData.CrestronSerialDeviceApi.DeviceSupport.Add(SupportedFeatureEnum.SupportsPowerFeedback, true);
                }
            }
        }

        #region ProcessResponses

        private ValidatedRxData ProcessCECVersionResponse(ValidatedRxData validatedData, string response)
        {
            if (response.Contains("\x04"))
            {
                _cecVersionValue = CecVersion.Version1_3a;
                CecVerValue = (uint)_cecVersionValue;
            }
            else if (response.Contains("\x05"))
            {
                _cecVersionValue = CecVersion.Version1_4ab;
                CecVerValue = (uint)_cecVersionValue;
            }
            else if (response.Contains("\x06"))
            {
                _cecVersionValue = CecVersion.Version2_0;
                CecVerValue = (uint)_cecVersionValue;
            }
            else
            {
                _cecVersionValue = CecVersion.None;
                CecVerValue = (uint)_cecVersionValue;
            }
            Initialized = true;
            validatedData.Data = response;
            validatedData.CommandGroup = CommonCommandGroupType.Unknown;
            validatedData.Ready = true;

            return validatedData;
        }

        private ValidatedRxData ProcessPowerResponse(ValidatedRxData validatedData, string response)
        {
            response = response.Remove(0, 1); //remove the logical address
            if (response.Contains(_standbyFeedback))
            {
                response = ValidatedData.PowerFeedback.Feedback[StandardFeedback.PowerStatesFeedback.Off];
            }
            else if (response.Contains(_onToStandbyFeedback))
            {
                response = ValidatedData.PowerFeedback.Feedback[StandardFeedback.PowerStatesFeedback.Off];
            }
            else if (response.Contains(_standbyToOnFeedback))
            {
                response = ValidatedData.PowerFeedback.Feedback[StandardFeedback.PowerStatesFeedback.Off];
            }

            validatedData.CommandGroup = CommonCommandGroupType.Power;
            validatedData.Data = response;
            validatedData.Ready = true;

            return validatedData;
        }

        private ValidatedRxData ProcessPlaybackStatusResponse(ValidatedRxData validatedData, string response)
        {
            validatedData.CommandGroup = CommonCommandGroupType.PlayBackStatus;
            validatedData.Data = response;
            validatedData.Ignore = true;

            return validatedData;
        }

        #endregion ProcessResponses

        #region Validate Response

        public ValidatedRxData DriverValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            // Keep track of last time a response was received
            _lastReceivedTick = CrestronEnvironment.TickCount;

            ValidatedRxData validatedData = new ValidatedRxData(false, string.Empty);

            if (response.Equals("\x05"))
            {
                //CrestronConsole.PrintLine("ignoring device discovery poll request from bluray player");
                validatedData.Ignore = true;
            }
            else if (response.Contains(ValidatedData.PowerFeedback.GroupHeader))
            {
                //CrestronConsole.PrintLine("checking for possible power status response");

                string result = response.Substring(0, 1);
                AssignLogicalAddress(result);

                validatedData = ProcessPowerResponse(validatedData, response);
            }
            else if (response.Contains("\x40\x9E") || response.Contains("\x80\x9E") || response.Contains("\xB0\x9E"))
            {
                //CrestronConsole.PrintLine("checking for possible cec version response");

                string result = response.Substring(0, 1);
                AssignLogicalAddress(result);

                validatedData = ProcessCECVersionResponse(validatedData, response);
            }
            else if (response.Contains("\x4F\x82\x13\x00") || response.Contains("\x8F\x82\x13\x00") || response.Contains("\x9F\x82\x13\x00") ||
                     response.Contains("\xBF\x82\x13\x00"))
            {
                //CrestronConsole.PrintLine("checking for possible playback started status response");

                string result = response.Substring(0, 1);
                AssignLogicalAddress(result);

                validatedData = ProcessPlaybackStatusResponse(validatedData, "play");
            }
            else
            {
                validatedData.Ready = false;
            }

            //CrestronConsole.PrintLine("ready = {0}", validatedData.Ready);
            //byte[] commandBytes = Encoding.GetBytes(response);
            //string readableString = BitConverter.ToString(commandBytes);
            //CrestronConsole.PrintLine("Response: {0}", readableString);

            return validatedData;
        }

        #endregion
    }
}