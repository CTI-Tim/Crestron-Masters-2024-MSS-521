using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.RAD.DeviceTypes.Display;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.Common.BasicDriver;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Display_IP_Driver
{
    public class DriverTransport : ABasicVideoDisplay,ITcp
    {
        public DriverTransport() 
        {
           
            
        }

        public void Initialize(IPAddress ipAddress, int port)
        {
            var tcpTransport = new TcpTransport
            {
                EnableAutoReconnect = EnableAutoReconnect,
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger,
                EnableRxDebug = InternalEnableRxDebug,
                EnableTxDebug = InternalEnableTxDebug,
                
            };

            tcpTransport.Initialize(ipAddress, port);
            ConnectionTransport = tcpTransport;     // Satisfies a component of ABasicDriver

            DisplayProtocol = new DriverProtocol(ConnectionTransport, Id)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
                
            };

            
            DisplayProtocol.StateChange += StateChange;
            // DisplayProtocol.RxOut += SendRxOut;
            DisplayProtocol.RxOut += DisplayProtocol_RxOut;
            DisplayProtocol.Initialize(DisplayData);
        }
        private void DisplayProtocol_RxOut(string message)
        {
            CrestronConsole.PrintLine("@@@@@[DRIVER] Message incoming from device = {0}", message);
            SendRxOut(message);
        }
    }
}
