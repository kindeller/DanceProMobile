using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using DancePro.Models;
using Java.Lang;

namespace DancePro.Droid
{
    public class TransferMediaAdapter : BaseRecycleViewAdapter
    {
        public List<NewDownloadModel> TransferList = new List<NewDownloadModel>();

        // - NOTE Removed this constrsuctor as the recycler was necessary in my media but at the moment its not needed: uncomment to pass to adapter for use.
        //RecyclerView recycleView;

        //public TransferMediaAdapter()
        //{

        //}

        //public TransferMediaAdapter(RecyclerView view)
        //{

        //    recycleView = view;
        //}

        public override int ItemCount
        {
            get { return TransferList.Count; }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TransferCellLayout, parent, false);
            TransferViewHolder vh = new TransferViewHolder(itemView);
            vh.CompletedImage.Visibility = ViewStates.Invisible;
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TransferViewHolder vh = holder as TransferViewHolder;
            var download = TransferList[position];
            if (download.Status == NewDownloadModel.DownloadStatus.Completed) vh.CompletedImage.Visibility = ViewStates.Visible;
            vh.Title.SetText(download.FileName, TextView.BufferType.Normal);
            vh.Status.SetText(download.Status.ToString(), TextView.BufferType.Normal);
        }
    }

    public class TransferViewHolder : RecyclerView.ViewHolder
    {
        public ImageView CompletedImage { get; private set; }
        public TextView Title { get; private set; }
        public TextView Status { get; private set; }

        public TransferViewHolder(View itemView) : base(itemView)
        {
            CompletedImage = itemView.FindViewById<ImageView>(Resource.Id.TransferCompleteImage);
            Title = itemView.FindViewById<TextView>(Resource.Id.TransferTitleView);
            Status = itemView.FindViewById<TextView>(Resource.Id.TransferStatusView);
        }
    }
}
