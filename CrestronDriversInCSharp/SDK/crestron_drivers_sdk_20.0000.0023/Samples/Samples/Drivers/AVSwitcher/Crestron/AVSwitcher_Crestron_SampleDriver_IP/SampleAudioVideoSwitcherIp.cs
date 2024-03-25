using System;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;

namespace AudioVideoSwitcher_Crestron_SampleDriverModel_IP
{
    public class SampleAudioVideoSwitcherIp : AAudioVideoSwitcher, ITcp
    {
        public void Initialize(IPAddress ipAddress, int port)
        {
            var tcpTransport = new TcpTransport
            {
                EnableAutoReconnect = EnableAutoReconnect,
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger,
                EnableRxDebug = InternalEnableRxDebug,
                EnableTxDebug = InternalEnableTxDebug
            };

            tcpTransport.Initialize(ipAddress, port);
            ConnectionTransport = tcpTransport;

            AudioVideoSwitcherProtocol = new SampleAudioVideoSwitcherProtocol(ConnectionTransport, Id)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            AudioVideoSwitcherProtocol.RxOut += SendRxOut;
            AudioVideoSwitcherProtocol.Initialize(AudioVideoSwitcherData);
        }
    }
}
