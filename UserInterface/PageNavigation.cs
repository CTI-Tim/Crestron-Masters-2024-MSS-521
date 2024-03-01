using Crestron.SimplSharpPro;
using Crestron.SimplSharp;
using Masters_2024_MSS_521.MessageSystem;
using MSSXpanel;
using MSSXpanel.MainPage; // Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    /*
     *  This class should really only do page navigation. 
     * this class is for touchpanel navigation,  so leave it at that.   the nice part of the contracts being it's own class means
     * we can subscribe to the button events elsewhere and put the code for actually triggering things there leaving this clean for only
     * doing one job, which is the page navigation for the panel.
     *
     * Because the Power ON and Off button event handlers are in here.  I will call those messages to the message broker from here.
     * Refrain from attaching those events to the page flip logic.   let the automation call the actual page flips.
     */
    public class PageNavigation
    {
        private MSSXpanel.Contract _myContract;
        private int _currentPage = 0;
        public PageNavigation(Contract myContract)
        {
            _myContract = myContract;

            MessageSystem.MessageBroker.AddDelegate("ShowMediaPlayPage", ShowMediaPlayPage);
            MessageSystem.MessageBroker.AddDelegate("ShowStartPage", ShowSystemStartPage);
            MessageSystem.MessageBroker.AddDelegate("ShowMainPage", ShowSystemMainPage);

            _myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent;
            _myContract.HeaderBar.PowerButton_PressEvent += MainPage_PowerButton_PressEvent;
            // The below subpage/Widget was set to "global" in construct to get access like this.
            // Select the widget, then in layer manager window select the WidgetContainer.  there you will find global for ControlContract
            _myContract.PowerOffOk.PowerOffNoButton_PressEvent += PowerOffOkSubpage_PowerOffNoButton_PressEvent;
            _myContract.PowerOffOk.PowerOffYesButton_PressEvent += PowerOffOkSubpage_PowerOffYesButton_PressEvent;

        }
        //They need to add this as well for the first lab
        private void MediaPage_HomeButton_PressEvent(object sender, UIEventArgs e)
        {
            ShowMainPage();
        }

        public void ShowStartPage()
        {
            _myContract.StartPage.StartPage_VisibilityJoin((sig, m) => sig.Pulse(20));
            _currentPage = 0;
        }
        public void ShowMainPage()
        {
            _myContract.MainPage.MainPage_VisibilityJoin((sig, m) => sig.Pulse(20));
            _currentPage = 1;
        }
        public void ShowMediaPage()
        {
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.Pulse(20));
            _currentPage = 2;
        }
        public void ReloadCurrentPage()
        {
            switch (_currentPage)
            {
                case 0:
                    ShowStartPage();
                    break;
                case 1:
                    ShowMainPage();
                    break;
                case 2:
                    ShowMediaPage();
                    break;
            }
        }

        public void ShowPowerOffSubpage()
        {
            // You still have to show the subpage on that page. so you will have to trigger visibility on every page the subpage exists on.
            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility(true);
            
        }
        public void HidePowerOffSubpage()
        {
            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility(false);
        }


        // Wrapper methods to accept a message to fit the signature
        private void ShowMediaPlayPage(MessageSystem.Message m)
        {
            ShowMediaPage();
        }
        private void ShowSystemStartPage(MessageSystem.Message m)
        {
            ShowStartPage();
        }
        private void ShowSystemMainPage(MessageSystem.Message m)
        {
            ShowMainPage();
        }

        // Event handler methods
        private void StartPage_Button_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                ShowMainPage();
            }
        }
        private void MainPage_PowerButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                ShowPowerOffSubpage();
            }
        }
        private void PowerOffOkSubpage_PowerOffNoButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                HidePowerOffSubpage();
            }
        }
        private void PowerOffOkSubpage_PowerOffYesButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                HidePowerOffSubpage();
            }
        }

        
    }
}
