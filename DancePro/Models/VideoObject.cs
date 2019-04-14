using System;
using AVFoundation;
using Foundation;
using MediaPlayer;

namespace DancePro.Models
{
    public class VideoObject : MediaObject
    {
#if __IOS__
        public AVAsset Asset { get; private set; }
        public AVPlayerItem PlayerItem { get; private set; }

        public VideoObject(string filePath) : base(filePath)
        {
            var url = NSUrl.FromFilename(filePath);
            Asset = AVAsset.FromUrl(url);
            PlayerItem = new AVPlayerItem(Asset);
            Thumb = Services.Thumbnail.GenerateThumbImage(Asset);
            SegueString = new NSString("VideoSegue");
            MediaType = MediaTypes.Video;

        }
#endif

#if __ANDROID__
        
        public VideoObject(string filePath) : base(filePath)
        {
            MediaType = MediaTypes.Video;
        }
#endif
    }
}
