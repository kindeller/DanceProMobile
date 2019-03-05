using System;
using System.Collections.Generic;
using DancePro.ViewModels;
using Foundation;
using DancePro.Services;
using DancePro.Models;

using UIKit;
using System.IO;

namespace DancePro.iOS.ViewControllers
{
    public partial class MyMediaViewController : UIViewController
    {
        FileTransferService service = new FileTransferService();
        List<MediaObject> MediaObjects = new List<MediaObject>();

        public MyMediaViewController(IntPtr intPtr) : base(intPtr)
        {

        }

        partial void OnConnectSwitchChanged(UISwitch sender)
        {
            if (sender.On)
            {
                List<UIAlertAction> actions = new List<UIAlertAction>();
                var action = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => { 
                service.Disconnect(); 
                sender.On = false;
                    GetMedia();
                ReloadMediaList();
                 });
                actions.Add(action);
                service.Connect();
                Alert("Connecting...", $"Enter: http://localhost:{service.Port}", actions);
            }
            else
            {
                service.Disconnect();
                GetMedia();
                ReloadMediaList();
            }
        }


        private void Alert(string title, string message, List<UIAlertAction> actions)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            foreach (var action in actions)
            {
                alert.AddAction(action);
            }
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            vc.PresentViewController(alert, true, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            //MediaCollectionView.Delegate = new MyMediaViewDelegate();
            GetMedia();
            ReloadMediaList();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void GetMedia()
        {
            MediaObjects.Clear();
            //TODO: Update this to use app wide service
            MediaObjects = new MediaService().GetMediaFromFolder("./Media/");

        }

        private void GetMedia(string Path)
        {
            MediaObjects.Clear();
            MediaObjects = new MediaService().GetMediaFromFolder(Path);
        }

        private void ReloadMediaList()
        {
            if(MediaObjects.Count > 0)
            {
                MyMediaUICollectionSource source = new MyMediaUICollectionSource(MediaObjects, this);
                MediaCollectionView.ShowsVerticalScrollIndicator = false;
                MediaCollectionView.Source = source;
            }

        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

                IMediaObjectController controller = segue.DestinationViewController as IMediaObjectController;
                if (controller != null)
                {
                    var cell = (MyMediaViewCell)sender;
                    if (cell != null)
                    {
                    switch (cell.MediaObject.MediaType)
                    {
                        case MediaTypes.Image:
                            controller.MediaObject = (ImageObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            break;
                        case MediaTypes.Audio:
                            controller.MediaObject = (AudioObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            break;
                        case MediaTypes.Video:
                            controller.MediaObject = (VideoObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            break;
                    }


                    }
                }

            MediaObjectEditController c = segue.DestinationViewController as MediaObjectEditController;

            if (c != null)
            {
                var cell = (MyMediaViewCell)sender;
                if(cell != null)
                {
                    c.MediaObject = cell.MediaObject;
                }


            }


        }

        public void UpdateMediaPath(string Path)
        {
            GetMedia(Path);
            ReloadMediaList();
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            GetMedia();
            ReloadMediaList();
        }
    }
}

