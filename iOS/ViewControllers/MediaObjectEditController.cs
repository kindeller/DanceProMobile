using System;
using System.Collections.Generic;
using AssetsLibrary;
using AVFoundation;
using DancePro.Models;
using Foundation;
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

            View.AddGestureRecognizer(new UITapGestureRecognizer((e) => {
                DismissViewController(true, null);
            }));

            SetUpButtons();
        }

        public void SetUpButtons()
        {
            var inset = 5;
            var buttonWidth = ShareButton.Frame.Width - (inset*2);
            var buttonHeight = ShareButton.Frame.Height - (inset*5);

            //Save Button
            //UIImageView view = new UIImageView(UIImage.FromBundle("Icon_Save"));
            //view.ContentMode = UIViewContentMode.ScaleAspectFit;
            //view.Frame = new CoreGraphics.CGRect(inset, inset, buttonWidth, buttonHeight);
            //SaveButton.AddSubview(view);

            //Move Button Set Up
            //view = new UIImageView(UIImage.FromBundle("Icon_Move"));
            //view.ContentMode = UIViewContentMode.ScaleAspectFit;
            //view.Frame = new CoreGraphics.CGRect(inset, inset, buttonWidth, buttonHeight);
            //MoveButton.AddSubview(view);

            //Share Button Set Up
            UIImageView view = new UIImageView(UIImage.FromBundle("Icon_Share"));
            view.ContentMode = UIViewContentMode.ScaleAspectFit;
            view.Frame = new CoreGraphics.CGRect(inset, inset, buttonWidth, buttonHeight);
            ShareButton.AddSubview(view);

            //Delete Button Set Up
            view = new UIImageView(UIImage.FromBundle("Icon_Delete"));
            view.ContentMode = UIViewContentMode.ScaleAspectFit;
            view.Frame = new CoreGraphics.CGRect(inset, inset, buttonWidth, buttonHeight);
            DeleteButton.AddSubview(view);

            //Duplicate Button Set Up
            view = new UIImageView(UIImage.FromBundle("Icon_Duplicate"));
            view.ContentMode = UIViewContentMode.ScaleAspectFit;
            view.Frame = new CoreGraphics.CGRect(inset, inset, buttonWidth, buttonHeight);
            DuplicateButton.AddSubview(view);

            //Rename Button Set Up
            view = new UIImageView(UIImage.FromBundle("Icon_Rename"));
            view.ContentMode = UIViewContentMode.ScaleAspectFit;
            view.Frame = new CoreGraphics.CGRect(inset, inset, buttonWidth, buttonHeight);
            RenameButton.AddSubview(view);
        }

        private void DismissEditMenu()
        {
            DismissViewController(true, null);
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
                PresentViewController(alert, true, DismissEditMenu);
            }
            else
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                vc.PresentViewController(alert, true, DismissEditMenu);
            }
        }

        partial void Share_TouchUpInside(UIButton sender)
        {
            UIActivityViewController activity = new UIActivityViewController(new NSObject[] { null }, null);
            var items = new NSObject[0];
            switch (MediaObject.MediaType)
            {
                case MediaTypes.Image:
                    ImageObject imageObject = MediaObject as ImageObject;
                    if (imageObject != null)
                    {
                        items = new[] { UIImage.FromFile(imageObject.FilePath) };
                    }
                    break;
                case MediaTypes.Audio:
                    AudioObject audioObject = MediaObject as AudioObject;
                    if (audioObject != null)
                    {
                        NSUrl url = NSUrl.CreateFileUrl(new[] { audioObject.FilePath });
                        items = new[] { url };
                    }
                    break;
                case MediaTypes.Video:
                    VideoObject videoObject = MediaObject as VideoObject;
                    if (videoObject != null)
                    {
                        NSUrl url = NSUrl.CreateFileUrl(new[] { videoObject.FilePath });
                        items = new[] { url };
                    }
                    break;
                case MediaTypes.Folder:
                    UIAlertController controller = UIAlertController.Create("Save Folder?", "Would you like to save the folder contents to camera roll? (Video and Image Only)", UIAlertControllerStyle.Alert);
                    controller.AddAction(UIAlertAction.Create("Save", UIAlertActionStyle.Default, (obj) => {
                        var list = App.MediaService.GetMediaFromFolder(MediaObject.FilePath);
                        foreach(var mo in list)
                        {
                            SaveMediaItemToCamera(mo);   
                        }
                    }));
                    controller.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    PresentAlert(controller);
                    return;
            }
            activity = new UIActivityViewController(items,null);
            activity.ExcludedActivityTypes = new[]
            {
                UIActivityType.AirDrop,
                //UIActivityType.SaveToCameraRoll, Remove to disable Save to Camera Roll
                UIActivityType.CopyToPasteboard,
                UIActivityType.OpenInIBooks,
                new NSString("com.apple.CloudDocsUI.AddToiCloudDrive"),
                //new NSString("com.apple.mobilenotes.SharingExtension"),
                //new NSString("com.apple.reminders.RemindersEditorExtension")
            };
            PresentViewController(activity, true, null);
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
                var isSuccess = (MediaObject.MediaType == MediaTypes.Folder) ? App.MediaService.RenameFolder(MediaObject, alert.TextFields[0].Text) : App.MediaService.RenameMediaObject(MediaObject, alert.TextFields[0].Text);
                if (isSuccess)
                {
                    controller.GetMedia();
                    DismissEditMenu();
                }
                else
                {
                    var a = UIAlertController.Create("Failed", "Please try again with a different name.", UIAlertControllerStyle.Alert);
                    a.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                    PresentViewController(a, true, null);
                }

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
                isSuccess = MediaObject.MediaType == MediaTypes.Folder ? App.MediaService.DeleteFolder(MediaObject) : App.MediaService.DeleteMediaObject(MediaObject);
                controller.GetMedia();
                DismissEditMenu();
            });

            var Message = (MediaObject.MediaType == MediaTypes.Folder) ? "Are you sure you want to delete the folder \"" + MediaObject.FileName + "\" and all it's contents?" : "Are you sure you want to delete the file " + MediaObject.FileName;
            var alert = UIAlertController.Create("WARNING!",Message, UIAlertControllerStyle.Alert);
            alert.AddAction(actionCancel);
            alert.AddAction(actionOk);
            PresentViewController(alert, true, null);
        }

        partial void Duplicate__TouchUpInside(UIButton sender)
        {
            App.MediaService.DuplicateMediaObject(MediaObject);
            controller.GetMedia();
            DismissViewController(true, null);

        }

        private void SaveMediaItemToCamera(MediaObject obj)
        {
            switch (obj.MediaType)
            {
                case MediaTypes.Image:
                    try
                    {
                        ImageObject Image = obj as ImageObject;
                        if (Image != null)
                        {
                            new ALAssetsLibrary().WriteImageToSavedPhotosAlbum(UIImage.FromFile(Image.FilePath).CGImage, ALAssetOrientation.Up, (arg1, arg2) => { });
                        }
                    }
                    catch (Exception e)
                    {
                        var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null);
                        Alert("Error Saving!", e.Message, new List<UIAlertAction>() { actionOk });
                        Console.WriteLine(e.Message);
                    }

                    break;
                case MediaTypes.Video:
                    try
                    {
                        VideoObject Video = obj as VideoObject;
                        if (Video != null)
                        {
                            NSUrl url = NSUrl.FromFilename(Video.FilePath);
                            new ALAssetsLibrary().WriteVideoToSavedPhotosAlbum(
                                url, (arg1, arg2) => {

                                }
                                );
                        }
                    }
                    catch (Exception e)
                    {
                        var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null);
                        Alert("Error Saving!", e.Message, new List<UIAlertAction>() { actionOk });
                        Console.WriteLine(e.Message);
                    }

                    break;
                default:
                    //Alert("Save Failed!", "Cannot save this item to device.", new List<UIAlertAction>() { UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null) });
                    Console.WriteLine("Cannot Save this type of media.");
                    break;
            }
        }

        //partial void Save_TouchUpInside(UIButton sender)
        //{
        //    switch (MediaObject.MediaType)
        //    {
        //        case MediaTypes.Image:
        //            try
        //            {
        //                ImageObject Image = MediaObject as ImageObject;
        //                if (Image != null)
        //                {
        //                    new ALAssetsLibrary().WriteImageToSavedPhotosAlbum(UIImage.FromFile(Image.FilePath).CGImage, AssetsLibrary.ALAssetOrientation.Up, (arg1, arg2) => { });
        //                }
        //            }
        //            catch(Exception e)
        //            {
        //                var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null);
        //                Alert("Error Saving!", e.Message, new List<UIAlertAction>() { actionOk });
        //                Console.WriteLine(e.Message);
        //            }

        //            break;
        //        case MediaTypes.Video:
        //            try
        //            {
        //                VideoObject Video = MediaObject as VideoObject;
        //                if (Video != null)
        //                {
        //                    NSUrl url = NSUrl.FromFilename(Video.FilePath);
        //                    new AssetsLibrary.ALAssetsLibrary().WriteVideoToSavedPhotosAlbum(
        //                        url, (arg1, arg2) => {

        //                        }
        //                        );
        //                }
        //            }catch(Exception e)
        //            {
        //                var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null);
        //                Alert("Error Saving!", e.Message, new List<UIAlertAction>() { actionOk });
        //                Console.WriteLine(e.Message);
        //            }

        //            break;
        //        default:
        //            Alert("Save Failed!", "Cannot save this item to device.", new List<UIAlertAction>() { UIAlertAction.Create("Ok",UIAlertActionStyle.Cancel,null) });
        //            Console.WriteLine("Cannot Save this type of media.");
        //            break;
        //    }
        //}

        //partial void Move_TouchUpInside(UIButton sender)
        //{
        //    throw new NotImplementedException();
        //}

        private UIAlertController Alert(string title, string message, List<UIAlertAction> actions)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            if(actions != null)
            {
                foreach (var action in actions)
                {
                    alert.AddAction(action);
                }
            }
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            vc.PresentViewController(alert, true, null);
            return alert;
        }
    }
}



