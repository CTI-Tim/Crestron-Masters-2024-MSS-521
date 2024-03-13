using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CTI_IP_Driver;

namespace DriverCollection
{
    internal class IPDisplayDriver : IDriverInterface
    {

        CTI_IP_Driver_Transport transport;

        public void Dispose()
        {
           
        }

        public void Initialize(string ip, int port)
        {
           IPAddress devIP = IPAddress.Parse(ip);
           transport.Initialize(devIP,port);

        }

        public void PowerOff()
        {
            throw new NotImplementedException();
        }

        public void PowerOn()
        {
            throw new NotImplementedException();
        }
    }
}
