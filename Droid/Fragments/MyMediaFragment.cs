
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace DancePro.Droid
{
    public class MyMediaFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        MediaFolder MediaFolder;
        MediaAdapter Adapter;
        RecyclerView.LayoutManager LayoutManager;
        RecyclerView MediaRecyclerView;

        public void BecameVisible()
        {
          
        }

        public static MyMediaFragment NewInstance() => new MyMediaFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_mymedia, container, false);

            // Create Media / TODO: Get Media
            MediaFolder = new MediaFolder();
            //Create Adapter
            Adapter = new MediaAdapter(MediaFolder);
            //Get Recycler View
            MediaRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.MyMediaRecycleView);
            //Create Layout
            LayoutManager = new GridLayoutManager(Context, 3);
            //Set Adapter
            MediaRecyclerView.SetAdapter(Adapter);
            //Set Layout
            MediaRecyclerView.SetLayoutManager(LayoutManager);
            return view;
        }
    }
}
