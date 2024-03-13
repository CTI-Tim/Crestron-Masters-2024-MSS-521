using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Interfaces;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using System;

namespace CTI_MainProgram
{
    public static class CreateDisplayDrivers
    {
        public static void LoadDrivers(string s , string ip)
        {
            // If there is a previosly loaded driver dispose of old driver before new one is created
            if (Global.Display != null)   
            {              
                Global.Display.Dispose();
            }


            // Determine if driver is a .dll or a .pkg file
            if (s.EndsWith(".dll"))
            {
                Global.Display = GetDriverAssembly.getAssembly<IBasicVideoDisplay>(Path.Combine(Global.GetDriverPath(), s), "IBasicVideoDisplay", "ITcp", ".dll");
            }
            else if (s.EndsWith(".pkg"))
            {
                Global.Display = GetDriverAssembly.getAssembly<IBasicVideoDisplay>(Path.Combine(Global.GetDriverPath(), s), "IBasicVideoDisplay", "ITcp", ".pkg");
            }

            try
            {
                if (Global.Display != null)
                {
                    ((ITcp)Global.Display).Initialize(IPAddress.Parse(ip), ((ITcp)Global.Display).Port);
                    Global.Display.StateChangeEvent += MyDisplay_StateChangeEvent;
                    Global.Display.EnableTxDebug = true;
    
                    Global.Display.Id = 1;

                    CrestronConsole.PrintLine($" Manufacturer:{Global.Display.Manufacturer} Description:{Global.Display.Description}");
                    CrestronConsole.PrintLine($" Version:{Global.Display.DriverVersion} Created on:{Global.Display.VersionDate}");

                    Global.Display.Connect();
                    var inputs = Global.Display.GetUsableInputs();
                    foreach (var item in inputs)
                    {
                        Global.Input.Add(item.InputType);
                    }
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine(e.ToString());
            }
        }

        #region Driver Events
        private static void MyDriver_ConnectedChanged(object sender, ValueEventArgs<bool> e)
        {
            CrestronConsole.PrintLine("#####[PROGRAM] ConnectionChanged event = {0}", e.Value);

            Global.xPanel.UShortInput[1].UShortValue = e.Value == true ? (ushort)1 : (ushort)0;
        }
        private static void MyDisplay_StateChangeEvent(Crestron.RAD.Common.Enums.DisplayStateObjects arg1, IBasicVideoDisplay arg2, byte arg3)
        {
            switch (arg1)
            {
                case DisplayStateObjects.Power:
                   Global.xPanel.BooleanInput[1].BoolValue = arg2.PowerIsOn;
                    break;
                case DisplayStateObjects.Input:
                    string inputIs = arg2.InputSource.Description.ToUpper();
                    Global.xPanel.BooleanInput[3].BoolValue = inputIs == "HDMI 1";
                    Global.xPanel.BooleanInput[4].BoolValue = inputIs == "HDMI 2";
                    break;
                case DisplayStateObjects.Connection:
                    if (arg2.Connected)
                    {
                        Global.xPanel.UShortInput[1].UShortValue = 1;
                    }
                    else
                    {   
                        Global.xPanel.UShortInput[1].UShortValue = 0;
                    }

                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
