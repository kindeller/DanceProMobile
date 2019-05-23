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
    [Register ("TransferMediaViewController")]
    partial class TransferMediaViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AddressLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ClearButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView DownloadsCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DownloadsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ToggleButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TotalLabel { get; set; }

        [Action ("Clear_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Clear_TouchUpInside (UIKit.UIButton sender);

        [Action ("ToggleButton_UpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ToggleButton_UpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AddressLabel != null) {
                AddressLabel.Dispose ();
                AddressLabel = null;
            }

            if (ClearButton != null) {
                ClearButton.Dispose ();
                ClearButton = null;
            }

            if (DownloadsCollectionView != null) {
                DownloadsCollectionView.Dispose ();
                DownloadsCollectionView = null;
            }

            if (DownloadsLabel != null) {
                DownloadsLabel.Dispose ();
                DownloadsLabel = null;
            }

            if (ToggleButton != null) {
                ToggleButton.Dispose ();
                ToggleButton = null;
            }

            if (TotalLabel != null) {
                TotalLabel.Dispose ();
                TotalLabel = null;
            }
        }
    }
}