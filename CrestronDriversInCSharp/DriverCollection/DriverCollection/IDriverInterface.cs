using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCollection
{
    public interface IDriverInterface :IDisposable
    {
        void PowerOn();
        void PowerOff();

        void Initialize(string ip,int port);
    }
}
