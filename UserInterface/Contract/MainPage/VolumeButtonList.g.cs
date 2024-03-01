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
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;
using MSSXpanel;

namespace MSSXpanel.MainPage
{

    /// <summary>
    /// Allow click events by item
    /// </summary>
    public interface IVolumeButtonListByItem
    {
        /// <summary>
        /// Fires on button list presses.  Event carries <see="IndexedButtonEventArgs"/> with ButtonIndex property (0 based).
        /// </summary>
        event EventHandler<IndexedButtonEventArgs> Button_PressEvent;
    }


    /// <summary>
    /// Search List
    /// </summary>
    internal partial class VolumeButtonList
    {
        #region CH5 Contract

        public event EventHandler<IndexedButtonEventArgs> Button_PressEvent;
        private void onButton_Press(IndexedEventArgs eventArgs)
        {
            EventHandler<IndexedButtonEventArgs> handler = Button_PressEvent;
            if (handler != null)
                handler(this, new IndexedButtonEventArgs((SmartObjectEventArgs)eventArgs.SigArgs, eventArgs.JoinIndex));
        }
                

        #endregion
    }

    /// <summary>
    /// VolumeButtonList
    /// </summary>
    public interface IVolumeButtonList : IVolumeButtonListByItem
    {
        object UserObject { get; set; }
    }

    /// <summary>
    /// Digital callback used in feedback events.
    /// </summary>
    /// <param name="boolInputSig">The <see cref="BoolInputSig"/> signal data.</param>
    /// <param name="volumebuttonlist">The <see cref="IVolumeButtonList"/> on which to apply the feedback.</param>
    public delegate void VolumeButtonListBoolInputSigDelegate(BoolInputSig boolInputSig, IVolumeButtonList volumebuttonlist);

    /// <summary>
    /// VolumeButtonList
    /// </summary>
    internal partial class VolumeButtonList : IVolumeButtonList, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }

        public object UserObject { get; set; }

        /// <summary>
        /// Gets the ControlJoinId a.k.a. SmartObjectId.  This Id identifies the extender symbol.
        /// </summary>
        public uint ControlJoinId { get; private set; }

        private IList<BasicTriListWithSmartObject> _devices;

        /// <summary>
        /// Gets the list of devices.
        /// </summary>
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private static class Joins
        {
            /// <summary>
            /// Digital signals,
            /// </summary>
            internal static class Booleans
            {
                /// <summary>
                /// Output or Event digital signal from panel to Control System: MainPage.VolumeButtonList.Button1ItemPress
                /// Button1.ItemPress
                /// </summary>
                public const uint Button_1_Button_PressEvent = 1001;

                /// <summary>
                /// Output or Event digital signal from panel to Control System: MainPage.VolumeButtonList.Button2ItemPress
                /// Button2.ItemPress
                /// </summary>
                public const uint Button_2_Button_PressEvent = 1002;

                /// <summary>
                /// Output or Event digital signal from panel to Control System: MainPage.VolumeButtonList.Button3ItemPress
                /// Button3.ItemPress
                /// </summary>
                public const uint Button_3_Button_PressEvent = 1003;


            }
        }

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeButtonList"/> component class.
        /// </summary>
        /// <param name="componentMediator">The <see cref="ComponentMediator"/> used to instantiate the component.</param>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        /// <param name="itemCount">The number of items.</param>
        internal VolumeButtonList(ComponentMediator componentMediator, uint controlJoinId, uint? itemCount)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId, itemCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeButtonList"/> component class.
        /// </summary>
        /// <param name="componentMediator">The <see cref="ComponentMediator"/> used to instantiate the component.</param>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        internal VolumeButtonList(ComponentMediator componentMediator, uint controlJoinId) : this(componentMediator, controlJoinId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance with default itemCount.
        /// </summary>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        private void Initialize(uint controlJoinId)
        {
            Initialize(controlJoinId, null);
        }

        private Dictionary<string, Indexes> _indexLookup = new Dictionary<string, Indexes>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeButtonList"/> component class.
        /// </summary>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        /// <param name="itemCount">The number of items.</param>
        private void Initialize(uint controlJoinId, uint? itemCount)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanItemEvent(controlJoinId, Joins.Booleans.Button_1_Button_PressEvent, GetIndexes, onButton_Press);
        }

        /// <summary>
        /// Get the offset when using indexed complex components.
        /// </summary>
        /// <param name="controlJoinId">The SmartObjectId of the component.</param>
        /// <param name="join">The join offset.</param>
        /// <param name="eSigType">The join data type.</param>
        private Indexes GetIndexes(uint controlJoinId, uint join, eSigType eSigType)
        {
            if (controlJoinId == ControlJoinId &&
                join >= Joins.Booleans.Button_1_Button_PressEvent &&
                join <= 1003)
            {
                return new Indexes(0, (ushort)(join - Joins.Booleans.Button_1_Button_PressEvent), false);
            }

            return new Indexes(0, 0, true);
        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        #endregion

        #region CH5 Contract


        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return (int)ControlJoinId;
        }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "VolumeButtonList", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            Button_PressEvent = null;
        }

        #endregion
    }
}
