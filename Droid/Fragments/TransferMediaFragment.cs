
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
        TextView textID;
        TextView textStatus;
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
            textID = view.FindViewById<TextView>(Resource.Id.textID);
            textStatus = view.FindViewById<TextView>(Resource.Id.textStatus);
            GetDeviceID();
            btnEnable = view.FindViewById<Button>(Resource.Id.btnEnable);
            btnEnable.Click += BtnEnable_Click;
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnClear.Click += (sender, e) =>
            {
                ViewModel.ClearDownloads();
            };
            ViewModel.OnStoppedListening += ViewModel_OnStoppedListening;
            ViewModel.DownloadsUpdated += ViewModel_DownloadsUpdated;
            ViewModel.OnServerConnected += ViewModel_OnServerConnected;
            ViewModel.OnWifiConnectFail += ViewModel_OnWifiConnectFail;
            ViewModel.OnWifiConnectSuccess += ViewModel_OnWifiConnectSuccess;
            ViewModel.OnWifiDisconnected += ViewModel_OnWifiDisconnected;

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

        private void ViewModel_OnServerConnected(object sender, EventArgs e)
        {
            textStatus.Text = "Status: Connected.";
            UpdateUI();
        }

        private void ViewModel_OnWifiDisconnected(object sender, EventArgs e)
        {
            textStatus.Text = "Status: Wifi Disconnected.";
            UpdateUI();
        }

        private void ViewModel_OnWifiConnectSuccess(object sender, EventArgs e)
        {
            if (!ViewModel.isNetworkListening())
            {
                textStatus.Text = "Status: Wifi Connected!";
                Connect();
                UpdateUI();
            }

        }

        private void ViewModel_OnWifiConnectFail(object sender, EventArgs e)
        {
            textStatus.Text = "Status: Wifi Connect Failed.";
            UpdateUI();
        }

        private void ViewModel_OnStoppedListening(object sender, EventArgs e)
        {
            ViewModel.UpdateNetworkService(new Services.NetworkServiceAndroid());
            textStatus.Text = "Status: Disconnected.";
            UpdateUI();
        }

        /// <summary>
        /// Toggles between connecting and disconnecting depending on if its currently listening.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnable_Click(object sender, EventArgs e)
        {
            textStatus.Text = "Status: Connecting...";
            if (ViewModel.isNetworkListening())
            {
                ViewModel.Disconnect();
            }
            else
            {
                Connect();
            }
            
        }

        public override bool UserVisibleHint {
            get => base.UserVisibleHint;
            set
            {
                base.UserVisibleHint = value;
                if (value)
                {
                    textStatus.Text = "Status: Connecting...";
                    Connect();
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
            textID.Text = ViewModel.GetDeviceText();
        }

        private void SetDeviceText()
        { 
            string result = ViewModel.GetDeviceText();
            if (!string.IsNullOrEmpty(result))
            {
                textID.Text = result;
            }
            else
            {
                textID.Text = "Device ID: -";
            }
        }


        /// <summary>
        /// Handles Connecting to the server. Checks first if on wifi and connects if not attempts to connect.
        /// </summary>
        private void Connect()
        {
            if (ViewModel.isOnWifi())
            {
                ViewModel.Connect();
            }
            else
            {
                ViewModel.ConnectToWifi();
            }
        }

        private void UpdateUI()
        {
            SetDeviceText();
            ToggleButtonText();
        }

        public override void OnStop()
        {
            base.OnStop();
            if(IsVisible) ViewModel.Disconnect();
        }
    }
}
