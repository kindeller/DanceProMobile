using System;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using Android.Widget;

namespace DancePro.Droid
{
    public class ImagePagerAdapter : PagerAdapter
    {
        Context Context;
        MediaFolder ImageData;

        public ImagePagerAdapter(Context context, MediaFolder imageData)
        {
            Context = context;
            ImageData = imageData;
            ImageData.ImagesOnly();
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {

            var imageView = new ScaleImageView(Context,null);
            imageView.SetImageBitmap(ImageData.GetFullImage(position));
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.AddView(imageView);
            return imageView;
            
        }

        public override int Count
        {
            get { return ImageData.ItemCount; }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(ImageData[position].FileName);
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.RemoveView(@object as View);
        }
    }
}
