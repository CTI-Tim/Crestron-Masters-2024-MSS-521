using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.RAD.DeviceTypes.Display;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.Common.BasicDriver;
using Crestron.SimplSharp;
using Independentsoft.Exchange;

namespace Display_IP_Driver
{
    public class DriverProtocol :ADisplayProtocol
    {
        public DriverProtocol(ISerialTransport transportDriver,byte id): base(transportDriver,id)
        {
            ResponseValidation = new ResponseValidator(ValidatedData);

            ValidatedData.PowerOnPollingSequence = new[]
            {
                StandardCommandsEnum.LampHoursPoll
            };
        }
    }

    public class ResponseValidator : ResponseValidation
    {
        public ResponseValidator(DataValidation dataValidation) : base(dataValidation)
        {

        }

        public override ValidatedRxData ValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            ValidatedRxData validatedData = new ValidatedRxData(false, string.Empty);

            CrestronConsole.PrintLine("@@@@@[DRIVER] ValidateResponse. Response = {0}, commandGroup = {1}",
                response.ToString(), commandGroup.ToString());

            validatedData.CommandGroup = commandGroup;
            validatedData.Data = response;
            validatedData.Ready = true;
            return validatedData;
        }
    }
}
