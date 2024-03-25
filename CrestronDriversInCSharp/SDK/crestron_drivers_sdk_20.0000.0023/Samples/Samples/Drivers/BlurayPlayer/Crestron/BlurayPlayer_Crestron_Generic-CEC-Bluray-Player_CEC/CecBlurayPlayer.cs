// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="CecDisplay.cs">
//   
// </copyright>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace Crestron.RAD.Drivers.BlurayPlayers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Crestron.RAD.Common;
    using Crestron.RAD.Common.BasicDriver;
    using Crestron.RAD.Common.Enums;
    using Crestron.RAD.Common.Interfaces;
    using Crestron.RAD.Common.Transports;
    using Crestron.RAD.DeviceTypes.BlurayPlayer;
    using Crestron.SimplSharp;
    using Crestron.RAD.Common.ExtensionMethods;

    using Newtonsoft.Json;

    public class CecBlurayPlayer : ABasicBlurayPlayer, ICecDevice
    {
        public CecBlurayPlayer()
        {
        }

        public void Initialize(ISerialTransport transport)
        {
            ConnectionTransport = transport;

            BlurayPlayerProtocol = new CecBlurayPlayerProtocol(transport)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            BlurayPlayerProtocol.StateChange += StateChange;
            BlurayPlayerProtocol.RxOut += SendRxOut;
            BlurayPlayerProtocol.Initialize(BlurayPlayerData);
        }

        /*public override void Connect()
        {
            (BlurayPlayerProtocol as CecBlurayPlayerProtocol).Initialized = false;
            base.Connect();
        }*/


        public SimplTransport Initialize(int id, Action<string, object[]> send)
        {
            var simplTransport = new SimplTransport { Send = send };
            ConnectionTransport = simplTransport;

            BlurayPlayerProtocol = new CecBlurayPlayerProtocol(simplTransport)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            BlurayPlayerProtocol.StateChange += StateChange;
            BlurayPlayerProtocol.RxOut += SendRxOut;
            BlurayPlayerProtocol.Initialize(BlurayPlayerData);

            return simplTransport;
        }
    }
}
