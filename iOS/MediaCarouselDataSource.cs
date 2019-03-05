using System;
using UIKit;
using Xamarin.iOS.iCarouselBinding;
using System.Collections.Generic;
using DancePro.Models;

namespace DancePro.iOS
{
    public class MediaCarouselDataSource : iCarouselDataSource
    {
    
        List<MediaObject> CarouselMediaList { get; set; }

        public MediaCarouselDataSource()
        {
        }

        public override nint NumberOfItemsInCarousel(iCarousel carousel)
        {
            return CarouselMediaList.Count;
        }

        public override UIView ViewForItemAtIndex(iCarousel carousel, nint index, UIView view)
        {
            throw new NotImplementedException();
        }
    }
}
