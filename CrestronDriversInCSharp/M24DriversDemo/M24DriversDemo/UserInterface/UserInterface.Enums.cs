using System;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro.UI;
using System.Collections.Generic;

namespace M24DriversDemo
{
    public partial class UserInterface
    {
        public enum JoinType
        {
            Digital,
            Analog,
            Serial,
        }
    }
}
