using System;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace DancePro.Droid
{

    public class MediaAdapter : BaseRecycleViewAdapter
    {
        public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);

        private RecyclerView recyclerView;
        public MediaFolder MediaFolder;
        iShowDetails DetailController;

        public MediaAdapter(MediaFolder mediaFolder, iShowDetails detailsController)
        {
            MediaFolder = mediaFolder;
            DetailController = detailsController;
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
            vh.Image.SetImageBitmap(MediaFolder.GetThumb(vh.ItemView.Context, position));
            vh.Title.Text = mediaobject.FileName;
            vh.Date.Text = mediaobject.DateCreated.ToShortDateString();
            vh.ItemView.FindViewById<Button>(Resource.Id.MediaItemDetails).Click += (object sender, EventArgs e) =>
            {
                DetailController.ShowDetailsLayoutFor(mediaobject);
            };
            vh.ItemView.Click -= ItemView_Click;
            vh.ItemView.Click += ItemView_Click;
            vh.DetailsButton.Visibility = (mediaobject.MediaType == Models.MediaTypes.Other) ? ViewStates.Invisible : ViewStates.Visible;
        }

        public void SetRecyclerView(RecyclerView view)
        {
            recyclerView = view;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            int position = recyclerView.GetChildAdapterPosition((View)sender);
            Models.MediaObject mediaobject = MediaFolder.GetItem(position);
            if (mediaobject.MediaType == Models.MediaTypes.Folder || mediaobject.MediaType == Models.MediaTypes.Other)
            {
                DirectoryInfo dir = new DirectoryInfo(mediaobject.FilePath);
                MediaFolder.ChangeDirectory(dir.ToString());
                NotifyDataSetChanged();
                return;
            }
            var context = ((View)sender).Context;
            Type activity = GetActivity(mediaobject);
            if (activity == null) return;
            Intent intent = new Intent(context, activity);
            intent.PutExtra("filepath", mediaobject.FilePath);
            //Adjust for Added Media back button
            if (MediaFolder.GetItem(0).MediaType == Models.MediaTypes.Other) position--;
            intent.PutExtra("position", position);
            context.StartActivity(intent);
        }

        public override int ItemCount
        {
            get { return MediaFolder.ItemCount; }
        }


        public Type GetActivity(IMediaObject mediaObject)
        {
            switch (mediaObject.MediaType)
            {
                case Models.MediaTypes.Image:
                    return typeof(Activities.ImageActivity);
                case Models.MediaTypes.Video:
                    return typeof(Activities.VideoActivity);
                case Models.MediaTypes.Audio:
                    return typeof(Activities.AudioActivity);
                default:
                    return null;
            }
        }
    }

    public class MediaViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Title { get; private set; }
        public TextView Date { get; private set; }
        public Button DetailsButton { get; private set; }

        public MediaViewHolder(View itemView) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.MediaCellImage);
            Title = itemView.FindViewById<TextView>(Resource.Id.MediaCellTitle);
            Date = itemView.FindViewById<TextView>(Resource.Id.MediaCellDate);
            DetailsButton = itemView.FindViewById<Button>(Resource.Id.MediaItemDetails);
        }
    }

}
