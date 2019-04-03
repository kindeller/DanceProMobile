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
            AddButtonText(MediaButton, "Download your media at dance competitions");
            AddButtonText(PhotosButton, "View and purchase concert and competition images");
            AddButtonText(VideosButton, "Concert video order form");
        }

        private void AddButtonText(UIButton button, string text)
        {

            var x = button.Frame.Width / 4;
            var y = button.Frame.Height / 4;
            var width = button.Frame.Width / 2;
            var height = button.Frame.Height / 2;
            UITextView textView = new UITextView(new CoreGraphics.CGRect(x, y, width, height));
            textView.TextAlignment = UITextAlignment.Center;
            textView.Text = text;
            button.AddSubview(textView);
        }

        partial void UIButton3099_TouchUpInside(UIButton sender) => ViewModel.OpenPhotosCommand.Execute(null);

        partial void UIButton3100_TouchUpInside(UIButton sender) => ViewModel.OpenVideosCommand.Execute(null);

        partial void MediaButton_TouchUpInside(UIButton sender)
        {
            TabBarController.SelectedIndex = 1;
        }
    }
}
