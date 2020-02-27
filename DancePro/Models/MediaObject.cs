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

        public string GetMimeType()
        {
            string MimeType;
            mimeTypes.TryGetValue(Path.GetExtension(FilePath), out MimeType);
            return MimeType;
        }

        private static readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".asf", "video/x-ms-asf"},
            {".asx", "video/x-ms-asf"},
            {".avi", "video/x-msvideo"},
            {".bin", "application/octet-stream"},
            {".cco", "application/x-cocoa"},
            {".crt", "application/x-x509-ca-cert"},
            {".css", "text/css"},
            {".deb", "application/octet-stream"},
            {".der", "application/x-x509-ca-cert"},
            {".dll", "application/octet-stream"},
            {".dmg", "application/octet-stream"},
            {".ear", "application/java-archive"},
            {".eot", "application/octet-stream"},
            {".exe", "application/octet-stream"},
            {".flv", "video/x-flv"},
            {".gif", "image/gif"},
            {".hqx", "application/mac-binhex40"},
            {".htc", "text/x-component"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".img", "application/octet-stream"},
            {".iso", "application/octet-stream"},
            {".jar", "application/java-archive"},
            {".jardiff", "application/x-java-archive-diff"},
            {".jng", "image/x-jng"},
            {".jnlp", "application/x-java-jnlp-file"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".js", "application/x-javascript"},
            {".mml", "text/mathml"},
            {".mng", "video/x-mng"},
            {".mov", "video/quicktime"},
            {".mp3", "audio/mpeg"},
            {".mp4", "video/mp4"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".msi", "application/oc" +
                "tet-stream"},
            {".msm", "application/octet-stream"},
            {".msp", "application/octet-stream"},
            {".pdb", "application/x-pilot"},
            {".pdf", "application/pdf"},
            {".pem", "application/x-x509-ca-cert"},
            {".pl", "application/x-perl"},
            {".pm", "application/x-perl"},
            {".png", "image/png"},
            {".prc", "application/x-pilot"},
            {".ra", "audio/x-realaudio"},
            {".rar", "application/x-rar-compressed"},
            {".rpm", "application/x-redhat-package-manager"},
            {".rss", "text/xml"},
            {".run", "application/x-makeself"},
            {".sea", "application/x-sea"},
            {".shtml", "text/html"},
            {".sit", "application/x-stuffit"},
            {".swf", "application/x-shockwave-flash"},
            {".tcl", "application/x-tcl"},
            {".tk", "application/x-tcl"},
            {".txt", "text/plain"},
            {".war", "application/java-archive"},
            {".wbmp", "image/vnd.wap.wbmp"},
            {".wmv", "video/x-ms-wmv"},
            {".xml", "text/xml"},
            {".xpi", "application/x-xpinstall"},
            {".zip", "application/zip"},
            {".map", "application/json"}
        };

    }
}
