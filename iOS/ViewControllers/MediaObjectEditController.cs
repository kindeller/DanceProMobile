using System;
using System.Collections.Generic;
using DancePro.Models;
using UIKit;

namespace DancePro.iOS.ViewControllers
{
    public partial class MediaObjectEditController : UIViewController
    {

        public MediaObject MediaObject { get; set; }
        public MyMediaViewController controller { get; set; }

        public MediaObjectEditController() : base("MediaObjectEditController", null)
        {
        }

        public MediaObjectEditController(IntPtr intPtr) : base(intPtr)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            View.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);

            switch (MediaObject.MediaType) {

                case MediaTypes.Audio:
                    SaveButton.Enabled = false;
                    break;
                case MediaTypes.Other:
                    ShareButton.Enabled = false;
                    DuplicateButton.Enabled = false;
                    SaveButton.Enabled = false;
                    break;
            };

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void PresentAlert(UIAlertController alert)
        {
            if (controller != null)
            {
                PresentViewController(alert, true, null);
            }
            else
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                vc.PresentViewController(alert, true, null);
            }
        }

        partial void Cancel_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }

        partial void Rename_TouchUpInside(UIButton sender)
        {
            var alert = UIAlertController.Create("Rename", "Select the new name...", UIAlertControllerStyle.Alert);
            alert.AddTextField((obj) => obj.Text = System.IO.Path.GetFileNameWithoutExtension(MediaObject.FileName));
            var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) => {
                App.MediaService.RenameMediaObject(MediaObject, alert.TextFields[0].Text);
                controller.GetMedia();
            });
            alert.AddAction(actionCancel);
            alert.AddAction(actionOk);
            PresentViewController(alert, true, null);
        }

        partial void Delete__TouchUpInside(UIButton sender)
        {
            var isSuccess = false;
            var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel,null);
            var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) => {
                isSuccess = App.MediaService.DeleteMediaObject(MediaObject);
                controller.GetMedia();
                DismissViewController(true, null);
            });
            var alert = UIAlertController.Create("Confirm", "Are you sure you want to delete " + MediaObject.FileName, UIAlertControllerStyle.Alert);
            alert.AddAction(actionCancel);
            alert.AddAction(actionOk);
            PresentViewController(alert, true,null);
        }

        partial void Duplicate__TouchUpInside(UIButton sender)
        {
            App.MediaService.DuplicateMediaObject(MediaObject);
            controller.GetMedia();
        }

        partial void Save_TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }

        partial void Move_TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }
    }
}



