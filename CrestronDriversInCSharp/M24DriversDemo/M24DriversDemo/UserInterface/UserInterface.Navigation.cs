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
        Dictionary<string, ushort> _pages = new Dictionary<string, ushort>();
        Dictionary<string, ushort> _subPages = new Dictionary<string, ushort>();
        Dictionary<string, ushort> _popups = new Dictionary<string, ushort>();
        public string CurrentSubpage { get; private set; }
        public string CurrentPage { get; private set; }
        public string CurrentPopup { get; private set; }
        public string LastSubpage { get; private set; }
        public string LastPage { get; private set; }
        public string LastPopup { get; private set; }

        public void AddPage(ushort join, string name)
        {
            _pages.Add(name, join);
        }

        public void AddSubPage(ushort join, string name)
        {
            _subPages.Add(name, join);
        }

        public void AddPopup(ushort join, string name)
        {
            _popups.Add(name, join);
        }

        public void CallPage(string name)
        {
            ushort _join;
            if (_pages.TryGetValue(name, out _join))
            {
                _device.BooleanInput[_join].Pulse();
                LastPage = CurrentPage;
                CurrentPage = name;
            }
            else
            {
                _debug("Page not found: " + name);
            }
        }

        public void CallSubPage(string name)
        {
            if (_subPages.ContainsKey(name))
            {
                ushort _join;
                ClearSubpages();
                _subPages.TryGetValue(name, out _join);
                _device.BooleanInput[_join].BoolValue = true;
                LastSubpage = CurrentSubpage;
                CurrentSubpage = name;
            }
            else
            {
                _debug("SubPage not found: " + name);
            }
        }

        public void ClearSubpages()
        {
            foreach (KeyValuePair<string, ushort> _sP in _subPages)
            {
                _device.BooleanInput[_sP.Value].BoolValue = false;
            }
            CurrentSubpage = "";
        }

        public void CallPopup(string name)
        {
            if (_popups.ContainsKey(name))
            {
                ClearPopups();
                ushort _join;
                _popups.TryGetValue(name, out _join);
                _device.BooleanInput[_join].BoolValue = true; ;
                LastPopup = CurrentPopup;
                CurrentPopup = name;
            }
            else
            {
                _debug("Popup not found: " + name);
            }
        }

        public void ClearPopups()
        {
            foreach (KeyValuePair<string, ushort> _p in _popups)
            {
                _device.BooleanInput[_p.Value].BoolValue = false;
            }
            CurrentPopup = "";
        }

    }
}

