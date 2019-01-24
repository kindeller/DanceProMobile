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
    [Register ("MyMediaViewController")]
    partial class MyMediaViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ConnectSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView MediaItemsCollectionView { get; set; }

        [Action ("OnConnectSwitchChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnConnectSwitchChanged (UIKit.UISwitch sender);

        void ReleaseDesignerOutlets ()
        {
            if (ConnectSwitch != null) {
                ConnectSwitch.Dispose ();
                ConnectSwitch = null;
            }

            if (MediaItemsCollectionView != null) {
                MediaItemsCollectionView.Dispose ();
                MediaItemsCollectionView = null;
            }
        }
    }
}