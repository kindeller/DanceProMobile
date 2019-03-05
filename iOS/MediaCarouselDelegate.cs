using System;
using DancePro.iOS.ViewControllers;
using UIKit;
using Xamarin.iOS.iCarouselBinding;

namespace DancePro.iOS
{
    public class MediaCarouselDelegate : iCarouselDelegate
    {
        private readonly MediaObjectViewController _viewController;
        public MediaCarouselDelegate(MediaObjectViewController vc)
        {
            _viewController = vc;
        }

        public override void DidSelectItemAtIndex(iCarousel carousel, nint index)
        {
            base.DidSelectItemAtIndex(carousel, index);

            //TODO: Impliment delegate to handle action from clicking on MediaObject IE swap views
        }
    }
}
