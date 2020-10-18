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
            Layer.BorderColor = UIColor.DarkGray.CGColor;
            Title.Text = System.IO.Path.GetFileNameWithoutExtension(mediaObject.FileName);
            DateLabel.Text = mediaObject.DateCreated.ToShortDateString();
            ThumbImage.Image = MediaObject.GetThumb();
            ThumbImage.ContentMode = UIViewContentMode.ScaleToFill;

            if(mediaObject.MediaType == MediaTypes.Other)
            {
                Title.Text = "Back: " + MediaObject.FileName;
                ThumbImage.Image = UIImage.FromBundle("Back");
                MenuButton.Enabled = false;
                MenuButton.Hidden = true;
                DateLabel.Enabled = false;
                DateLabel.Hidden = true;
                Title.TextAlignment = UITextAlignment.Center;
                BackgroundColor = UIColor.SystemBackgroundColor;
            }
            else
            {
                MenuButton.Enabled = true;
                MenuButton.Hidden = false;
                DateLabel.Enabled = true;
                DateLabel.Hidden = false;
                Title.TextAlignment = UITextAlignment.Left;
                BackgroundColor = UIColor.FromName("CellBgColor");
            }

            if(mediaObject.MediaType == MediaTypes.Folder)
            {
                optionalLabel.Text = $"({System.IO.Directory.GetFiles(mediaObject.FilePath).Length})";
                ThumbImage.ContentMode = UIViewContentMode.ScaleAspectFit;
            }
            else
            {
                optionalLabel.Text = "";
            }
        }

        partial void MediaCellMenu_UpInside(UIButton sender)
        {
            Controller.PerformSegue("ShowMenu", this);
        }
    }
}