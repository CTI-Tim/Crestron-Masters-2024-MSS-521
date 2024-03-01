using System;
using System.Timers;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;

namespace Masters_2024_MSS_521.Devices
{
    public class Nvx351
    {
        /*
         *  This is a framework wrapper class for NVX. This allows you to unify how hardware works so the interface you have to
         * deal with is more consistent across devices.
         *
         * We are going to recreate a couple of NVX enums so that we do not have to bring in that library elsewhere
         * later.  Having everything in your class simplifies it's use and reduces confusion when others use your class
         */

        public enum EMode
        {
            Rx,
            Tx
        }

        public enum ESource
        {
            Disable,
            Hdmi1,
            Hdmi2,
            Stream
        }

        private readonly ComPort _myComPort;
        private readonly DmNvx351 _myNvx;
        private Timer _myTimer;
        private bool _timerRunning;

        /// <summary>
        ///     Creates a DmNvx351 for use in the program Auto Initiation is enabled in code as well as led's
        ///     Automatic input routing is disabled.
        /// </summary>
        /// <param name="ipId">IpID for the device to use</param>
        /// <param name="mode">Initial Mode the Device is set to with the program starts</param>
        /// <param name="cs">Control System this Device is attached to</param>
        public Nvx351(uint ipId, EMode mode, CrestronControlSystem cs)
        {
            _myNvx = new DmNvx351(ipId, cs);
            _myNvx.OnlineStatusChange += MyNvx_OnlineStatusChange;
            _myNvx.BaseEvent += MyNvx_BaseEvent;

            // Set our desired default settings
            _myNvx.Control.EnableLeds();
            _myNvx.Control.EnableAutomaticInitiation();
            _myNvx.Control.DisableAutomaticInputRouting();
            //_myNvx.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1; //Default to hdmi1

            if (mode == EMode.Rx)
                _myNvx.Control.DeviceMode = eDeviceMode.Receiver;
            else
                _myNvx.Control.DeviceMode = eDeviceMode.Transmitter;

            _myNvx.Register();

            if (_myNvx.RegistrationFailureReason !=
                eDeviceRegistrationUnRegistrationFailureReason.DEVICE_REG_UNREG_RESPONSE_NO_FAILURE)
                ErrorLog.Error($"## ERROR Unable to register Nvx 351 at IPID {ipId:X}");

            // Set Up the Com Port
            /*
             *  All the settings are hardcoded here.  in this case this meets 95% of my use cases with devices using 9600:n:8:1
             *  you may see more variation in these settings and therefore should make your class support changing these
             *  by adding in the methods for adjusting them.
             */
            _myComPort = _myNvx.ComPorts[1]; // Crestron is 1 based  don't forget.
            _myComPort.SetComPortSpec(
                ComPort.eComBaudRates.ComspecBaudRate9600,
                ComPort.eComDataBits.ComspecDataBits8,
                ComPort.eComParityType.ComspecParityNone,
                ComPort.eComStopBits.ComspecStopBits1,
                ComPort.eComProtocolType.ComspecProtocolRS232,
                ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                false);
            _myComPort.SerialDataReceived += MyComPort_SerialDataReceived;
        }

        public bool RegisteredFeedback { get; private set; }
        public bool OnlineFeedback { get; private set; }
        public EMode ModeFeedback { get; private set; }
        public string MulticastAddressFeedback { get; private set; }
        public string IpAddressFeedback { get; private set; }
        public string ServerUrlFeedback { get; private set; }
        public ESource InputFeedback { get; private set; }

        public event EventHandler<Args> BaseEvent;
        public event EventHandler<Serial> SerialRxEvent;


        // Public methods

        public void SetMulticastAddress(string address)
        {
            _myNvx.Control.MulticastAddress.StringValue = address;
            UpdateInfo();
        }

        /*
         *  This is how we are going to tune in sources. and this is how you connect to global sources.
         *
         * Be aware that we are relying on auto initiation to start the stream, if you are programming for an NVX installation
         * on a compromised network that can not handle all the data all of the time then you will have to manage
         * stream start and stop to ensure that rooms that are not in use shut down their stream to release bandwidth
         * for other rooms to use. If the network was set up to be blocking and lacks the bandwidth you may need to do a bit more
         * programming to manage the data usage. In this case the network was designed properly and do not have to consider that.
         */
        public void SetStreamLocation(string location)
        {
            if (location.Length < 16) // were we sent just an ipv4 address? then add the complete RTSP URL for NVX
                location = "rtsp://" + location + ":554/live.sdp";
            CrestronConsole.PrintLine($"Stream Location is {location}");
            _myNvx.Control.ServerUrl.StringValue = location;
            //_myNvx.Control.Start();  // if auto initiation is on this is not needed
        }

        /// <summary>
        ///     Sets the desired input the NVX will send to the output
        /// </summary>
        /// <param name="source">this enum matches what the NVX class uses</param>
        public void SetInput(ESource source)
        {
            _myNvx.Control.VideoSource = (eSfpVideoSourceTypes)source;
            UpdateInfo();
        }

        public void SetAnalogOutVolume(ushort volume)
        {
            _myNvx.Control.AnalogAudioOutputVolume.UShortValue = volume;
        }

        public void SerialTx(string tx)
        {
            _myComPort.Send(tx);
        }

