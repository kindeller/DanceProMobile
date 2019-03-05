using System;
using DancePro.Models;
using UIKit;

namespace DancePro.iOS.ViewControllers
{
    public partial class MediaObjectEditController : UIViewController
    {

        public MediaObject MediaObject { get; set; }

        public MediaObjectEditController() : base("MediaObjectEditController", null)
        {
        }

        public MediaObjectEditController(IntPtr intPtr) : base(intPtr)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            View.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void Cancel_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }

        partial void Rename_TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }

        partial void Delete__TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }

        partial void Duplicate__TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }

        partial void Save_TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }

        partial void Move_TouchUpInside(UIButton sender)
        {
            throw new NotImplementedException();
        }
    }
}

