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
        }

        partial void UIButton3099_TouchUpInside(UIButton sender) => ViewModel.OpenPhotosCommand.Execute(null);

        partial void UIButton3100_TouchUpInside(UIButton sender) => ViewModel.OpenVideosCommand.Execute(null);

        partial void MediaButton_TouchUpInside(UIButton sender)
        {
            TabBarController.SelectedIndex = 1;
        }
    }
}
