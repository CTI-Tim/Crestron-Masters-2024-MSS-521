using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.BlurayPlayer;
using Crestron.RAD.Drivers.BlurayPlayers;

namespace Crestron.RAD.Drivers.BlurayPlayers
{
    public class CecBlurayPlayerResponseValidator : ResponseValidation
    {
        public CecBlurayPlayerResponseValidator(byte id, DataValidation dataValidation)
            : base(id, dataValidation)
        {
            Id = id;
            DataValidation = dataValidation;
        }

        public override ValidatedRxData ValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            ValidatedRxData validatedRxData = new ValidatedRxData(false, null);
            return validatedRxData;
        }
    }
}