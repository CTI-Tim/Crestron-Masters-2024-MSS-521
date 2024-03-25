using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro.UI;
using System.Collections.Generic;

namespace M24DriversDemo
{
    public partial class UserInterface
    {

        public uint Ipid { private set; get; }

        string _sgdFile;
        BasicTriListWithSmartObject _device;

        public UserInterface(uint ipid, CrestronControlSystem controlSystem, string sgdFile)
        {
            _UserInterfaceConstructor(ipid, controlSystem, sgdFile, "");
        }
        public UserInterface(uint ipid, CrestronControlSystem controlSystem, string sgdFile, string projectName)
        {
            _UserInterfaceConstructor(ipid, controlSystem, sgdFile, projectName);
        }

        void _UserInterfaceConstructor(uint ipid, CrestronControlSystem controlSystem, string sgdFile, string projectName)
        {
            AddUIDevice(ipid, controlSystem, sgdFile, projectName);
        }

        public void AddUIDevice(uint ipid, CrestronControlSystem controlSystem, string sgdFile, string projectName)
        {
            ErrorLog.Notice("###### Adding UI Device #########");
            Ipid = ipid;
            _sgdFile = string.Format("{0}{1}UserInterface{1}SGD Files{1}{2}", Directory.GetApplicationDirectory(), Path.DirectorySeparatorChar.ToString(), sgdFile);
            if (controlSystem.SupportsEthernet)
            {
                try
                {
                    if (projectName.Length > 0)
                    {
                        _debug("###### Touch Panel Set to iPad #########");
                        CrestronApp _d = new CrestronApp(Ipid, controlSystem);
                        _d.ParameterProjectName.Value = projectName;
                        _device = _d as CrestronApp;

                    }
                    else
                    {
                        _debug("###### Touch Panel Set to XPANEL #########");
                        XpanelForSmartGraphics _d = new XpanelForSmartGraphics(Ipid, controlSystem);
                        _device = _d as XpanelForSmartGraphics;
                    }
                    _register();

                }
                catch (Exception e)
                {
                    _debug("Error with creation of the UI Device " + e.Message);
                }
            }
        }

        void _register()
        {
            if (_device.Register() == eDeviceRegistrationUnRegistrationResponse.Success) //If panel registers set all its defaults
            {

                _device.SigChange += _SigChange;

                if (_sgdFile.Length > 0 && _sgdFile != null)
                {
                    _smartObjects = new Dictionary<string, SmartObject>();
                    _registerSmartObjects();
                }

                _device.OnlineStatusChange += _OnlineStatusChange;
            }
            else
            {
                CrestronConsole.PrintLine("Control Surface Registration Error :" + Ipid);
                ErrorLog.Error("Control Surface was not registered: {0}", _device.RegistrationFailureReason);
            }
        }

        void PanelSetup()
        {
            //Any required panel setup goes here       
        }

        void _OnlineStatusChange(GenericBase device, OnlineOfflineEventArgs args)
        {
            if (args.DeviceOnLine == true)
            {
                ErrorLog.Notice("UI Conected " + device.ID);
            }
            if (args.DeviceOnLine == false)
            {
                ErrorLog.Notice("UI Disconnected " + device.ID);
            }
        }
    }
}

