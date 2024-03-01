using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSXpanel;                    // Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    public class MediaSubPage
    {
        private MSSXpanel.Contract _myContract;
        public MediaSubPage(Contract myContract)
        {
            _myContract = myContract;
            _myContract.MediaControl.Dpad.Button_PressEvent += Dpad_Button_PressEvent;
            _myContract.MediaControl.BackButton_PressEvent += MediaPage_BackButton_PressEvent;
            _myContract.MediaControl.MenuButton_PressEvent += MediaPage_MenuButton_PressEvent;
            _myContract.MediaControl.PlayButton_PressEvent += MediaPage_PlayButton_PressEvent;
        }

        private void MediaPage_PlayButton_PressEvent(object sender, UIEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MediaPage_MenuButton_PressEvent(object sender, UIEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MediaPage_BackButton_PressEvent(object sender, UIEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Dpad_Button_PressEvent(object sender, DpadEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
