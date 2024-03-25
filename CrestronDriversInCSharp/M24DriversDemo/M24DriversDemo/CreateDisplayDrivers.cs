
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Interfaces;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro.UI;
using System;

namespace M24DriversDemo
{
    public static class CreateDisplayDrivers
    {
        public static void LoadDrivers(string fileName, string ip)
        {
            // If there is a previosly loaded driver dispose of old driver before new one is created
            if (Global.Display != null)
            {
                Global.Display.Dispose();
            }
            
            Global.Display = GetDriverAssembly.getAssembly<IBasicVideoDisplay>(Path.Combine(Global.GetDriverPath(), fileName), "IBasicVideoDisplay", "ITcp"); //Call the assembly load. Reflects the dll into the object.
            try
            {
                if (Global.Display != null)
                {
                    DriverEmulation.Disconnect(); //Just for test feedback
                    DriverEmulation.Initialize(((ITcp)Global.Display).Port); //Just for test feedback

                    ((ITcp)Global.Display).Initialize(IPAddress.Parse(ip), ((ITcp)Global.Display).Port); //Prepare Driver for communication

                    Global.Display.StateChangeEvent += MyDisplay_StateChangeEvent; //Subscribe to change event
                    Global.Display.EnableTxDebug = true;

                    //Put data about now active driver on the UI
                    Global.M24DemoUI.SetJoin((ushort)JoinNumbers.Serial.ActiveDriverManufacturer, Global.Display.Manufacturer);
                    Global.M24DemoUI.SetJoin((ushort)JoinNumbers.Serial.ActiveDriverModel, Global.Display.BaseModel);
                    Global.M24DemoUI.SetJoin((ushort)JoinNumbers.Serial.DriverPort, ((ITcp)Global.Display).Port.ToString());
                    Global.M24DemoUI.SetJoin((ushort)JoinNumbers.Serial.ActiveDriverIpAddress, ip);
                        
                    var inputs = Global.Display.GetUsableInputs(); //Get the inputs that the display supports
                    foreach (var item in inputs)
                    {
                        Global.Input.Add(item.InputType);
                    }

                    Global.Display.Connect(); //Connect Driver to IP
                }
            }
            catch (Exception e)
            {
                Console.SendLine("Error in LoadDrivers :" + e.ToString());
            }
        }

        #region Driver Events
        private static void MyDriver_ConnectedChanged(object sender, ValueEventArgs<bool> e)
        {
            Console.SendLine("Driver Connected");
        }

        /// <summary>
        /// Feedback from Driver to display on UI
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private static void MyDisplay_StateChangeEvent(Crestron.RAD.Common.Enums.DisplayStateObjects arg1, IBasicVideoDisplay arg2, byte arg3)
        {
            switch (arg1)
            {
                case DisplayStateObjects.Power:
                    Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.PowerOn, false);
                    Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.PowerOff, false);
                    if (arg2.PowerIsOn)
                    {
                        Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.PowerOn, true);
                    }
                    else
                    {
                        Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.PowerOff, true);
                    }
                    break;
                case DisplayStateObjects.Input:
                    string inputIs = arg2.InputSource.Description.ToUpper();
                    Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.HDMI1, inputIs == "HDMI 1");
                    Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.HDMI2, inputIs == "HDMI 2");

                    break;
                case DisplayStateObjects.Connection:
                    Global.M24DemoUI.SetJoin((uint)JoinNumbers.Digital.Connected, arg2.Connected);

                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}

