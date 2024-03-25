using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.BlurayPlayer;

namespace Crestron.RAD.Drivers.BlurayPlayers
{
    public class ArcamCDS50ResponseValidation : ResponseValidation
    {
        ArcamCDS50Protocol _protocol;
        private int _elapsedTimeFeedbackCount = 1;
        private const int _maxElapsedTimeFeedbackIgnoreCount = 5;

        public ArcamCDS50ResponseValidation(byte id, DataValidation dataValidation, ArcamCDS50Protocol protocol)
            : base(id, dataValidation)
        {
            Id = id;
            DataValidation = dataValidation;
            _protocol = protocol;
        }

        public override ValidatedRxData ValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            ValidatedRxData validatedData = new ValidatedRxData(false, string.Empty);

            try
            {
                if (response.Length > 3)
                {
                    byte[] responseBytes = Encoding.GetBytes(response);
                    
                    byte dataLengthByte = responseBytes[4];
                    int dataLength = Convert.ToInt16(dataLengthByte);
                    int carriageReturnPosition = 4 + dataLength + 1;

                    if (responseBytes != null && responseBytes[carriageReturnPosition] == 13)
                    {
                        byte[] headerBytes = new byte[2];

                        Array.Copy(responseBytes, 0, headerBytes, 0, 2);

                        if (headerBytes[0] == 33 && headerBytes[1] == 1)
                        {
                            byte answerCode = responseBytes[3];

                            if (answerCode == 131)
                            {
                                if (_protocol.EnableLogging)
                                {
                                    _protocol.LogMessage("Command not recognized .");
                                }
                            }
                            else if (answerCode == 0)
                            {
                                byte commandType = responseBytes[2];
                                if (Convert.ToInt16(commandType) == Convert.ToInt16(DataValidation.Feedback.PowerFeedback.GroupHeader))
                                {
                                    validatedData = ProcessPowerFeedback(responseBytes);
                                }
                                else if (Convert.ToInt16(commandType) == Convert.ToInt16(DataValidation.Feedback.TrackElapsedTimeFeedback.GroupHeader))
                                {
                                    // We do not poll for this, so ignore response & process it manually
                                    _elapsedTimeFeedbackCount++;
                                    if (_elapsedTimeFeedbackCount % _maxElapsedTimeFeedbackIgnoreCount == 0)
                                    {
                                        _protocol.DeconstructFeedback(ProcessTrackElapsedTimeFeedback(response));
                                    }
                                    validatedData.Ignore = true;
                                }
                                else if (Convert.ToInt16(commandType) == 45)
                                {
                                    validatedData = ProcessTrackFeedback(response);
                                }
                                else if (Convert.ToInt16(commandType) == Convert.ToInt16(DataValidation.Feedback.PlayBackStatusFeedback.GroupHeader))
                                {
                                    validatedData = ProcessPlaybackStatusFeedback(response);
                                }
                                else if (Convert.ToInt16(commandType) == 44)
                                {
                                    validatedData = ProcessInputFeedback(responseBytes);
                                }
                                else if (Convert.ToInt16(commandType) == 8)
                                {
                                    validatedData = ProcessInputSubscriptionFeedback(responseBytes);
                                }
                                else
                                {
                                    validatedData.Ignore = true;
                                    if (_protocol.EnableLogging)
                                    {
                                        _protocol.LogMessage(string.Format("Did not process message {0} with command group {1}",
                                        BitConverter.ToString(responseBytes), commandGroup.ToString()));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        validatedData.Ignore = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_protocol.EnableLogging)
                {
                    _protocol.LogMessage(string.Format("ArcamCDS50.ValidateResponse error:{0}", ex.Message));
                }
            }

            return validatedData;
        }

        internal ValidatedRxData ProcessInputFeedback(byte[] responseBytes)
        {
            var validatedData = new ValidatedRxData(false, string.Empty);

            switch (responseBytes[5])
            {
                case 2:
                    {
                        _protocol.SetDriverInitializedStatus(true);
                        break;
                    }
                default:
                    {
                        _protocol.SetDriverInitializedStatus(false);
                        break;
                    }
            }

            validatedData.Ignore = true;

            return validatedData;
        }

        internal ValidatedRxData ProcessInputSubscriptionFeedback(byte[] responseBytes)
        {
            var validatedData = new ValidatedRxData(false, string.Empty);

            if (responseBytes[5] == 20 && responseBytes[6] == 90)
            {
                //CD Input
                _protocol.SetDriverInitializedStatus(true);
            }
            else
            {
                //Not CD Input
                _protocol.SetDriverInitializedStatus(false);
            }

            validatedData.Ignore = true;

            return validatedData;
        }

        internal ValidatedRxData ProcessPowerFeedback(byte[] responseBytes)
        {
            var validatedData = new ValidatedRxData(false, string.Empty);

            switch (responseBytes[5])
            {
                case 0:
                    {
                        validatedData.Data = "standby";
                        break;
                    }
                case 1:
                    {
                        validatedData.Data = "on";
                        break;
                    }
            }

            validatedData.CommandGroup = CommonCommandGroupType.Power;
            validatedData.Ready = true;

            return validatedData;
        }

        internal ValidatedRxData ProcessTrackElapsedTimeFeedback(string response)
        {
            var validatedData = new ValidatedRxData(false, string.Empty);

            byte[] timeBytes = Encoding.GetBytes(response.Substring(5, 3));

            string hours = timeBytes[0].ToString().PadLeft(2, '0');
            string minutes = timeBytes[1].ToString().PadLeft(2, '0');
            string seconds = timeBytes[2].ToString().PadLeft(2, '0');

            string time = string.Format("{0}:{1}:{2}", hours, minutes, seconds);

            validatedData.Data = time;
            validatedData.CommandGroup = CommonCommandGroupType.TrackElapsedTime;
            validatedData.Ready = true;

            return validatedData;
        }

        internal ValidatedRxData ProcessTrackFeedback(string response)
        {
            var validatedData = new ValidatedRxData(false, string.Empty);

            byte[] chanpterBytes = Encoding.GetBytes(response.Substring(5, 1));

            validatedData.Data = chanpterBytes[0].ToString();
            validatedData.CommandGroup = CommonCommandGroupType.TrackFeedback;
            validatedData.Ready = true;

            return validatedData;
        }

        internal ValidatedRxData ProcessPlaybackStatusFeedback(string response)
        {
            var validatedData = new ValidatedRxData(false, string.Empty);

            byte[] playbackStatebytes = Encoding.GetBytes(response.Substring(6, 1));
            byte[] trayStatusBytes = Encoding.GetBytes(response.Substring(5, 1));
            byte[] scanningDirectionBytes = Encoding.GetBytes(response.Substring(7, 1));

            switch (trayStatusBytes[0])
            {
                case 0:
                    {
                        validatedData.Data = "OPEN";
                        break;
                    }
                case 1:
                    {
                        switch (playbackStatebytes[0])
                        {
                            case 0:
                            case 10:
                                {
                                    validatedData.Data = "STOP";
                                    break;
                                }
                            case 1:
                                {
                                    validatedData.Data = "PLAY";
                                    break;
                                }
                            case 2:
                                {
                                    validatedData.Data = "PAUSE";
                                    break;
                                }
                            case 3:
                                {
                                    validatedData.Data = "STOP";
                                    break;
                                }
                            case 4:
                                {
                                    switch (scanningDirectionBytes[0])
                                    {
                                        case 129:
                                            {
                                                validatedData.Data = "FREV";
                                                break;
                                            }
                                        case 1:
                                            {
                                                validatedData.Data = "FFWD";
                                                break;
                                            }
                                        default:
                                            {
                                                validatedData.Data = "UNKNOWN";
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    validatedData.Data = "Coaxial SPDIF";
                                    break;
                                }
                            case 6:
                                {
                                    validatedData.Data = "Optical SPDIF";
                                    break;
                                }
                            //case 10:
                            default:
                                {
                                    validatedData.Data = "UNKNOWN";
                                    break;
                                }
                        }
                        break;
                    }
            }

            validatedData.CommandGroup = CommonCommandGroupType.PlayBackStatus;
            validatedData.Ready = true;

            return validatedData;
        }
    }
}