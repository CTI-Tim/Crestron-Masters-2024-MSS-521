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
        void _SigChange(BasicTriList currentDevice, SigEventArgs args)
        {
            if (args.Sig.UserObject is Action<bool>)
            {
                try
                {
                    (args.Sig.UserObject as Action<bool>)(args.Sig.BoolValue);
                }
                catch (Exception e)
                {
                    ErrorLog.Error("Error in digital IPID:" + currentDevice.ID + " / Join " + args.Sig.Number + "\r Message: " + e.Message + "\r Stack Trace: " + e.StackTrace + "\r Inner Exception: " + e.InnerException);
                }
            }
            else if (args.Sig.UserObject is Action<ushort>)
            {
                try
                {
                    (args.Sig.UserObject as Action<ushort>)(args.Sig.UShortValue);
                }
                catch (Exception e)
                {
                    ErrorLog.Error("Error in analog IPID:" + currentDevice.ID + " / Join " + args.Sig.Number + "\r Message: " + e.Message + "\r Stack Trace: " + e.StackTrace + "\r Inner Exception: " + e.InnerException);
                }
            }
            else if (args.Sig.UserObject is Action<string>)
            {
                try
                {
                    (args.Sig.UserObject as Action<string>)(args.Sig.StringValue);
                }
                catch (Exception e)
                {
                    ErrorLog.Error("Error in serial IPID:" + currentDevice.ID + " / Join " + args.Sig.Number + "\r Message: " + e.Message + "\r Stack Trace: " + e.StackTrace + "\r Inner Exception: " + e.InnerException);
                }
            }
        }

        public void SetJoin(uint join, bool state)
        {
            _device.BooleanInput[join].BoolValue = state;
        }

        public void PulseJoin(uint join)
        {
            _device.BooleanInput[join].Pulse();
        }

        public void SetJoin(uint join, ushort value)
        {
            _device.UShortInput[join].UShortValue = value;
        }

        public void SetJoin(uint join, string data)
        {
            _device.StringInput[join].StringValue = data;
        }

        public string GetSerialJoin(uint join)
        {
            return _device.StringOutput[join].StringValue;
        }

        public ushort GetAnalogJoin(uint join)
        {
            return _device.UShortOutput[join].UShortValue;
        }

        public bool GetDigitalJoin(uint join)
        {
            return _device.BooleanOutput[join].BoolValue;
        }

        public void SetOutputObject(uint join, JoinType jointype, object obj)
        {
            switch (jointype)
            {
                case JoinType.Analog:
                    {
                        _device.UShortOutput[join].UserObject = obj;
                        break;
                    }
                case JoinType.Digital:
                    {
                        _device.BooleanOutput[join].UserObject = obj;
                        break;
                    }
                case JoinType.Serial:
                    {
                        _device.StringOutput[join].UserObject = obj;
                        break;
                    }
            }
        }

        public object ReturnOutputObject(uint join, JoinType jointype)
        {
            switch (jointype)
            {
                case JoinType.Analog:
                    {
                        return _device.UShortOutput[join].UserObject;
                    }
                case JoinType.Digital:
                    {
                        return _device.BooleanOutput[join].UserObject;
                    }
                case JoinType.Serial:
                    {
                        return _device.StringOutput[join].UserObject;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public void SubscribeFeedback(uint join, ref Action<ushort> action)
        {
            _device.UShortInput[join].UserObject = new Action<ushort>(_value => { _device.UShortInput[join].UShortValue = _value; });
            action += _device.UShortInput[join].UserObject as Action<ushort>;
        }
        public void SubscribeFeedback(uint join, ref Action<bool> action)
        {
            _device.BooleanInput[join].UserObject = new Action<bool>(_state => { _device.BooleanInput[join].BoolValue = _state; });
            action += _device.BooleanInput[join].UserObject as Action<bool>;
        }

        public void SubscribeFeedback(uint join, Action<string> action)
        {
            _device.StringInput[join].UserObject = new Action<string>(_data => { _device.StringInput[join].StringValue = _data; });
            action += _device.StringInput[join].UserObject as Action<string>;
        }
        public void UnsubscribeFeedback(uint join, Action<ushort> action)
        {
            action -= _device.UShortInput[join].UserObject as Action<ushort>;
        }
        public void UnsubscribeFeedback(uint join, Action<bool> action)
        {
            action -= _device.BooleanInput[join].UserObject as Action<bool>;
        }

        public void UnsubscribeFeedback(uint join, Action<string> action)
        {
            action -= _device.StringInput[join].UserObject as Action<string>;
        }
    }
}


