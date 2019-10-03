
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DancePro.Droid.Activities
{
    [Activity(Label = "VideoActivity")]
    public class VideoActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VideoItemLayout);

            var path = Intent.GetStringExtra("filepath");
            var position = Intent.GetIntExtra("position", 0);
            VideoView view = FindViewById<VideoView>(Resource.Id.videoView1);
            view.SetVideoPath(path);
            MediaController controller = new MediaController(view.Context);
            view.SetMediaController(controller);
            FindViewById<Button>(Resource.Id.VideoCloseButton).Click += VideoActivity_Click;
            view.Start();
        }

        private void VideoActivity_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}
