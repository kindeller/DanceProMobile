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
    [Register ("AudioObjectViewController")]
    partial class AudioObjectViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider DurationBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DurationTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PlayButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StopButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TotalDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider VolumeBar { get; set; }

        [Action ("DurationSliderChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DurationSliderChanged (UIKit.UISlider sender);

        [Action ("Play:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Play (UIKit.UIButton sender);

        [Action ("Stop:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Stop (UIKit.UIButton sender);

        [Action ("VolumeBar_Changed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void VolumeBar_Changed (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (DurationBar != null) {
                DurationBar.Dispose ();
                DurationBar = null;
            }

            if (DurationTime != null) {
                DurationTime.Dispose ();
                DurationTime = null;
            }

            if (PlayButton != null) {
                PlayButton.Dispose ();
                PlayButton = null;
            }

            if (StopButton != null) {
                StopButton.Dispose ();
                StopButton = null;
            }

            if (TotalDuration != null) {
                TotalDuration.Dispose ();
                TotalDuration = null;
            }

            if (VolumeBar != null) {
                VolumeBar.Dispose ();
                VolumeBar = null;
            }
        }
    }
}