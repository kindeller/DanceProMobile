using System;

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
            Title = ViewModel.Title;
            //TransferMediaSubtitle.AdjustsFontForContentSizeCategory = true;
            //WebsiteSubtitle.AdjustsFontForContentSizeCategory = true;
            //TransferMediaSubtitle.Font = MyMediaSubtitle.Font;
            //WebsiteSubtitle.Font = MyMediaSubtitle.Font;
            AddButtonText(MediaButton, "View and manage your personal DancePro media gallery.");
            AddButtonText(TransferButton, "Transfer media from a DancePro Kiosk event.");
            AddButtonText(WebsiteButton, "Visit our Website for other competitions and events.");
        }

        private void AddButtonText(UIButton button, string text)
        {

            var x = 0;
            var y = button.Frame.Height / 8;
            var width = button.Frame.Width;
            var height = button.Frame.Height / 2;
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
            ViewModel.OpenWebsiteCommand.Execute(null);
        }

        partial void TransferButton_TouchUpInside(UIButton sender)
        {
            TabBarController.SelectedIndex = 1;
        }
    }
}
