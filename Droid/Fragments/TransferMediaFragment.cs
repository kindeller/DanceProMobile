
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
using System.Threading.Tasks;

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
        private bool cancelUIUpdate = false;

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
            GetDeviceID();
            btnEnable = view.FindViewById<Button>(Resource.Id.btnEnable);
            btnEnable.Click += (sender, e) => {
                if (ViewModel.isNetworkListening())
                {
                    ViewModel.Disconnect();
                    SetDeviceText("Disconnected");
                    ToggleButtonText();
                    
                }
                else
                {
                    if (ViewModel.GetIsConnecting()) return;
                    BackgroundConnectAsync();
                }
                
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

        public override bool UserVisibleHint {
            get => base.UserVisibleHint;
            set
            {
                base.UserVisibleHint = value;
                if (value)
                {
                    BackgroundConnectAsync();
                }
            }
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
            Activity.RunOnUiThread(() =>
            {
                var text = ViewModel.isNetworkListening() ? "Disable" : "Enable";
                btnEnable.Text = text;
            });
        }

        private void GetDeviceID()
        {
            connectedText.Text = ViewModel.GetDeviceText();
        }


        private async void BackgroundConnectAsync()
        {

            Console.WriteLine("Background Async Started");
            ViewModel.ConnectToWifi(SetDeviceText);
            //Check if failed to connect to wifi
            Console.WriteLine("Connecting...");
            Activity.RunOnUiThread(() =>
            {
                SetDeviceText("Connecting");
            });
            if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
            {
                while (!cancelUIUpdate || !ViewModel.isNetworkListening())
                {
                    ViewModel.Connect();
                    await Task.Delay(1000);
                    Activity.RunOnUiThread(() =>
                    {
                        Console.WriteLine("Updating UI...");
                        SetDeviceText();
                        ToggleButtonText();
                        return;
                    });
                }
                cancelUIUpdate = false;
            }
        }

        private void SetDeviceText()
        { 
            string result = ViewModel.GetDeviceText();
            if (!string.IsNullOrEmpty(result)) connectedText.Text = result;
        }


        /// <summary>
        /// This method needs reworked to represent what it is
        /// </summary>
        /// <param name="text"></param>
        private void SetDeviceText(string text)
        {
            Console.WriteLine("Updating Text: " + text);
            switch (text)
            {
                case "Wifi Declined":
                    cancelUIUpdate = true;
                    connectedText.Text = text;
                    break;
                case "Enabling Network":
                    connectedText.Text = text;
                    SetDeviceText();
                    break;
                default:
                    connectedText.Text = text;
                    break;

            }
            ToggleButtonText();

        }
    }
}
