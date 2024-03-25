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
        Dictionary<string, SmartObject> _smartObjects;
        void _registerSmartObjects()
        {
            if (File.Exists(_sgdFile)) //Check for SGD 
            {
                // Load the SGD file 
                _device.LoadSmartObjects(_sgdFile);
                _smartObjects = new Dictionary<string, SmartObject>();
                _debug("SGD Loaded");
            }
            else
            {
                _debug("SGD File not found: " + _sgdFile);
            }
        }

        public void AddSmartObject(uint id, string name)
        {
            _device.SmartObjects[id].SigChange += _SOSigChange;
            _smartObjects.Add(name, _device.SmartObjects[id]);
            _debug("SO Added");
        }


        void _SOSigChange(GenericBase device, SmartObjectEventArgs args)
        {
            if (args.Sig.UserObject is Action<bool>)
            {
                (args.Sig.UserObject as Action<bool>)(args.Sig.BoolValue);
            }
            else if (args.Sig.UserObject is Action<ushort>)
            {
                (args.Sig.UserObject as Action<ushort>)(args.Sig.UShortValue);
            }
            else if (args.Sig.UserObject is Action<string>)
            {
                (args.Sig.UserObject as Action<string>)(args.Sig.StringValue);
            }
        }

        public void SetJoin(string smartObjectName, string sigName, bool state)
        {
            _smartObjects[smartObjectName].BooleanInput[sigName].BoolValue = state;
        }

        public void SetJoin(string smartObjectName, string sigName, ushort value)
        {
            _smartObjects[smartObjectName].UShortInput[sigName].UShortValue = value;
        }

        public void SetJoin(string smartObjectName, string sigName, string data)
        {
            _smartObjects[smartObjectName].StringInput[sigName].StringValue = data;
        }

        public void SetOutputObject(string smartObjectName, string sigName, JoinType jointype, object obj)
        {
            switch (jointype)
            {
                case JoinType.Analog:
                    {
                        _smartObjects[smartObjectName].UShortOutput[sigName].UserObject = obj;
                        break;
                    }
                case JoinType.Digital:
                    {
                        _smartObjects[smartObjectName].BooleanOutput[sigName].UserObject = obj;
                        break;
                    }
                case JoinType.Serial:
                    {
                        _smartObjects[smartObjectName].StringOutput[sigName].UserObject = obj;
                        break;
                    }
            }
        }

        public object ReturnObject(string smartObjectName, string sigName, JoinType jointype)
        {
            switch (jointype)
            {
                case JoinType.Digital:
                    {
                        return _smartObjects[smartObjectName].BooleanInput[sigName].UserObject;
                    }
                case JoinType.Analog:
                    {
                        return _smartObjects[smartObjectName].UShortInput[sigName].UserObject;
                    }
                case JoinType.Serial:
                    {
                        return _smartObjects[smartObjectName].StringOutput[sigName].UserObject;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public void SubscribeFeedback(string smartObejctName, string sigName, ref Action<ushort> action)
        {
            _smartObjects[smartObejctName].UShortInput[sigName].UserObject = new Action<ushort>(_value => { _smartObjects[smartObejctName].UShortInput[sigName].UShortValue = _value; });
            action += _smartObjects[smartObejctName].UShortInput[sigName].UserObject as Action<ushort>;
        }
        public void SubscribeFeedback(string smartObjectName, string sigName, ref Action<bool> action)
        {
            _smartObjects[smartObjectName].BooleanInput[sigName].UserObject = new Action<bool>(_state => { _smartObjects[smartObjectName].BooleanInput[sigName].BoolValue = _state; });
            action += _smartObjects[smartObjectName].BooleanInput[sigName].UserObject as Action<bool>;
        }

        public void SubscribeFeedback(string smartObjectName, string sigName, ref Action<string> action)
        {
            _smartObjects[smartObjectName].StringInput[sigName].UserObject = new Action<string>(_data => { _smartObjects[smartObjectName].StringInput[sigName].StringValue = _data; });
            action += _smartObjects[smartObjectName].StringInput[sigName].UserObject as Action<string>;
        }
        public void UnsubscribeFeedback(string smartObjectName, string sigName, ref Action<ushort> action)
        {
            action -= _smartObjects[smartObjectName].UShortInput[sigName].UserObject as Action<ushort>;
        }
        public void UnsubscribeFeedback(string smartObjectName, string sigName, ref Action<bool> action)
        {
            action -= _smartObjects[smartObjectName].BooleanInput[sigName].UserObject as Action<bool>;
        }

        public void UnsubscribeFeedback(string smartObjectName, string sigName, ref Action<string> action)
        {
            action -= _smartObjects[smartObjectName].StringInput[sigName].UserObject as Action<string>;
        }
    }
}
