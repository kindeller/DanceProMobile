
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace DancePro.Droid.Activities
{
    [Activity(Label = "AudioActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AudioActivity : Activity, SeekBar.IOnSeekBarChangeListener
    {
        bool isPlaying;
        MediaPlayer Player = new MediaPlayer();
        Handler Handler = new Handler();
        SeekBar SeekBar;
        TextView CurrentTimeText;
        TextView DurationTimeText;
        ImageButton PlayButton;
        ImageButton StopButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.Activity_Audio);

            ResetPlayer();
            var CloseButton = FindViewById<Button>(Resource.Id.AudioCloseButton);
            CloseButton.Click += CloseButton_Click;
            PlayButton = FindViewById<ImageButton>(Resource.Id.PlayButton);
            PlayButton.Click += AudioActivity_Play_Click;
            StopButton = FindViewById<ImageButton>(Resource.Id.StopButton);
            StopButton.Click += AudioActivity_Stop_Click;

            SeekBar = FindViewById<SeekBar>(Resource.Id.AudioSeekBar);
            SeekBar.SetOnSeekBarChangeListener(this);
            CurrentTimeText = FindViewById<TextView>(Resource.Id.textCurrentTime);
            DurationTimeText = FindViewById<TextView>(Resource.Id.textTotalTime);
            Play();
        }


        private void CloseButton_Click(object sender, EventArgs e)
        {
            Handler.RemoveCallbacks(audioUpdate);
            Handler.Dispose();
            Handler = null;
            Stop();
            Finish();

        }

        private void AudioActivity_Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void AudioActivity_Play_Click(object sender, EventArgs e)
        {
            if (Player == null) return;

            if (isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }


        void Play()
        {
            Player.Start();
            isPlaying = true;
            PlayButton.SetImageResource(Resource.Drawable.PauseButton);
            StopButton.Enabled = true;
            BeginUpdateUI();
        }

        void Pause()
        {
            Player.Pause();
            isPlaying = false;
            PlayButton.SetImageResource(Resource.Drawable.PlayButton);
        }

        void Stop()
        {
            if (Player == null) return;
            isPlaying = false;
            Player.Stop();
            ResetPlayer();
            StopButton.Enabled = false;
            PlayButton.SetImageResource(Resource.Drawable.PlayButton);
            if(Handler != null) Handler.RemoveCallbacks(audioUpdate);

        }

        void ResetPlayer()
        {
            Player.Reset();
            var path = Intent.GetStringExtra("filepath");
            Player.SetDataSource(path);
            Player.Prepare();
        }

        /// <summary>
        /// Called on play to update UI elements of the 
        /// </summary>
        void BeginUpdateUI()
        {
            if (Player == null || SeekBar == null) return;
            Handler.RemoveCallbacks(audioUpdate);
            Handler = new Handler();
            SeekBar.Max = Player.Duration;
            DurationTimeText.SetText(getTime(Player.Duration / 1000), null);

            RunOnUiThread(audioUpdate);

        }

        protected override void OnDestroy()
        {
            Player.Release();
            base.OnDestroy();
        }


        private string getTime(double time)
        {

            var minutes = Java.Lang.Math.Floor(time / 60);
            var seconds = Java.Lang.Math.Floor(time - minutes * 60);
            var m = (minutes < 10) ? "0" + minutes.ToString() : minutes.ToString();
            var s = (seconds < 10) ? "0" + seconds.ToString() : seconds.ToString();

            return m + ":" + s;
        }

        private void audioUpdate()
        {
            if (Handler == null) return;
            CurrentTimeText.SetText(getTime(Player.CurrentPosition / 1000), null);
            SeekBar.Progress = Player.CurrentPosition;
            Handler.PostDelayed(audioUpdate, 1000);
        }

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            Handler.RemoveCallbacks(audioUpdate);
        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            Player.SeekTo(seekBar.Progress);
            RunOnUiThread(audioUpdate);
        }
    }
}
