using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.RAD.Common.Attributes.Programming;         	// For Generic Device Support


namespace M24DriversDemo
{
    public class ControlSystem : CrestronControlSystem
    {
        
        DriverBrowser DriverSelectionUI;        

        public ControlSystem() : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
                CrestronConsole.AddNewConsoleCommand(LoadDriver, "LoadDriver", "Loads New Display Driver and IP.   DriverName:IPaddress", ConsoleAccessLevelEnum.AccessOperator);
                //Subscribe to the controller events (System, Program, and Ethernet)                
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }
        public override void InitializeSystem()
        {
            
            try
            {
                //Jump right into a new thread 
                Thread t = new Thread(Start, null);

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }


        }

        public ThreadCallbackFunction Start(object o)
        {
            SetupUI();
            DriverSelectionUI = new DriverBrowser();
            Console.Initialize(8005); //Create a console on port 8005
            CWSManager.InitializeCWSServer(); ;
            DriverSelectionUI.RefreshList(DriverBrowser.DriverType.All);                      
            return null;
        }


        public void SetupUI()
        {
            //Add smart graphcs
            Global.M24DemoUI = new UserInterface(03, this, "M24DriverDemo.sgd");
            Global.M24DemoUI.AddSmartObject(1, "DriverList");

            //Setup sort joins
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.SortAll ,UserInterface.JoinType.Digital , new Action<bool>(_press => { if (_press) { DriverSelectionUI.RefreshList(DriverBrowser.DriverType.All); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.SortDisplay, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { DriverSelectionUI.RefreshList(DriverBrowser.DriverType.Display); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.SortReceiver, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { DriverSelectionUI.RefreshList(DriverBrowser.DriverType.Receiver); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.SortSource, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { DriverSelectionUI.RefreshList(DriverBrowser.DriverType.Source); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.SetIPAddress, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { DriverSelectionUI.DriverIPAddress = Global.M24DemoUI.GetSerialJoin((uint)JoinNumbers.Serial.DriverIpAddress); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.PowerOn,UserInterface.JoinType.Digital,  new Action<bool>(_press => { if (_press) { Global.Display.PowerOn(); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.PowerOff, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { Global.Display.PowerOff(); } }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.HDMI1, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { Global.Display.SetInputSource(Global.Input[0]);} }));
            Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.HDMI1, UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { Global.Display.SetInputSource(Global.Input[1]); } }));

        }

        private void LoadDriver(string s)
        {
            string[] values = s.Split(':');
            string driver = values[0].Trim();
            string ipAddress = values[1].Trim();
            CreateDisplayDrivers.LoadDrivers(driver, ipAddress);
        }

    }
}