        /// <summary>
        ///     Send the CEC control string OUT to Hdmi inputs
        /// </summary>
        /// <param name="tx">CEC command string to send</param>
        /// <param name="input">input number to send it to</param>
        public void CecInputSend(string tx, uint input)
        {
            // NOTE:  this is ASCII in regards to Crestron.  not ASCII in regards to c# and the ASCII codepage
            // Remember C# ASCII has a hard stop at 127. to use all 256 characters you need Code page 28591
            // In this case eEncodingASCII will do what you want and send all 256 characters possible on the CEC pin

            if (input > _myNvx.HdmiIn.Count) input = 1;

            _myNvx.HdmiIn[input].StreamCec.Send.StringEncoding = eStringEncoding.eEncodingASCII;
            _myNvx.HdmiIn[input].StreamCec.Send.StringValue = tx;
        }

        /// <summary>
        ///     Send a CEC command to the Output
        /// </summary>
        /// <param name="tx">CEC command string to send</param>
        public void CecOutputSend(string tx)
        {
            _myNvx.HdmiOut.StreamCec.Send.StringEncoding = eStringEncoding.eEncodingASCII;
            _myNvx.HdmiOut.StreamCec.Send.StringValue = tx;
        }


        // Private methods 

        private void UpdateInfo()
        {
            RegisteredFeedback = _myNvx.Registered;
            OnlineFeedback = _myNvx.IsOnline;
            ModeFeedback = (EMode)_myNvx.Control.DeviceModeFeedback;
            MulticastAddressFeedback = _myNvx.Control.MulticastAddressFeedback.StringValue;
            IpAddressFeedback = _myNvx.Network.IpAddressFeedback.StringValue;
            ServerUrlFeedback = _myNvx.Control.ServerUrlFeedback.StringValue;
            InputFeedback = (ESource)_myNvx.Control.VideoSourceFeedback;
        }

        private void CheckForReboot()
        {
            if (_myNvx.Control.RebootRequiredFeedback.BoolValue)
                _myNvx.Control.Reboot();
        }


        // Timer code used to debounce the MyNVX_BaseEvent below

        private void SetTimer()
        {
            _timerRunning = true;
            _myTimer = new Timer(300); //300ms calms it down from 17 events to only 2
            _myTimer.Elapsed += MyTimer_Elapsed;
            _myTimer.AutoReset = false;
            _myTimer.Enabled = true;
        }

        // This is the method called when the timer elapses
        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateInfo();
            OnRaiseEvent(new Args("BaseEvent"));
            CheckForReboot();
            _timerRunning = false;
            _myTimer.Stop();
            _myTimer.Close();
        }


        // Hardware Event Handlers

        private void MyNvx_BaseEvent(GenericBase device, BaseEventArgs args)
        {
            /*
             *  BaseEvents are extremely chatty. In this case
             *  just changing the stream location fires 17 events.  we do not care that all those fired as we can
             *  not figure out what each one of those are reporting,  so we need to de-bounce the Base event to get that the event
             *  happened and then we can update our information all at once.
             *
             *  We do not want to throttle it too much.  the timer is set to 300ms so this reduces it to only 2 events.
             *  the first is when the change happens the second is when the video stream actually starts when you
             *  Tell the NVX to change streams.
             *
             *  The disadvantage with doing it this way is all event triggers will be delayed by 300ms. if you have extremely critical
             *  response timing then you should rework the code to trigger the first event instantly and then ignore the rest for
             *  the next 300ms.
             *
             */

            // Lets use our timer
            if (!_timerRunning) // If timer running = false  we can start our timer
                SetTimer();
        }

        private void MyNvx_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            UpdateInfo();
            OnRaiseEvent(new Args("OnlineOffline"));
            CheckForReboot();
        }

        private void MyComPort_SerialDataReceived(ComPort receivingComPort, ComPortSerialDataEventArgs args)
        {
            OnRaiseSerialEvent(new Serial(args.SerialData));
        }

        // Our custom Event methods

        protected virtual void OnRaiseEvent(Args e)
        {
            // we make a copy because if subscribers unsubscribed during the event it can cause an exception
            var raiseEvent = BaseEvent;
            if (raiseEvent != null)
            {
                e.Online = _myNvx.IsOnline;
                e.Registered = _myNvx.Registered;
                e.Mode = (EMode)_myNvx.Control.DeviceModeFeedback;
                e.MulticastAddress = _myNvx.Control.MulticastAddressFeedback.StringValue;
                e.CurrentAddress = _myNvx.Network.IpAddressFeedback.StringValue;
                e.ServerUrl = _myNvx.Control.ServerUrlFeedback.StringValue;
                e.Input = (ESource)_myNvx.Control.VideoSourceFeedback;

                e.Hdmi1Sync = _myNvx.HdmiIn[1].SyncDetectedFeedback.BoolValue;
                e.Hdmi2Sync = _myNvx.HdmiIn[2].SyncDetectedFeedback.BoolValue;


                raiseEvent(this, e); // Fire the event
            }
        }

        protected virtual void OnRaiseSerialEvent(Serial e)
        {
            var raiseEvent = SerialRxEvent;
            if (raiseEvent != null) raiseEvent(this, e); // Fire the event
        }

        // Nested class used for our event args
        public class Args
        {
            public Args(string message)
            {
                Message = message;
            }

            public string Message { get; set; }
            public bool Online { get; set; }
            public bool Registered { get; set; }
            public EMode Mode { get; set; }
            public string CurrentAddress { get; set; }
            public string MulticastAddress { get; set; }
            public string ServerUrl { get; set; }
            public ESource Input { get; set; }
            public bool Hdmi1Sync { get; set; }
            public bool Hdmi2Sync { get; set; }
            
        }

        public class Serial
        {
            public Serial(string rx)
            {
                Rx = rx;
            }

            public string Rx { get; set; }
        }
    }
}