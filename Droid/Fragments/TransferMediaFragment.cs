
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

using DancePro.ViewModels;
using DancePro.Models;
using Android.Support.V7.Widget;

namespace DancePro.Droid
{
    public class TransferMediaFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        TransferViewModel ViewModel;
        Button btnEnable;
        Button btnClear;
        TextView connectedText;
        TextView TotalCompletedTextView;
        RecyclerView TransferItemsView;
        TransferMediaAdapter Adapter;
        RecyclerView.LayoutManager LayoutManager;


        public void BecameVisible()
        {
            
        }

        public static TransferMediaFragment NewInstance() => new TransferMediaFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = new TransferViewModel(App.NetworkService);

            View view = inflater.Inflate(Resource.Layout.fragment_TransferMedia, container, false);
            TotalCompletedTextView = view.FindViewById<TextView>(Resource.Id.transfertotaltextview);
            TransferItemsView = view.FindViewById<RecyclerView>(Resource.Id.TransferItemsView);
            connectedText = view.FindViewById<TextView>(Resource.Id.textConnected);
            connectedText.Text = ViewModel.GetDeviceText();
            btnEnable = view.FindViewById<Button>(Resource.Id.btnEnable);
            btnEnable.Click += (sender, e) => {

                ViewModel.ToggleConnection();
                ToggleButtonText();
            };
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnClear.Click += (sender, e) =>
            {
                ViewModel.ClearDownloads();
            };

            ViewModel.DownloadsUpdated += ViewModel_DownloadsUpdated;

            //Create Adapter
            Adapter = new TransferMediaAdapter();
            //Create Layout
            LayoutManager = new GridLayoutManager(Context, 3);
            //Set Adapter
            TransferItemsView.SetAdapter(Adapter);
            //Set Layout
            TransferItemsView.SetLayoutManager(LayoutManager);


            return view;
        }

        private void ViewModel_DownloadsUpdated(List<NewDownloadModel> mediaList)
        {
            Activity.RunOnUiThread(() => {
                //setup Adapter and media list

                Adapter.TransferList = mediaList;
                Adapter.NotifyDataSetChanged();
                // -- Old iOS Example
                //TransferUICollectionSource source = new TransferUICollectionSource();
                //source.DownloadList = mediaList;
                //DownloadsCollectionView.Source = source;

                //Setup completed and downloading from Model and update text

                TotalCompletedTextView.Text = ViewModel.GetCompletedText(); 
                // -- Old iOS Example
                //var completed = Model.GetCompletedCount();
                //var downloading = Model.GetDownloadCount();
                //TotalLabel.Text = $"({completed} / {downloading})";
                //TotalLabel.TextColor = (completed == downloading) ? AppDelegate.DanceProBlue : UIColor.Black;
            });
        }

        private void ToggleButtonText()
        {
            var text = ViewModel.isNetworkListening() ? "Disable" : "Enable";
            btnEnable.Text = text;
        }
    }
}
