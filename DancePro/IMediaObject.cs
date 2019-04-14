using System;
using DancePro.Models;


#if __IOS__
using UIKit;
#endif


namespace DancePro
{
    public interface IMediaObject
    {

        string FileName { get; set; }
        string FilePath { get; set; }
        DateTime DateCreated { get; set; }
        MediaTypes MediaType { get; set; }

#if __IOS__
        UIImage Thumb { get; set; }
        UIView GetDetailView(UIViewController mainController);
        bool SaveToCameraRoll();
#endif

#if __ANDROID__



#endif
    }
}
