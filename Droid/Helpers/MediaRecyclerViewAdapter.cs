
using System;
using System.Collections.Generic;

using DancePro.Models;

using Android.Views;
using Android.Support.V7.Widget;
using Android.Content;

namespace DancePro.Droid.Helpers
{
    public class MediaRecyclerViewAdapter : RecyclerView.Adapter
    {
        private LayoutInflater mInflater;
        private iMediaItemClickListener mClickListener;

        private List<MediaObject> mMediaObjects;

        MediaRecyclerViewAdapter(Context context, List<MediaObject> mediaObjects)
        {
            mInflater = LayoutInflater.From(context);
            mMediaObjects = mediaObjects;
        }

        public override int ItemCount => mMediaObjects.Count;
        

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = mInflater.Inflate(Resource.Layout.MediaCellLayout, parent, false);
            return new ViewHolder(view);
        }

        public class ViewHolder : RecyclerView.ViewHolder
        {
            MediaObject mediaObject;
            


            public ViewHolder(View itemView) : base(itemView)
            {

            }

            public void SetObject(MediaObject _mediaObject)
            {
                mediaObject = _mediaObject;


            }

            public void OnClick(View v)
            {
                throw new NotImplementedException();
            }
        }

        void setClickListener(iMediaItemClickListener itemClickListener)
        {
            mClickListener = itemClickListener;
        }

        public interface iMediaItemClickListener
        {
            void onItemClick(View view, int Position);
        }
    }


}
