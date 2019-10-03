using System;
using System.IO;
using System.Collections.Generic;


#if __IOS__
using UIKit;
using AVFoundation;
using CoreGraphics;
using Foundation;
#endif


namespace DancePro.Models
{
    public enum MediaTypes{ 
        Image,
        Video,
        Audio,
        Folder,
        Other

    };


    public class MediaObject : IMediaObject
    {

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateCreated { get; set; }
        public MediaTypes MediaType { get; set; }


#if __IOS__
        public UIImage Thumb { get; set; }
        public NSString SegueString { get; protected set; }

        public MediaObject(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            DateCreated = File.GetCreationTime(filePath);
            Thumb = UIImage.FromBundle("Folder");
            SegueString = new NSString("Folder");
            MediaType = MediaTypes.Folder;
        }

        public virtual UIView GetDetailView(UIViewController mainController)
        {
            throw new NotImplementedException();
        }

        public virtual bool SaveToCameraRoll() {
            return false;
        }

#endif

#if __ANDROID__
        public int ResourceID { get; private set; }

        public MediaObject(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            DateCreated = File.GetCreationTime(filePath);
            MediaType = MediaTypes.Other;
        }

#endif



    }
}
