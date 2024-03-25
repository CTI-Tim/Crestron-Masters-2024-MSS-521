using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.RADAVReceiver;
using Crestron.RAD.Drivers.AVReceivers;
using Crestron.SimplSharp;

namespace Crestron.RAD.Drivers.AVReceivers
{
    public class SoundUnitedAvrResponseValidation : ResponseValidation
    {
        internal const string ZonePowerCommandGroup = "PowerPoll";
        internal const string ZoneVolumeCommandGroup = "VolumePoll";
        internal const string ZoneInputCommandGroup = "InputPoll";
        internal const string ZoneMuteCommandGroup = "MutePoll";

        private const string _onFeedback = "ON";
        private const string _offFeedback = "OFF";

        private static List<string> _ignoredResponses = new List<string>()
        {
            "PSLFE",
            "TFANCMP",
            "SS",
            "OPT"
        };

        public SoundUnitedAvrResponseValidation(byte id, DataValidation dataValidation)
            : base(id, dataValidation)
        {
            Id = id;
            DataValidation = dataValidation;
        }

        public override ValidatedRxData ValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            ValidatedRxData validatedData = new ValidatedRxData(false, string.Empty);

            // Wait for a full response
            if (!response.EndsWith(SoundUnitedAvrProtocol.ApiDelimiter))
                return validatedData;

            response = response.Replace(SoundUnitedAvrProtocol.ApiDelimiter, string.Empty);

            // Messages that the driver doesn't need to process
            if (_ignoredResponses.Any(ignoredResponse => response.StartsWith(ignoredResponse)))
            {
                validatedData.Ignore = true;
                return validatedData;
            }

            // Volume feedback comes in During during fast volume movements using the volume knob on the device

            // Non-standard responses for tuner frequency mode
            if (response.Contains(SoundUnitedAvrProtocol.TuningModeAuto) ||
                response.Contains(SoundUnitedAvrProtocol.TuningModeManual))
            {
                validatedData.Data = response;
                validatedData.CustomCommandGroup = SoundUnitedAvrProtocol.TuningModeCommandGroup;
                validatedData.Ready = true;
                return validatedData;
            }

            // Zone2 and Zone3 share common headers, but don't specify what command group a response is
            // Z2OFF Z2BD Z277 Z3ON Z3BD Z398 except for mute.
            // Figure it out based on the feedback given

            var responseCommandGroup = CommonCommandGroupType.Unknown;

            if (response.StartsWith(SoundUnitedAvrProtocol.Zone1Header))
            {
                responseCommandGroup = CommonCommandGroupType.AvrZone1;
                response = response.Replace(SoundUnitedAvrProtocol.Zone1Header, string.Empty);
            }
            else if (response.StartsWith(SoundUnitedAvrProtocol.Zone2Header))
            {
                responseCommandGroup = CommonCommandGroupType.AvrZone2;
                response = response.Replace(SoundUnitedAvrProtocol.Zone2Header, string.Empty);
            }

            // Main zone
            if (responseCommandGroup == CommonCommandGroupType.Unknown)
            {
                validatedData = base.ValidateResponse(response, CommonCommandGroupType.Unknown);
                if (validatedData.Ready &&
                    !string.IsNullOrEmpty(validatedData.Data))
                {
                    if (validatedData.CommandGroup == CommonCommandGroupType.Unknown)
                    {
                        // Ignore anything the base code couldn't figure out
                        validatedData.Ignore = true;
                    }
                    else if (validatedData.CommandGroup == CommonCommandGroupType.ToneControl)
                    {
                        // Base code sets the command group to the wrong one
                        validatedData.CommandGroup = CommonCommandGroupType.ToneState;
                    }
                    else if (validatedData.CommandGroup == CommonCommandGroupType.Volume)
                    {
                        // Base code messes up the volume level, manually copy it back here
                        validatedData.Data = response.Replace(DataValidation.VolumeFeedback.GroupHeader, string.Empty);
                    }
                }
                else
                {
                    // Ignore anything not marked as ready since it was the full response
                    validatedData.Ignore = true;
                }

                return validatedData;
            }
            // Zone 2 and 3
            else
            {
                // If it contains MU header, then it is mute feedback for the zone
                // if there is no header, then the on/off feedback is power
                if (response.Contains(DataValidation.MuteFeedback.GroupHeader))
                {
                    validatedData.Data = response.Replace(DataValidation.MuteFeedback.GroupHeader, string.Empty);
                    validatedData.CommandGroup = responseCommandGroup;
                    validatedData.CustomCommandGroup = ZoneMuteCommandGroup;
                    validatedData.Ready = true;
                }
                else if (response == _onFeedback || response == _offFeedback)
                {
                    validatedData.Data = response;
                    validatedData.CommandGroup = responseCommandGroup;
                    validatedData.CustomCommandGroup = ZonePowerCommandGroup;
                    validatedData.Ready = true;
                }
                else
                {
                    // Can be either an input or volume level. Try to parse as int and if that fails, then try an input
                    var isVolumeLevel = false;
                    try
                    {
                        var parsedValue = int.Parse(response);
                        isVolumeLevel = true;
                    }
                    catch { }

                    if (isVolumeLevel)
                    {
                        validatedData.Data = response;
                        validatedData.CommandGroup = responseCommandGroup;
                        validatedData.CustomCommandGroup = ZoneVolumeCommandGroup;
                        validatedData.Ready = true;
                    }
                    else
                    {
                        // Treat everything as video input feedback.
                        // Override on DeConstruct will handle differentiating between the two
                        var videoInputs = DataValidation.InputFeedback.Feedback;
                        var audioInputs = DataValidation.AudioInputFeedback.Feedback;
                        var matchedToInput = false;

                        for (int i = 0; i < videoInputs.Count; i++)
                        {
                            if (response.Contains(videoInputs[i].Feedback))
                            {
                                matchedToInput = true;
                                break;
                            }
                        }
                        if (!matchedToInput)
                        {
                            for (int i = 0; i < audioInputs.Count; i++)
                            {
                                if (response.Contains(audioInputs[i].Feedback))
                                {
                                    matchedToInput = true;
                                    break;
                                }
                            }
                        }

                        if (matchedToInput)
                        {
                            validatedData.Data = response.Replace(DataValidation.InputFeedback.GroupHeader, string.Empty);
                            validatedData.CommandGroup = responseCommandGroup;
                            validatedData.CustomCommandGroup = ZoneInputCommandGroup;
                            validatedData.Ready = true;
                        }
                        else
                        {
                            // Ignore everything else
                            validatedData.Ignore = true;
                        }
                    }
                }
            }
            return validatedData;
        }
    }
}