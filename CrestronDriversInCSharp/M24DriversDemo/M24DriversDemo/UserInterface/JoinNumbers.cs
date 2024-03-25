using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M24DriversDemo
{
    /// <summary>
    /// Join jumbers for UI
    /// </summary>
    public class JoinNumbers
    {
        public enum Digital
        {
            SortAll = 1,
            SortDisplay = 2,
            SortReceiver = 3,
            SortSource = 4,
            LoadDriver = 5,
            PowerOn = 6,
            PowerOff = 7,
            SetIPAddress = 8,
            HDMI1 = 9,
            HDMI2 = 10,
            Connected = 11,

        }

        public enum Analog
        {

        }

        public enum Serial
        {
            SelectedDeviceType = 1,
            SelectedManufacture = 2,
            SelectedBaseModel = 3,
            DriverIpAddress = 10,
            DriverPort = 8,
            ActiveDriverManufacturer = 5,
            ActiveDriverModel = 6,
            ActiveDriverIpAddress = 7,
            ErrorMessage = 20,
        }

    }
}