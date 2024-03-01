//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by CrestronConstruct.
//     Version: 1.3001.21.0
//
//     Project:     MSSXpanel
//     Version:     1.0.0.0
//     Sdk:         CH5:2.7.0
//     Strategy:    Modern
//     IndexOnly:   True
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;

namespace MSSXpanel
{
    public class UIEventArgs : EventArgs
    {
        public BasicTriListWithSmartObject Device { get; private set; }
        public SigEventArgs SigArgs { get; private set; }

        public UIEventArgs(SmartObjectEventArgs eventArgs)
        {
            Device = (BasicTriListWithSmartObject) eventArgs.SmartObjectArgs.Device;
            SigArgs = eventArgs;
        }

        public static UIEventArgs CreateEventArgs(SmartObjectEventArgs eventArgs)
        {
            return new UIEventArgs(eventArgs);
        }
    }

    /// <summary>
    /// Data structure to carry return values because 2008 does not support Tuples.
    /// </summary>
    public struct Indexes
    {
        /// <summary>
        /// Item index in Widget List.
        /// </summary>
        public ushort ItemIndex;

        /// <summary>
        /// join index in List (Button List, Tab, Dpad, keypad).
        /// </summary>
        public ushort JoinIndex;

        public bool IsError;

        public Indexes(ushort itemIndex, ushort joinIndex, bool isError)
        {
            ItemIndex = itemIndex;
            JoinIndex = joinIndex;
            IsError = isError;
        }
    }
}

