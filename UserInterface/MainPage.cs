using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Masters_2024_MSS_521.MessageSystem;
using MSSXpanel;// Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    public class MainPage
    {
        private readonly Contract _myContract;

        public MainPage(Contract myContract)
        {
            _myContract = myContract;

            MessageBroker.AddDelegate("SetSourceListFeedback", SetSourceListFeedback);
            MessageBroker.AddDelegate("SetVolumeBarFeedback", SetVolumeBarFeedback);
            MessageBroker.AddDelegate("SetMuteFeedback", SetMuteFeedback);
            MessageBroker.AddDelegate("SetSourceFeedback", SetSourceFeedback);
            MessageBroker.AddDelegate("AirmediaAddressFb", SetAirmediaAddressFb);
            MessageBroker.AddDelegate("AirmediaPinFb", SetAirmediaPinFb);
            MessageBroker.AddDelegate("NvxAddressFeedback", SetNvxAddressFeedback);

            //All the buttons on the MainPage are button lists.
            _myContract.MainPage.SourceList.Button_PressEvent += SourceList_Button_PressEvent;
            _myContract.MainPage.VolumeButtonList.Button_PressEvent += VolumeButtonList_Button_PressEvent;


            //Lets populate the source list
            _myContract.MainPage.SourceList.Button_Text(0, "Apple TV");
            _myContract.MainPage.SourceList.Button_Text(1, "Airmedia");
            for (ushort i = 2; i <= 5; i++) _myContract.MainPage.SourceList.Button_Text(i, "Global Source " + i);
        }

        private void VolumeButtonList_Button_PressEvent(object sender, IndexedButtonEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                //volume buttons are a list
                switch (e.ButtonIndex)
                {
                    case 0: // Vol Up
                        MessageBroker.SendMessage("DisplayVolumeUp", new Message());
                        break;
                    case 1: // Mute Toggle
                        MessageBroker.SendMessage("DisplayMuteToggle", new Message());
                        break;
                    case 2: // Vol Dn
                        MessageBroker.SendMessage("DisplayVolumeDown", new Message());
                        break;
                }
            }
            else
            {
                //Button was released stop the ramping
                if (e.ButtonIndex != 1)
                    MessageBroker.SendMessage("DisplayVolumeStop", new Message());
            }
        }

        private void SourceList_Button_PressEvent(object sender, IndexedButtonEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                MessageBroker.SendMessage("SourceSelect", new Message { Analog = e.ButtonIndex }); //Source Select is in Automation.cs

        }

        private void SetSourceListFeedback(Message m)
        {
            for (ushort i = 0; i <= 5; i++)
                _myContract.MainPage.SourceList.Button_Selected(i, false);
            _myContract.MainPage.SourceList.Button_Selected(m.Analog, true);

            ShowMainSubpages(m.Analog);
        }

        private void SetVolumeBarFeedback(Message m)
        {
            _myContract.MainPage.VolumeBar_Touchfb(m.Analog);
        }
        private void SetMuteFeedback(Message m)
        {
            _myContract.MainPage.MutedFeedback_Visibility(m.Digital);
        }

        private void SetSourceFeedback(Message m)
        {
            _myContract.MainPage.SourceFeedbackLabel_Indirect(m.Serial);
        }

        private void SetAirmediaAddressFb(Message m)
        {
            _myContract.AirMediaInfo.AirmediaAddressFb_Indirect(m.Serial);
        }
        private void SetAirmediaPinFb(Message m)
        {
            _myContract.AirMediaInfo.AirmediaPinFb_Indirect(m.Serial);
        }

        private void SetNvxAddressFeedback(Message m)
        {
            _myContract.NvxInfo.NvxAddressFb_Indirect(m.Serial);
        }

        private void ShowMainSubpages(int page)
        {
            // TODO: Finish subpage visibility 
            /*
             *  page will contain what was requested.
             *  Add in code to show the subpages one at a tim as they are needed.
             *  there are 3 total subpages,  one needs to be displayed for multiple sources.
             *  the variable page above will contain a number from 0-6
             *  0 = media page  1 = Airmedia page all the rest are the NVX global source page
             *
             *  You will need to find in the contract files the subpage classes and objects to use
             *  Hint:  MainPage is the page they are on.
             *
             */
        }
    }
}