// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace DancePro.iOS
{
    [Register ("MenuViewController")]
    partial class MenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MediaButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PhotosButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton VideosButton { get; set; }

        [Action ("UIButton3099_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton3099_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton3100_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton3100_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (MediaButton != null) {
                MediaButton.Dispose ();
                MediaButton = null;
            }

            if (PhotosButton != null) {
                PhotosButton.Dispose ();
                PhotosButton = null;
            }

            if (VideosButton != null) {
                VideosButton.Dispose ();
                VideosButton = null;
            }
        }
    }
}