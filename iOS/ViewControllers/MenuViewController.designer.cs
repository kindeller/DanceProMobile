﻿// WARNING
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
        UIKit.UIView MainView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MediaButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MyMediaSubtitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TransferButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TransferMediaSubtitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton WebsiteButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WebsiteSubtitle { get; set; }

        [Action ("MediaButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MediaButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("TransferButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TransferButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("WebsiteButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void WebsiteButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (MainView != null) {
                MainView.Dispose ();
                MainView = null;
            }

            if (MediaButton != null) {
                MediaButton.Dispose ();
                MediaButton = null;
            }

            if (MyMediaSubtitle != null) {
                MyMediaSubtitle.Dispose ();
                MyMediaSubtitle = null;
            }

            if (TransferButton != null) {
                TransferButton.Dispose ();
                TransferButton = null;
            }

            if (TransferMediaSubtitle != null) {
                TransferMediaSubtitle.Dispose ();
                TransferMediaSubtitle = null;
            }

            if (WebsiteButton != null) {
                WebsiteButton.Dispose ();
                WebsiteButton = null;
            }

            if (WebsiteSubtitle != null) {
                WebsiteSubtitle.Dispose ();
                WebsiteSubtitle = null;
            }
        }
    }
}