using System;
using System.IO;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace DancePro.Droid
{
    public class MediaAdapter : BaseRecycleViewAdapter
    {
        public MediaFolder MediaFolder;

        public MediaAdapter(MediaFolder mediaFolder)
        {
            MediaFolder = mediaFolder;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MediaCellLayout, parent, false);
            MediaViewHolder vh = new MediaViewHolder(itemView);
            return vh;
            
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MediaViewHolder vh = holder as MediaViewHolder;
            var mediaobject = MediaFolder[position];
            BitmapFactory.Options op = new BitmapFactory.Options();
            op.InSampleSize = 5;
            var imageBitmap = BitmapFactory.DecodeFile(mediaobject.FilePath, op);
            vh.Image.SetImageBitmap(Bitmap.CreateScaledBitmap(imageBitmap,150,150,false));
            vh.Title.Text = mediaobject.FileName;
            vh.Date.Text = mediaobject.DateCreated.ToShortDateString();
        }

        public override int ItemCount
        {
            get { return MediaFolder.ItemCount; }
        }
    }

    public class MediaViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Title { get; private set; }
        public TextView Date { get; private set; }


        public MediaViewHolder(View itemView) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.MediaCellImage);
            Title = itemView.FindViewById<TextView>(Resource.Id.MediaCellTitle);
            Date = itemView.FindViewById<TextView>(Resource.Id.MediaCellDate);
        }
    }
}
