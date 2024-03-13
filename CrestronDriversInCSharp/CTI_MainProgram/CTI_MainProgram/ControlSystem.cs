using Crestron.SimplSharp;                              // For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharpPro.UI;
using CTI_MainProgram.CWS;
using System;

namespace CTI_MainProgram
{
    public class ControlSystem : CrestronControlSystem
    {

        private static XpanelForSmartGraphics mypanel;
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);


                // Add console command to load drivers on a hardware platform
                CrestronConsole.AddNewConsoleCommand(LoadDriver,"LoadDriver", "Loads New Display Driver and IP.   DriverName:IPaddress", ConsoleAccessLevelEnum.AccessOperator);
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
                mypanel = new XpanelForSmartGraphics(0x04, this);
                mypanel.SigChange += Mypanel_SigChange;

                if (mypanel.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Panel failed to register = {0}", mypanel.RegistrationFailureReason);
                }
                else
                {
                    Global.xPanel = mypanel;
                }

                // Determine if this is running on a server or hardwrae device   ie:  VC4 or 4Series Processor
                Global.GetPlatform(); 
                CWSManager.InitializeCWSServer();
                
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        private void LoadDriver(string s)
        {
            string[] values = s.Split(':');
            string driver = values[0].Trim();
            string ipAddress = values[1].Trim();
            CreateDisplayDrivers.LoadDrivers(driver,ipAddress);
        }
           
       
        enum eProjoCMDs // Assistance for creating switch-case
        {
            PowerON = 1,
            PowerOFF,
            HDMI1,
            HDMI2,
            Connected
        }
        private void Mypanel_SigChange(BasicTriList currentDevice, SigEventArgs args)
        {
            if (args.Sig.Type == eSigType.Bool && args.Sig.BoolValue == true)
            {
                switch ((eProjoCMDs)args.Sig.Number)
                {
                    case eProjoCMDs.PowerON:
                        Global.Display.PowerOn();
                        break;
                    case eProjoCMDs.PowerOFF:
                        Global.Display.PowerOff();
                        break;
                    case eProjoCMDs.HDMI1:
                        Global.Display.SetInputSource(Global.Input[0]);
                        break;
                    case eProjoCMDs.HDMI2:
                        Global.Display.SetInputSource(Global.Input[1]);
                        break;
                    default:
                        break;
                }
            }
        }




        #region Crestron Methods

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }

        #endregion
    }
}