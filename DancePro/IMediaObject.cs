using System;
using DancePro.Models;
using UIKit;

namespace DancePro
{
    public interface IMediaObject
    {

        string FileName { get; set; }
        string FilePath { get; set; }
        DateTime DateCreated { get; set; }
        MediaTypes MediaType { get; set; }
        UIImage Thumb { get; set; }


        UIView GetDetailView(UIViewController mainController);
    }
}
