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
            BackgroundColor = UIColor.FromRGB(220, 220, 220);
            Layer.BorderColor = UIColor.DarkGray.CGColor;
            Title.Text = mediaObject.FileName;
            DateLabel.Text = mediaObject.DateCreated.ToShortDateString();
            ThumbImage.Image = mediaObject.Thumb;
            ThumbImage.ContentMode = UIViewContentMode.ScaleAspectFit;

            if(mediaObject.MediaType == MediaTypes.Other)
            {
                MenuButton.Enabled = false;
                MenuButton.Hidden = true;
                DateLabel.Enabled = false;
                DateLabel.Hidden = true;
                Title.TextAlignment = UITextAlignment.Center;
            }
            else
            {
                MenuButton.Enabled = true;
                MenuButton.Hidden = false;
                DateLabel.Enabled = true;
                DateLabel.Hidden = false;
                Title.TextAlignment = UITextAlignment.Left;
            }
        }

        partial void MediaCellMenu_UpInside(UIButton sender)
        {
            Controller.PerformSegue("ShowMenu", this);
        }
    }
}