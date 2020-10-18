using System;
using Microsoft.AppCenter.Analytics;
using UIKit;

namespace DancePro.iOS
{
    public partial class MenuViewController : UIViewController
    {
        public MenuModel ViewModel { get; set; }

        public MenuViewController(IntPtr handle) : base(handle)
        {
            ViewModel = new MenuModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Analytics.TrackEvent("[Menu] Loaded Page");

            //TODO: Remove temp fix for darkmode issues and add dark mode support
            //if (AppDelegate.CheckVersion(13))
            //{
            //    OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            //}
            Title = ViewModel.Title;
            //TransferMediaSubtitle.AdjustsFontForContentSizeCategory = true;
            //WebsiteSubtitle.AdjustsFontForContentSizeCategory = true;
            //TransferMediaSubtitle.Font = MyMediaSubtitle.Font;
            //WebsiteSubtitle.Font = MyMediaSubtitle.Font;
            AddButtonText(MediaButton, "View and manage your personal DancePro media gallery.");
            AddButtonText(TransferButton, "Transfer photos and videos onto your device from our kiosk during a competition.");
            AddButtonText(WebsiteButton, "Visit our website to view and purchase concert photos and videos.");
            UIImageView image = new UIImageView(UIImage.FromBundle("MenuBackground"));
            
            image.ContentMode = UIViewContentMode.ScaleAspectFill;
            image.Frame = View.Frame;
            image.Layer.ZPosition = -1;
            image.Alpha = new nfloat(0.25);
            View.AddSubview(image);
        }

        private void AddButtonText(UIButton button, string text)
        {

            var x = 0;
            var y = button.Frame.Height - 5;
            var width = View.Frame.Width;
            var height = button.Frame.Height * 1.5;
            UILabel label = new UILabel(new CoreGraphics.CGRect(x, y, width, height));
            label.Lines = 2;
            label.TextAlignment = UITextAlignment.Center;
            label.Text = text;
            
            button.AddSubview(label);
        }

        partial void MediaButton_TouchUpInside(UIButton sender)
        {
            TabBarController.SelectedIndex = 2;
        }

        partial void WebsiteButton_TouchUpInside(UIButton sender)
        {
            Analytics.TrackEvent("[Menu] Clicked Website Button");
            ViewModel.OpenWebsiteCommand.Execute(null);
        }

        partial void TransferButton_TouchUpInside(UIButton sender)
        {
            TabBarController.SelectedIndex = 1;
        }
    }
}
