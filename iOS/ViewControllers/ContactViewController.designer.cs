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
    [Register ("ContactViewController")]
    partial class ContactViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton FacebookButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton InstragramButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MailButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton WebsiteButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton websitelink { get; set; }

        [Action ("Facebook_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Facebook_TouchUpInside (UIKit.UIButton sender);

        [Action ("Instagram_UpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Instagram_UpInside (UIKit.UIButton sender);

        [Action ("Mail_UpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Mail_UpInside (UIKit.UIButton sender);

        [Action ("Website_UpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Website_UpInside (UIKit.UIButton sender);

        [Action ("WebsiteLink_UpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void WebsiteLink_UpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (FacebookButton != null) {
                FacebookButton.Dispose ();
                FacebookButton = null;
            }

            if (InstragramButton != null) {
                InstragramButton.Dispose ();
                InstragramButton = null;
            }

            if (MailButton != null) {
                MailButton.Dispose ();
                MailButton = null;
            }

            if (WebsiteButton != null) {
                WebsiteButton.Dispose ();
                WebsiteButton = null;
            }

            if (websitelink != null) {
                websitelink.Dispose ();
                websitelink = null;
            }
        }
    }
}