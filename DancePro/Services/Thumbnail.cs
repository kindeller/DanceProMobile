using System;

#if __ANDROID__
using System.IO;
using Android.Graphics;
using Android.Media;
using System.Collections.Generic;
#endif

#if __IOS__
using UIKit;
using ImageIO;
using AVFoundation;
using CoreMedia;
using Foundation;
using CoreGraphics;
#endif

namespace DancePro.Services
{
    public static class Thumbnail
    {

#if __IOS__
        public static UIImage GetImageThumbnail(UIImage image,int width, int height, int quality)
        {
            return PhotoResizer.ResizeImageIOS(image, width, height, quality);
        }

        public static UIImage GetImageThumbnail(byte[] bytes,int width, int height, int quality)
        {
            return PhotoResizer.ImageFromByteArray(PhotoResizer.ResizeImageIOS(bytes, width, height, quality));
        }

        public static CGImageSource GenerateThumbImage(string url, long usecond)
        {
            AVAssetImageGenerator imageGenerator = new AVAssetImageGenerator(AVAsset.FromUrl((new Foundation.NSUrl(url))));
            imageGenerator.AppliesPreferredTrackTransform = true;
            CMTime actualTime;
            NSError error;
            CGImage cgImage = imageGenerator.CopyCGImageAtTime(new CMTime(usecond, 1000000), out actualTime, out error);
            return CGImageSource.FromData((NSData.FromStream(new UIImage(cgImage).AsPNG().AsStream())));
        }

        public static UIImage GenerateThumbImage(AVAsset asset)
        {
            AVAssetImageGenerator imageGenerator = new AVAssetImageGenerator(asset);
            imageGenerator.AppliesPreferredTrackTransform = true;
            CMTime actualTime;
            NSError error;

            long time = (long)Math.Floor(asset.Duration.Seconds / 2);
            CGImage cgImage = imageGenerator.CopyCGImageAtTime(new CMTime(time, 1000000), out actualTime, out error);
            return new UIImage(cgImage);
        }
#endif

#if __ANDROID__
        //public ImageSource GenerateThumbImage(string url, long usecond)
        //{   
        //    MediaMetadataRetriever retriever = new MediaMetadataRetriever();
        //    retriever.SetDataSource(url, new Dictionary<string, string>());
        //    Bitmap bitmap = retriever.GetFrameAtTime(usecond);
        //    if (bitmap != null)
        //    {
        //        MemoryStream stream = new MemoryStream();
        //        bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
        //        byte[] bitmapData = stream.ToArray();
        //        return ImageSource.FromStream(() => new MemoryStream(bitmapData));
        //    }
        //    return null;
        //}

#endif
    }
}
