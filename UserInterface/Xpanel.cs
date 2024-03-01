using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using Masters_2024_MSS_521.MessageSystem;
using MSSXpanel;// Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    public class Xpanel
    {
        private MainPage _mainPage;
        private MediaSubPage _mediaPage;
        private readonly Contract _myContract;
        private readonly XpanelForHtml5 _myXpanel;

        private readonly PageNavigation _navigation;


        public Xpanel(uint ipId, ControlSystem cs)
        {
            _myXpanel = new XpanelForHtml5(ipId, cs);
            _myXpanel.OnlineStatusChange += _myXpanel_OnlineStatusChange;
            _myXpanel.Register();
            _myContract = new Contract();
            _myContract.AddDevice(_myXpanel);

            // Bring in the pages classes
            _navigation = new PageNavigation(_myContract); 
            _mainPage = new MainPage(_myContract);
            _mediaPage = new MediaSubPage(_myContract);

            // subscribing to the power buttons here to trigger their automation
            _myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent; // On
            _myContract.PowerOffOk.PowerOffYesButton_PressEvent +=
                PowerOffOkSubpage_PowerOffYesButton_PressEvent; //off

            // Lets set the room name header bar 
            _myContract.HeaderBar.RoomNameLabel_Indirect("MSS-521 Conference Room");


            // Customer wants the system to flip to the start page on power loss or program load
            _navigation.ShowStartPage();
        }

        // If the xpanel goes offline or online this will make sure we go back to the page the program wants us on
        private void _myXpanel_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            _navigation.ReloadCurrentPage();
        }

        private void StartPage_Button_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                MessageBroker.SendMessage("SystemOn", new Message()); //trigger the system on automation
        }

        private void PowerOffOkSubpage_PowerOffYesButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                MessageBroker.SendMessage("SystemOff", new Message()); //trigger the system off automation
        }
    }
}