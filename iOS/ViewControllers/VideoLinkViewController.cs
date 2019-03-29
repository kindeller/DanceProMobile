using System;
using System.Windows.Input;
using UIKit;

namespace DancePro.iOS
{
    public partial class VideoLinkViewController : UIViewController
    {
        ICommand OpenVideoCommand;

        public VideoLinkViewController(IntPtr handle) : base(handle)
        {
            OpenVideoCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://videos.dancepro.com.au"));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //Open Browser
            OpenVideoCommand.Execute(null);
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
