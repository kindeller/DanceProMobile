
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vlc.DotNet.Core;
using LibVLCSharp.Shared;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using System.Threading.Tasks;

namespace DancePro.Droid.Activities
{
    [Activity(Label = "VideoActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class VideoActivity : Activity
    {
        LibVLCSharp.Platforms.Android.VideoView view;
        LibVLCSharp.Shared.MediaPlayer player;
        SeekBar DurationBar;
        TextView currentTime;
        Media media;

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.VideoItemLayout);
            InitialiseUI();
            //OldVideoSetUp();
            NewVideoSetUp();
        }

        private void InitialiseUI()
        {
            FindViewById<Button>(Resource.Id.VideoCloseButton).Click += VideoActivity_Click;
            FindViewById<Button>(Resource.Id.VideoPlayButton).Click += VideoPlayButton_Click;
            FindViewById<Button>(Resource.Id.VideoStopButton).Click += VideoStopButton_Click;
            DurationBar = FindViewById<SeekBar>(Resource.Id.seekBar1);
            DurationBar.ProgressChanged += VideoActivity_ProgressChanged;
            currentTime = FindViewById<TextView>(Resource.Id.textVideoCurrentTime);
            
            view = FindViewById<LibVLCSharp.Platforms.Android.VideoView>(Resource.Id.videoView1);
        }

        private void VideoActivity_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.Progress);
            if(player != null && e.FromUser)
            {
                player.Time = e.Progress;
            }
        }

        private void VideoActivity_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void VideoPlayButton_Click(object sender, EventArgs e)
        {
            Button b = FindViewById<Button>(Resource.Id.VideoPlayButton);
            if (player.IsPlaying)
            {
                b.Text = GetString(Resource.String.play);
                player.Pause();
            }
            else
            {
                b.Text = GetString(Resource.String.pause);
                player.Play();
            }
            
        }

        private void VideoStopButton_Click(object sender, EventArgs e)
        {
            Button b = FindViewById<Button>(Resource.Id.VideoPlayButton);
            b.Text = GetString(Resource.String.play);
            player.Stop();
            DurationBar.Progress = 0;
            currentTime.Text = "0:00";
        }

        private void NewVideoSetUp()
        {
            
            var path = Intent.GetStringExtra("filepath");
            Console.WriteLine($"************* {path} *************");
            Core.Initialize();

            using (LibVLC lib = new LibVLC())
            {
                player = new LibVLCSharp.Shared.MediaPlayer(lib)
                {
                    EnableHardwareDecoding = false
                };
                view.MediaPlayer = player;
                media = new Media(lib, path, FromType.FromPath);
                media.ParsedChanged += Media_ParsedChanged;
                player.TimeChanged += Player_TimeChanged;
                player.Play(media);
                player.EncounteredError += Player_EncounteredError;
            }

        }

        private void Media_ParsedChanged(object sender, MediaParsedChangedEventArgs e)
        {
            Console.WriteLine();
            if (e.ParsedStatus == MediaParsedStatus.Done)
            {
                DurationBar.Max = (int)media.Duration;

                RunOnUiThread(() => { FindViewById<TextView>(Resource.Id.textVideoTotalTime).Text = TimeSpan.FromMilliseconds(media.Duration).ToString(@"m\:ss"); });
                media.ParsedChanged -= Media_ParsedChanged;
            };
        }

        private void Player_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            DurationBar.Progress = (int)e.Time;
            RunOnUiThread(() => { currentTime.Text = TimeSpan.FromMilliseconds(e.Time).ToString(@"m\:ss"); });
        }

        private void Player_EncounteredError(object sender, EventArgs e)
        {
            Console.WriteLine(sender);
            Console.WriteLine(e);
        }

        private void OldVideoSetUp()
        {
                var path = Intent.GetStringExtra("filepath");
                Console.WriteLine($"************* {path} *************");
                var position = Intent.GetIntExtra("position", 0);
                var view = FindViewById<Android.Widget.VideoView>(Resource.Id.videoView1);
                view.SetVideoPath(path);
                MediaController controller = new MediaController(this);
                view.SetMediaController(controller);
                FindViewById<Button>(Resource.Id.VideoCloseButton).Click += VideoActivity_Click;
                view.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            player.Stop();
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            return base.OnKeyUp(keyCode, e);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            media.ParsedChanged -= Media_ParsedChanged;
            media.Dispose();
            player.Stop();
            player.Dispose();
        }
    }
}
