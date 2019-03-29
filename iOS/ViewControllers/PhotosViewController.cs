using System;
using System.Windows.Input;
using UIKit;

namespace DancePro.iOS.ViewControllers
{
    public partial class PhotosViewController : UIViewController
    {
        ICommand OpenPhotosCommand;
        public PhotosViewController() : base("PhotosViewController", null)
        {
        }

        public PhotosViewController(IntPtr intPtr) : base(intPtr)
        {
            OpenPhotosCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://photos.dancepro.com.au/"));

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            OpenPhotosCommand.Execute(null);
            UITabBarController controller = UIApplication.SharedApplication.KeyWindow.RootViewController as UITabBarController;
            if (controller != null)
            {
                controller.SelectedIndex = 0;
            }
        }
    }
}

