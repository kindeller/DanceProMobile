using CoreGraphics;
using DancePro.iOS.ViewControllers;
using DancePro.Models;
using Foundation;
using System;
using System.Drawing;
using UIKit;

namespace DancePro.iOS
{

    public partial class MyMediaViewCell : UICollectionViewCell
    {
        public MyMediaViewController Controller { get; private set; }
        public MediaObject MediaObject { get; private set; }


        public static NSString CellID = new NSString("MyMediaViewCell");


        public MyMediaViewCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateRow(MediaObject mediaObject, MyMediaViewController controller)
        {
            Controller = controller;
            MediaObject = mediaObject;
            BackgroundColor = UIColor.LightGray;
            Layer.BorderColor = UIColor.DarkGray.CGColor;
            Title.Text = mediaObject.FileName;
            DateLabel.Text = mediaObject.DateCreated.ToShortDateString();
            ThumbImage.Image = mediaObject.Thumb;
            ThumbImage.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        partial void MediaCellMenu_UpInside(UIButton sender)
        {
            Controller.PerformSegue("ShowMenu", this);
        }
    }
}