using System;

#if __IOS__
using AVFoundation;
using Foundation;
using MediaPlayer;
#endif

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
            SegueString = new NSString("VideoSegue");
            MediaType = MediaTypes.Video;

        }

        /// <summary>
        /// Thumb Generator method to control the setting of the objects thumbnail.
        /// Handles if the object fails to create a thumb and sets error image (possible corrupt file).
        /// </summary>
        /// <returns>A UIImage object that represents the thumbnail to be used by the collection view</returns>
        public override UIKit.UIImage GetThumb()
        {
            try
            {
                return Services.Thumbnail.GenerateThumbImage(Asset);
            }
            catch
            {
                Console.WriteLine("Error Loading Video Thumbnail: " + FileName);
                return UIKit.UIImage.FromBundle("Error");
            }

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
