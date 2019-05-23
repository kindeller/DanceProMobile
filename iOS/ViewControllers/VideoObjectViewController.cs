using System;
using System.Collections.Generic;
using DancePro.Models;
using UIKit;
using AVKit;
using AVFoundation;
using Foundation;

namespace DancePro.iOS.ViewControllers
{
    public partial class VideoObjectViewController : AVPlayerViewController, IMediaObjectController
    {
        public List<MediaObject> MediaList { get; set; }
        public MediaObject MediaObject { get; set; }

        public VideoObjectViewController(IntPtr intPtr) : base(intPtr)
        {
            MediaList = new List<MediaObject>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            AppDelegate.CanRotate = true;
            InitialisePlayer();
        }


        private void InitialisePlayer()
        {
            var video = (VideoObject)MediaObject;
            Player = new AVPlayer(video.PlayerItem);
            NavigationController.NavigationBar.Hidden = true;
            TabBarController.TabBar.Hidden = true;
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            TabBarController.TabBar.Hidden = false;
            NavigationController.NavigationBar.Hidden = false;
        }


        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Player.Pause();
            Player.Dispose();
            Player = null;
            AppDelegate.CanRotate = false;
        }
    }
}

