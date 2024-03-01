/*
 *  This class is simply used to setup all of our hardware devices and set up our communication between
 * the different parts.   This is purely for organization and readability of the code.  Performance is no different than
 * if you were to put all this in ControlSystem.cs
 *
 * The rules there STILL apply here.   We Instantiated this class in the initialize method,  we have 20 seconds to return so that
 * method can finish.   Just because we jumped over to another class does not mean those rules do not apply anymore.
 * If you have to do something that takes time, you MUST spin up a thread and have that do the work while you let the
 * constructor finish quickly and return.
 *
 * This class can get large if you have a large amount of hardware.  it would be a good idea to split this class up to multiple
 * classes that do each job if you find it starts to become huge and hard to find what you are looking for in it.
 * You can leverage regions to manage the code readability better, some people have the opinion that the use of regions
 * are code smell. It's only their opinion and you can use what works for you, just do not abuse it.
 * Using regions to make a 20,000 line class more manageable would be a misuse of regions and a good indicator that
 * you should break the class up.
 *
 * Code in here should deal with the devices only or pass information to the event broker.
 *
 */

using Crestron.SimplSharp;
using Masters_2024_MSS_521.Devices;
using Masters_2024_MSS_521.MessageSystem;

namespace Masters_2024_MSS_521
{
    public class DeviceSetup

    {

        private ushort _airMediaPinCode = 0;
        private string _airMediaAddress = "";

        public AirMedia3100 MyAirMedia;
        public CrestronConnected MyCrestronConnected;
        public Nvx351 MyNvx;

        public DeviceSetup(ControlSystem cs)
        {

            MyAirMedia = new AirMedia3100(0x22, cs);
            MyAirMedia.BaseEvent += MyAirMedia_BaseEvent;
            MyAirMedia.SetPinCodeRandom(); // Customer wants the pincode to be random
            MessageBroker.AddDelegate("AirMediaSetPinCode", AirMediaSetPinCode);

            MyNvx = new Nvx351(0x11, Nvx351.EMode.Rx, cs);
            MyNvx.BaseEvent += MyNvx_BaseEvent;
            MessageBroker.AddDelegate("NvxSetStreamLocation", NvxSetStreamLocation);
            MessageBroker.AddDelegate("NvxSetInput", NvxSetInput);

            MyCrestronConnected = new CrestronConnected(0x09, cs);
            MyCrestronConnected.BaseEvent += MyCrestronConnected_BaseEvent;
            MessageBroker.AddDelegate("DisplayPower", DisplayPower);
            MessageBroker.AddDelegate("DisplayInput", DisplayInput);
            MessageBroker.AddDelegate("DisplayVolumeDown", DisplayVolumeDown);
            MessageBroker.AddDelegate("DisplayVolumeUp", DisplayVolumeUp);
            MessageBroker.AddDelegate("DisplayVolumeStop", DisplayVolumeStop);
            MessageBroker.AddDelegate("DisplayMuteToggle", DisplayMuteToggle);
        }

        // Event handlers
        private void MyCrestronConnected_BaseEvent(object sender, CrestronConnected.Args e)
        {
            if(e.Message == "Volume")
                MessageBroker.SendMessage("SetVolumeBarFeedback", new Message { Analog = e.VolumeFb });
            else if (e.Message == "Mute")
                MessageBroker.SendMessage("SetMuteFeedback", new Message { Digital = e.MuteOn });
        }

        private void MyNvx_BaseEvent(object sender, Nvx351.Args e)
        {
            // These are not currently used in this program but can be used to show if sync is available
            //MessageBroker.SendMessage("NvxInput1SyncFeedback", new Message { Digital = e.Hdmi1Sync });
            //MessageBroker.SendMessage("NvxInput2SyncFeedback", new Message { Digital = e.Hdmi2Sync });
        }

        private void MyAirMedia_BaseEvent(object sender, AirMedia3100.Args e)
        {
            if(e.CurrentPinCode != _airMediaPinCode)
                MessageBroker.SendMessage("AirmediaPinFb", 
                    new Message { Serial =  e.CurrentPinCode.ToString().PadLeft(4,'0') });
            if(e.CurrentAddress != _airMediaAddress)
                MessageBroker.SendMessage("AirmediaAddressFb", new Message { Serial =  e.CurrentAddress });
        }

        /*
         *  This is where we create the messaging broker wrapper methods for all the different hardware.
         * your helper classes or frameworks may or may not be designed around the messaging system or other frameworks you
         * designed.  It is 100% ok to write simple wrapper methods to make something work or interface one method with another.
         * Our messaging broker system sends a payload in a Message class, absolutely none of the crestron devices support this
         * data object, nor does any of the framework classes around them.   we simply create a method to accept the message
         * and then we just use the part of the message we need to send to the device or framework we are using.
         * be sure to be organized and clear with these.   this is a point that can cause confusion for yourself or others
         * that deal with your code later.
         *
         * Commenting your code as to what they are and even comments by the hardware setup as to what class these are in will make your life
         * easier and the next  programmers life easier.
         */

        // Airmedia
        private void AirMediaSetPinCode(Message m)
        {
            MyAirMedia.SetPinCode(m.Analog);
        }

        // Nvx
        private void NvxSetStreamLocation(Message m)
        {
            MyNvx.SetStreamLocation(m.Serial);
        }

        private void NvxSetInput(Message m)
        {
            /*
             *  the Nvx Interface class we have wants to pass the enum for selecting the input chosen
             *  This is great for using that class directly, but in this example we are only passing
             *  the 3 base crestron data types so we need to convert the 0-3 "analog" value to what the
             *  class wants, here we can simply cast it.
             */
            MyNvx.SetInput((Nvx351.ESource)m.Analog);
        }

        // Display
        private void DisplayPower(Message m)
        {
            switch (m.Digital)
            {
                case true:
                    MyCrestronConnected.On();
                    break;
                case false:
                    MyCrestronConnected.Off();
                    break;
            }
        }

        private void DisplayInput(Message m)
        {
            MyCrestronConnected.Input(m.Analog);
        }

        private void DisplayVolumeDown(Message m)
        {
            MyCrestronConnected.VolumeDown();
        }

        private void DisplayVolumeUp(Message m)
        {
            MyCrestronConnected.VolumeUp();
        }

        private void DisplayVolumeStop(Message m)
        {
            MyCrestronConnected.VolumeStop();
        }

        private void DisplayMuteToggle(Message m)
        {
            MyCrestronConnected.MuteToggle();
        }
    }
}