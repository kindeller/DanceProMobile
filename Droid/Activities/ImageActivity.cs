
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace DancePro.Droid.Activities
{
    [Activity(Label = "ImageActivity")]
    public class ImageActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_image_gallery);

            ViewPager pager = FindViewById<ViewPager>(Resource.Id.viewPager1);
            var path = Intent.GetStringExtra("filepath");
            var position = Intent.GetIntExtra("position", 0);

            pager.Adapter = new ImagePagerAdapter(pager.Context, new MediaFolder(path));
            pager.SetCurrentItem(position, true);

            FindViewById<Button>(Resource.Id.GalleryCloseButton).Click += ImageActivity_Click;
           
        }

        private void ImageActivity_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}
