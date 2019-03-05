using System;
using System.Collections.Generic;
using DancePro.Models;
using UIKit;
using AVKit;
using AVFoundation;

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


            InitialisePlayer();

       }

        private void InitialisePlayer()
        {
            var video = (VideoObject)MediaObject;
            Player = new AVPlayer(video.PlayerItem);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

