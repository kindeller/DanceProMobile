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

namespace DancePro.iOS.ViewControllers
{
    [Register ("MediaObjectEditController")]
    partial class MediaObjectEditController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DeleteButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DuplicateButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RenameButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShareButton { get; set; }

        [Action ("Cancel_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Cancel_TouchUpInside (UIKit.UIButton sender);

        [Action ("Delete__TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Delete__TouchUpInside (UIKit.UIButton sender);

        [Action ("Duplicate__TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Duplicate__TouchUpInside (UIKit.UIButton sender);

        [Action ("Rename_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Rename_TouchUpInside (UIKit.UIButton sender);

        [Action ("Share_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Share_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (DeleteButton != null) {
                DeleteButton.Dispose ();
                DeleteButton = null;
            }

            if (DuplicateButton != null) {
                DuplicateButton.Dispose ();
                DuplicateButton = null;
            }

            if (RenameButton != null) {
                RenameButton.Dispose ();
                RenameButton = null;
            }

            if (ShareButton != null) {
                ShareButton.Dispose ();
                ShareButton = null;
            }
        }
    }
}