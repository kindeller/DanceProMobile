using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFoundation;
using DancePro.Services;
using DancePro.ViewModels;
using Microsoft.AppCenter.Analytics;
using UIKit;


namespace DancePro.iOS.ViewControllers
{
    public partial class TransferMediaViewController : UIViewController
    {
        TransferViewModel Model;
        bool cancelUIUpdate = false;

        public TransferMediaViewController(IntPtr intPtr) : base(intPtr)
        {
            Model = new TransferViewModel(AppDelegate.NetworkService);
            AppDelegate.NetworkService.OnStoppedListening += NetworkService_OnStoppedListening;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Analytics.TrackEvent("[Transfer] Loaded Page");
            // Perform any additional setup after loading the view, typically from a nib.
            //TODO: Remove temp fix for darkmode issues and add dark mode support
            //if (AppDelegate.CheckVersion(13))
            //{
            //    OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            //}
            Model.DownloadsUpdated += Model_DownloadsUpdated;
            InitTransfer();
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        void NetworkService_OnStoppedListening(object sender, EventArgs e)
        {
            InvokeOnMainThread(() => {
                Model.UpdateNetworkService(AppDelegate.RefreshNetworkService());
                AppDelegate.NetworkService.OnStoppedListening += NetworkService_OnStoppedListening;
                SetDeviceText();
                ToggleButton.Enabled = true;
                ToggleButtonText();
            });

        }

        void Model_DownloadsUpdated(List<Models.NewDownloadModel> mediaList)
        {
            //Console.WriteLine("[TModel] Updating Media List");

            //foreach(var item in mediaList)
            //{
            //    Console.WriteLine("(" + item.ID + ") " + item.FileName + " - " + item.Message);
            //}
            //Console.WriteLine("// End List");
            InvokeOnMainThread(() => {
                TransferUICollectionSource source = new TransferUICollectionSource();
                source.DownloadList = mediaList;
                DownloadsCollectionView.Source = source;

                //DownloadsCollectionView.ReloadData();
                var completed = Model.GetCompletedCount();
                var downloading = Model.GetDownloadCount();
                TotalLabel.Text = $"({completed} / {downloading})";

                if (completed == downloading)
                {
                    TotalLabel.TextColor = AppDelegate.DanceProBlue;
                    Analytics.TrackEvent("[Transfer] Transfer Complete");
                }
                else
                {
                    TotalLabel.TextColor = UIColor.Black;
                }
            });

        }

        partial void ToggleButton_UpInside(UIButton sender)
        {

            if (Model.isNetworkListening())
            {
                Analytics.TrackEvent("[Transfer] Disabling Server Listening");
                ToggleButton.Enabled = false;
                SetDeviceText("Not Connected");
                Model.Disconnect();
            }
            else
            {
                Analytics.TrackEvent("[Transfer] Enabling Server Listening");
                DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
                {
                    BackgroundConnectAsync();
                });
            }
            
        }


        partial void Clear_TouchUpInside(UIButton sender)
        {
            Model.ClearDownloads();
        }

        private void InitTransfer()
        {
            TransferUICollectionSource source = new TransferUICollectionSource();
            DownloadsCollectionView.Source = source;
            DownloadsCollectionView.ReloadData();
            //AddressLabel.Text = $"{NetworkService.Address}:{NetworkService.Port}";
        }

        private async void BackgroundConnectAsync()
        {
            Model.ConnectToWifi(SetDeviceText);
            //Check if failed to connect to wifi
            Console.WriteLine("Connecting...");
            InvokeOnMainThread(() =>
            {
                SetDeviceText("Connecting");
            }); 
            while (!cancelUIUpdate && !Model.isNetworkListening())
            {
                //Console.WriteLine(Model.isNetworkListening());
                Model.Connect();
                await Task.Delay(1000);
                InvokeOnMainThread(() =>
                {
                    Console.WriteLine("Updating UI...");
                    SetDeviceText();
                    ToggleButtonText();
                    return;
                });
            }
            cancelUIUpdate = false;
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
            Analytics.TrackEvent("[Transfer] Recieved Memory Warning");
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            SetDeviceText("Not Connected");
            DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
            {
                BackgroundConnectAsync();
            });
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UIApplication.SharedApplication.IdleTimerDisabled = false;
            Model.Disconnect();
        }

        private void SetDeviceText()
        {
            string result = Model.GetDeviceText();
            if (!string.IsNullOrEmpty(result)) AddressLabel.Text = result;
        }

        private void SetDeviceText(string text)
        {
            if(text == "Wifi Declined")
            {
                cancelUIUpdate = true;
            }
            AddressLabel.Text = text;
        }

        private void ToggleButtonText()
        { 
            ToggleButton.SetTitle(Model.GetButtonText(), UIControlState.Normal);
        }

    }
}

