using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Masters_2024_MSS_521.MessageSystem;
using Masters_2024_MSS_521.UserInterface;


namespace Masters_2024_MSS_521
{
    public class ControlSystem : CrestronControlSystem
    {
        public Automation SetupAutomation;

        public DeviceSetup SetupHardware;
        public EventTimers SetupTimers;
        public Xpanel SetupUserInterface;

        public ControlSystem()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
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
                // debugging commands for the broker system these are optional and only work on appliance targets
                CrestronConsole.AddNewConsoleCommand(MessageBroker.ListKeys, "brokerlist",
                    "MSS-521 program List the Broker subscriptions", ConsoleAccessLevelEnum.AccessOperator);
                CrestronConsole.AddNewConsoleCommand(MessageBroker.MonitorTraffic, "brokermon",
                    "MSS-521 program Monitor traffic send on to turn on anything else for off",
                    ConsoleAccessLevelEnum.AccessOperator);

                // we call our custom classes for setting things up all the work is done in these
                // be 100% sure each returns as fast as possible. you have 20 seconds here total to get it done.
                SetupHardware = new DeviceSetup(this);
                SetupUserInterface = new Xpanel(0x04, this);
                SetupAutomation = new Automation(); //This is where your system steppers and main program code lives
                SetupTimers = new EventTimers(); // Hardcoded timers such as automatic shutoff


                // Populate the NVX global addresses.   This would be handy to have populated from a file that was read on startup..
                // Maybe add a configuration class that does this?
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.128");
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.128");
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.128");
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.128");
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }
    }
}