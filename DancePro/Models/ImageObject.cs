using System;
using System.IO;


#if __IOS__
using UIKit;
using AVFoundation;
using CoreGraphics;
using Foundation;
#endif

namespace DancePro.Models
{
    public class ImageObject : MediaObject
    {


#if __IOS__
        public ImageObject(string filePath) : base(filePath)
        {
            MediaType = MediaTypes.Image;
            SegueString = new NSString("ImageSegue");
        }

        public override UIImage GetThumb() { return UIImage.FromFile(FilePath).Scale(new CGSize(UIImage.FromFile(FilePath).Size.Width / 5, UIImage.FromFile(FilePath).Size.Height / 5)); }

        public override UIView GetDetailView(UIViewController mainController)
        {
            CGRect rect = new CGRect(0, 0, mainController.View.Frame.Width, mainController.View.Frame.Height);
            UIImageView ImageView = new UIImageView(rect);
            ImageView.Image = UIImage.FromFile(FilePath);
            UIScrollView ScrollView = new UIScrollView(rect);
            //ScrollView.AddSubview(ImageView);
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ScrollView.ContentSize = ImageView.Image.Size;
            //ScrollView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ScrollView.MinimumZoomScale = 1f;
            ScrollView.MaximumZoomScale = 10f;
            ScrollView.ViewForZoomingInScrollView += (view) => { return ImageView; };
            ScrollView.AddSubview(ImageView);
            ScrollView.DidZoom += (object sender, EventArgs e) => {
                //AdjustImage();
            };
            ScrollView.AddGestureRecognizer(new UITapGestureRecognizer((e) => {

                //TODO: Perform Hide and reveal of Image UI
                //ToolbarsView.Hidden = !ToolbarsView.Hidden;
                mainController.NavigationController.NavigationBar.Hidden = !mainController.NavigationController.NavigationBar.Hidden;
                mainController.TabBarController.TabBar.Hidden = !mainController.TabBarController.TabBar.Hidden;

            }));
            return ScrollView;
        }
        
#endif

#if __ANDROID__

        public ImageObject(string filePath) : base(filePath)
        {
            MediaType = MediaTypes.Image;

        }

#endif


    }
}