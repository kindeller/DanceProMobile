using System;
using System.IO;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;

#if __IOS__
using UIKit;
using AVFoundation;
#endif

namespace DancePro.Models
{
    public enum MediaTypes{ 
        Image,
        Video,
        Audio,
        Other

    };


    public class MediaObject : IMediaObject
    {

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateCreated { get; set; }
        public MediaTypes MediaType { get; set; }
        public UIImage Thumb { get; set; }

#if __IOS__

        public NSString SegueString { get; protected set; }

        public MediaObject(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            DateCreated = File.GetCreationTime(filePath);
            Thumb = UIImage.FromBundle("Folder");
            SegueString = new NSString("Folder");
            MediaType = MediaTypes.Other;
        }

        public virtual UIView GetDetailView(UIViewController mainController)
        {
            throw new NotImplementedException();
        }


#endif



    }
}
