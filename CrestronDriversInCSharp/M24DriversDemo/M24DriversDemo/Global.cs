
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro.UI;
using System;
using System.Collections.Generic;

namespace M24DriversDemo
{
    /// <summary>
    /// Global logic including definition of UI object, Driver Object, and Platform check logic. 
    /// </summary>
    internal static class Global
    {
        public static UserInterface M24DemoUI;
        public static IBasicVideoDisplay Display { get; set; }
        public static List<VideoConnections> Input = new List<VideoConnections>();
        public static bool isHardware { get; set; }

        public static void GetPlatform()
        {

            try
            {
                if (CrestronEnvironment.DevicePlatform != eDevicePlatform.Server)
                {
                    isHardware = true;
                }
                else
                {
                    isHardware = false;
                }
            }
            catch (Exception)
            {


            }
        }

        public static string GetDriverPath()
        {
            if (Global.isHardware)
            {
                return @"\user\Drivers\";
            }
            return Path.Combine(Directory.GetApplicationRootDirectory(), "USER/Drivers/");
        }

        static void LoadDriver(string s)
        {
            string[] values = s.Split(':');
            string driver = values[0].Trim();
            string ipAddress = values[1].Trim();
            CreateDisplayDrivers.LoadDrivers(driver, ipAddress);
        }

    }
}

