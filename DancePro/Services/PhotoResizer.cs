using System;
using System.Drawing;
using System.IO;

#if __ANDROID__
using Android.Graphics;
#endif

#if __IOS__
using UIKit;
#endif

namespace DancePro.Services
{
    public static class PhotoResizer
    {
        public static byte[] ResizeImage(byte[] imageData, float width, float height, int quality)
        {
#if __IOS__
            return ResizeImageIOS(imageData, width, height, quality);
#endif
#if __ANDROID__
            return ResizeImageAndroid ( imageData, width, height, quality );
#endif
#if WINDOWS_PHONE
            return ResizeImageWinPhone ( imageData, width, height, quality );
#endif
        }


#if __IOS__
        public static UIImage ResizeImageIOS(UIImage image, float width, float height, int quality)
        {
            UIImage originalImage = image;

            float oldWidth = (float)originalImage.Size.Width;
            float oldHeight = (float)originalImage.Size.Height;
            float scaleFactor = 0f;

            if (oldWidth > oldHeight)
            {
                scaleFactor = width / oldWidth;
            }
            else
            {
                scaleFactor = height / oldHeight;
            }

            float newHeight = oldHeight * scaleFactor;
            float newWidth = oldWidth * scaleFactor;

            //create a 24bit RGB image
            using (CoreGraphics.CGBitmapContext context = new CoreGraphics.CGBitmapContext(IntPtr.Zero,
                (int)newWidth, (int)newHeight, 8,
                (int)(4 * newWidth), CoreGraphics.CGColorSpace.CreateDeviceRGB(),
                CoreGraphics.CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, newWidth, newHeight);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage());

                // save the image as a jpeg
                return resizedImage;
            }
        }

        public static byte[] ResizeImageIOS(byte[] imageData, float width, float height, int quality)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            UIImage thumb = ResizeImageIOS(originalImage, width, height, quality);
            var bytes = thumb.AsJPEG((float)quality).ToArray();
            return bytes;


        }

        public static UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIImage image;
            try
            {
                image = new UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }
#endif

#if __ANDROID__

        public static byte[] ResizeImageAndroid (byte[] imageData, float width, float height, int quality)
        {
        // Load the bitmap
        Android.Graphics.Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);

        float oldWidth = (float)originalImage.Width;
        float oldHeight = (float)originalImage.Height;
        float scaleFactor = 0f;

        if (oldWidth > oldHeight)
        {
            scaleFactor = width / oldWidth;
        }
        else
        {
            scaleFactor = height / oldHeight;
        }

        float newHeight = oldHeight * scaleFactor;
        float newWidth = oldWidth * scaleFactor;

        Android.Graphics.Bitmap resizedImage = Android.Graphics.Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

        using (MemoryStream ms = new MemoryStream())
        {
        resizedImage.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, quality, ms);
        return ms.ToArray ();
        }
        }

#endif

#if WINDOWS_PHONE

        public static byte[] ResizeImageWinPhone (byte[] imageData, float width, float height)
        {
        byte[] resizedData;

        using (MemoryStream streamIn = new MemoryStream (imageData))
        {
        WriteableBitmap bitmap = PictureDecoder.DecodeJpeg (streamIn, (int)width, (int)height);

        using (MemoryStream streamOut = new MemoryStream ())
        {
        bitmap.SaveJpeg(streamOut, (int)width, (int)height, 0, 100);
        resizedData = streamOut.ToArray();
        }
        }
        return resizedData;
        }

#endif
    }
}
