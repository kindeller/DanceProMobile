using System;
using System.Drawing;
using Foundation;
using UIKit;
using DancePro.Models;

namespace DancePro.iOS.ViewControllers
{
    public class MyMediaViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("MyMediaCell");
        public UIImageView ImageView { get; private set; }
        public UILabel LabelView { get; private set; }

        [Export("initWithFrame:")]
        public MyMediaViewCell(RectangleF frame) : base(frame)
        {
            
            BackgroundColor = UIColor.Black;

            ImageView = new UIImageView();
            ImageView.Layer.BorderColor = UIColor.DarkGray.CGColor;
            ImageView.Layer.BorderWidth = 1f;
            ImageView.Layer.CornerRadius = 3f;
            ImageView.Layer.MasksToBounds = true;
            ImageView.ContentMode = UIViewContentMode.ScaleToFill;

            ContentView.AddSubview(ImageView);

            LabelView = new UILabel();
            LabelView.BackgroundColor = UIColor.Clear;
            LabelView.TextColor = UIColor.DarkGray;
            LabelView.TextAlignment = UITextAlignment.Center;

            ContentView.AddSubview(LabelView);
        }

        public void UpdateRow(MediaObject mediaObject, float fontSize, SizeF imageViewSize)
        {
            LabelView.Text = mediaObject.FileName;
            ImageView.Image = mediaObject.Image;

            LabelView.Font = UIFont.FromName("HelveticaNeue-Bold", fontSize);

            ImageView.Frame = new RectangleF(0, 0, imageViewSize.Width, imageViewSize.Height);
            LabelView.Frame = new RectangleF(0, (float)ImageView.Frame.Bottom, imageViewSize.Width,
                                             (float)ContentView.Frame.Height - (float)ImageView.Frame.Bottom);
        }
    }
}
