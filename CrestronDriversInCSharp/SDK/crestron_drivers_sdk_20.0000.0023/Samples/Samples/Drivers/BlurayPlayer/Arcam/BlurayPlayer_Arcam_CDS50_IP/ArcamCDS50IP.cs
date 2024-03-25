using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.SimplSharp;
using Crestron.RAD.DeviceTypes.BlurayPlayer;

namespace Crestron.RAD.Drivers.BlurayPlayers
{
    public class ArcamCDS50IP : ABasicBlurayPlayer, ITcp
    {
        public ArcamCDS50IP()
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
                EnableTxDebug = InternalEnableTxDebug
            };

            tcpTransport.Initialize(ipAddress, port);
            ConnectionTransport = tcpTransport;

            BlurayPlayerProtocol = new ArcamCDS50Protocol(ConnectionTransport, Id)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            BlurayPlayerProtocol.StateChange += StateChange;
            BlurayPlayerProtocol.RxOut += SendRxOut;
            BlurayPlayerProtocol.Initialize(BlurayPlayerData);            
        }

        public override bool SupportsDisconnect { get { return true; } }
        public override bool SupportsReconnect { get { return true; } }
    }
}
