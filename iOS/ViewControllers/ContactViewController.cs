﻿using System;
using System.Windows.Input;
using UIKit;

namespace DancePro.iOS
{
    public partial class ContactViewController : UIViewController
    {
        ICommand OpenContactCommand;

        public ContactViewController(IntPtr handle) : base(handle)
        {
            OpenContactCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://www.dancepro.com.au/Contact"));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //Open Browser
            OpenContactCommand.Execute(null);
            //Get Tab Controller
            UITabBarController controller = UIApplication.SharedApplication.KeyWindow.RootViewController as UITabBarController;
            if (controller != null)
            {
                //Return to Home
                controller.SelectedIndex = 0;
            }
        }
    }
}
