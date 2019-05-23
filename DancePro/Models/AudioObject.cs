using System;

#if __IOS__
using AVFoundation;
using Foundation;
using UIKit;
using CoreGraphics;
#endif

namespace DancePro.Models
{
    public class AudioObject : MediaObject
    {

        //NSUrl AudioFileURL;

#if __IOS__

        public AVAudioFile Audio { get; set; }

        public AudioObject(string filePath) : base(filePath)
        {
            MediaType = MediaTypes.Audio;
            NSUrl url = NSUrl.FromFilename(filePath);
            NSError err;
            Audio = new AVAudioFile(url, out err);
            if(err != null)
            {
                Console.WriteLine("Failed to create audio file: " + FileName);
            }
            SegueString = new NSString("AudioSegue");
            Thumb = UIImage.FromBundle("Audio");
        }

        /*public override UIView GetDetailView(UIViewController mainController)
        {
            CGRect mainFrame = mainController.View.Frame;
            CGRect rect = new CGRect(0, 0, mainFrame.Width / 2, mainFrame.Height / 3);
            UIView view = new UIView(mainFrame);
            UIButton PlayButton = new UIButton(UIButtonType.Plain);
            PlayButton.Frame = rect;
            UILabel playButtonLabel = new UILabel(rect);
            playButtonLabel.Text = "Play";
            playButtonLabel.TextColor = UIColor.Black;
            PlayButton.AddSubview(playButtonLabel);
            view.AddSubview(PlayButton);
            playButtonLabel.ContentMode = UIViewContentMode.Center;
            PlayButton.ContentMode = UIViewContentMode.Center;

            UIButton StopButton = new UIButton(UIButtonType.Plain);
            rect = new CGRect(0, rect.Height, mainFrame.Width / 2, mainFrame.Height / 3);
            StopButton.Frame = rect;
            UILabel stopButtonLabel = new UILabel(rect);
            stopButtonLabel.Text = "Stop";
            stopButtonLabel.TextColor = UIColor.Black;
            StopButton.AddSubview(stopButtonLabel);
            view.AddSubview(StopButton);
            stopButtonLabel.ContentMode = UIViewContentMode.Center;
            StopButton.ContentMode = UIViewContentMode.Center;

            mainController.View.BackgroundColor = UIColor.White;


            return view;
        }*/

#endif

#if __ANDROID__

        public AudioObject(string filePath) : base(filePath)
        {
            MediaType = MediaTypes.Audio;
        }

#endif

    }
}
