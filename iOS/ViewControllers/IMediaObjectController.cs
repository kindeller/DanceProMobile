using System;
using UIKit;
using System.Collections.Generic;
using DancePro.Models;
namespace DancePro.iOS.ViewControllers
{
    public interface IMediaObjectController
    {

        List<MediaObject> MediaList { get; set; }
        MediaObject MediaObject { get; set; }


    }
}
