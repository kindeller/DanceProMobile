using System;
using System.Collections.Generic;
using DancePro.Models;
using UIKit;
using AVFoundation;
using Foundation;
using CoreMedia;

namespace DancePro.iOS.ViewControllers
{
    public partial class AudioObjectViewController : UIViewController, IMediaObjectController
    {

        public List<MediaObject> MediaList { get; set; }
        public MediaObject MediaObject { get; set; }

        private static AVAudioPlayer Player { get; set; }
        private AVAsset Asset { get; set; }
        private AVPlayerItem item { get; set; }
        private AVAudioSession Session { get; set; }
        NSTimer timer;

        private NSError PlayerError;

        public AudioObjectViewController(IntPtr intPtr) : base(intPtr)
        {
            MediaList = new List<MediaObject>();
            Session = AVAudioSession.SharedInstance();

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            InitAudio();
            InitPlayer();

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void InitPlayer()
        {
            if (Player == null)
            {
                NSUrl url = NSUrl.FromFilename(MediaObject.FilePath);
                Player = new AVAudioPlayer(url, "Audio", out PlayerError);
                Player.FinishedPlaying += Player_FinishedPlaying;
                Player.SetVolume(0.5f, 2);
                VolumeBar.Value = 0.5f;
            }
            else
            {
                if (Player.Playing)
                {
                    PlayButton.SetTitle("Pause", UIControlState.Normal);
                    UI_UpdateStop();
                    VolumeBar.Value = Player.Volume;
                }

            }

            VolumeBar.Continuous = false;
        }

        partial void VolumeBar_Changed(UISlider sender)
        {
            Player.SetVolume(sender.Value, 0.5);
        }


        partial void DurationSliderChanged(UISlider sender)
        {
            if (sender.Value < Asset.Duration.Seconds)
            {
                //TODO: Fix the slider to equal the full duration for the song so the value matches
                Player.CurrentTime = sender.Value;
            }
        }

        private void UpdateTrackUI()
        {
            DurationTime.Text = getTime(Player.CurrentTime);
            DurationBar.Value = (float)Player.CurrentTime;
        }

        private void InitAudio()
        {
            var newAsset = AVAsset.FromUrl(NSUrl.FromFilename(MediaObject.FilePath));
            if (Asset != null)
            {
                if (Asset != newAsset)
                {
                    Player.Stop();
                    PlayButton.SetTitle("Play", UIControlState.Normal);
                    Asset = newAsset;
                }
            }
            else
            {
                Asset = newAsset;
            }

            DurationBar.MinValue = 0;
            DurationBar.MaxValue = (float)Asset.Duration.Seconds;
            TotalDuration.Text = getTime(Asset.Duration.Seconds);
        }

        partial void Play(UIButton sender)
        {

            if (Player.Playing)
            {
                UI_Pause();
            }
            else
            {
                UI_Play();
            }
        }


        private void UI_Pause()
        {
            Session.SetActive(false);
            Player.Pause();
            PlayButton.SetTitle("Play", UIControlState.Normal);
            UI_UpdateStop();
            StopButton.Enabled = true;
        }

        private void UI_Play()
        {
            Session.SetCategory(AVAudioSessionCategory.Playback);
            Session.SetActive(true);
            Player.Play();
            PlayButton.SetTitle("Pause", UIControlState.Normal);
            UI_UpdateStart();
            StopButton.Enabled = true;
        }

        private void UI_UpdateStop()
        {
            if (timer != null)
            {
                timer.Invalidate();
                timer = null;
            }
        }

        private void UI_UpdateStart()
        {
            if(timer == null)
            {
                timer = timer = NSTimer.CreateRepeatingScheduledTimer(1, (NSTimer obj) => UpdateTrackUI());
                timer.Fire();
            }

        }

        partial void Stop(UIButton sender)
        {
            Session.SetActive(false);
            Player.Stop();
            Player.CurrentTime = 0;
            DurationBar.Value = 0;
            DurationTime.Text = "00:00";
            PlayButton.SetTitle("Play", UIControlState.Normal);
            UI_UpdateStop();
            StopButton.Enabled = false;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if(Player != null)
            {
                if (!Player.Playing)
                {
                    Player.Dispose();
                    Player = null;
                }
            }

        }

        void Player_FinishedPlaying(object sender, AVStatusEventArgs e)
        {

            UI_UpdateStop();
            PlayButton.SetTitle("Play", UIControlState.Normal);
            StopButton.Enabled = false;
        }


        private string getTime(double time)
        {

            var minutes = Math.Floor(time / 60);
            var seconds = Math.Floor(time - minutes * 60);
            var m = (minutes < 10) ? "0" + minutes.ToString() : minutes.ToString();
            var s = (seconds < 10) ? "0" + seconds.ToString() : seconds.ToString();

            return m + ":" + s;
        }

    }
}

