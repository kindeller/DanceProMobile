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
    public UIImage Image { get; set; }
        public ImageObject(string filePath) : base(filePath)
        {
            MediaType = MediaTypes.Image;
            Image = UIImage.FromFile(filePath);
            Thumb = Image?.Scale(new CGSize(Image.Size.Width/10,Image.Size.Height/10));
            SegueString = new NSString("ImageSegue");
        }


        public override UIView GetDetailView(UIViewController mainController)
        {
            CGRect rect = new CGRect(0, 0, mainController.View.Frame.Width, mainController.View.Frame.Height);
            UIImageView ImageView = new UIImageView(rect);
            ImageView.Image = Image;
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