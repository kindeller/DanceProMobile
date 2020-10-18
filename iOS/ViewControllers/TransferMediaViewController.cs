using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFoundation;
using DancePro.Services;
using DancePro.ViewModels;
using Microsoft.AppCenter.Analytics;
using UIKit;
using System.Linq;


namespace DancePro.iOS.ViewControllers
{
    public partial class TransferMediaViewController : UIViewController
    {
        TransferViewModel Model;
        bool isDownloading = false;

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
            Model.DownloadsUpdated += Model_DownloadsUpdated;
            InitTransfer();
            UIApplication.SharedApplication.IdleTimerDisabled = true;

            Model.OnWifiConnectFail += Model_OnWifiConnectFail;
            Model.OnWifiConnectSuccess += Model_OnWifiConnectSuccess;
            Model.OnWifiDisconnected += Model_OnWifiDisconnected;
            Model.OnServerConnected += Model_OnServerConnected;
        }

        private void Model_OnServerConnected(object sender, EventArgs e)
        {
            SetDeviceText();
            ToggleButtonText();
        }

        private void Model_OnWifiDisconnected(object sender, EventArgs e)
        {
            SetDeviceText("Wifi Disconnected!");
        }

        private void Model_OnWifiConnectSuccess(object sender, EventArgs e)
        {
            SetDeviceText("Wifi Connected...");
            DelayedConnectAsync();
        }

        private void Model_OnWifiConnectFail(object sender, EventArgs e)
        {
            SetDeviceText("Wifi connect failed!");
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

                mediaList = mediaList.OrderBy(o => (int)o.Status).ToList();
                TransferUICollectionSource source = new TransferUICollectionSource();
                source.DownloadList = mediaList;
                DownloadsCollectionView.Source = source;

                //DownloadsCollectionView.ReloadData();
                var completed = Model.GetCompletedCount();
                var downloading = Model.GetDownloadCount();
                TotalLabel.Text = $"({completed} / {downloading})";
                if (!isDownloading)
                {
                    isDownloading = true;
                    OverlaySetup();
                }
                if (completed == downloading)
                {
                    TotalLabel.TextColor = AppDelegate.DanceProBlue;
                    Analytics.TrackEvent("[Transfer] Transfer Complete");
                    isDownloading = false;
                    RemoveOverlay();
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
                AddressLabel.Text = "Downloads";
                Connect();
                
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
            Connect();
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
            AddressLabel.Text = text;
        }

        private void ToggleButtonText()
        { 
            ToggleButton.SetTitle(Model.GetButtonText(), UIControlState.Normal);
        }

        private void Connect()
        {
            if (Model.isOnWifi())
            {
                Model.Connect();
            }
            else
            {
                Model.ConnectToWifi();
            }
        }

        private async void DelayedConnectAsync()
        {
            SetDeviceText("Connecting...");
            await Task.Delay(1500);
            Model.Connect();
        }

        private void OverlaySetup()
        {
            AddressLabel.Text = "Downloading...";
            DownloadsLabel.Text = "Stay on this screen";
            UIView mainView = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
            UIView Overlay = new UIView(mainView.Frame);
            Overlay.Layer.BorderColor = CoreGraphics.CGColor.CreateSrgb(255, 0, 0, 50);
            Overlay.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 100);
            Overlay.Layer.BorderWidth = 5;
            Overlay.Layer.ZPosition = 5;
            Overlay.Tag = 43;
            Overlay.Alpha = new nfloat(0.9);
            UIActivityIndicatorView activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            var activityWidth = activity.Frame.Width * 2;
            var activityHeight = activity.Frame.Height * 2;
            var activityX = mainView.Frame.Width / 2 - activity.Frame.Width;
            var activityY = mainView.Frame.Height / 2 - activity.Frame.Height;
            CoreGraphics.CGRect rect = new CoreGraphics.CGRect(activityX, activityY, activityWidth, activityHeight);
            activity.Frame = rect;
            activity.StartAnimating();
            Overlay.AddSubview(activity);
            mainView.AddSubview(Overlay);
        }

        private void RemoveOverlay()
        {
            foreach (var view in UIApplication.SharedApplication.KeyWindow.RootViewController.View.Subviews)
            {
                if(view.Tag == 43)
                {
                    view.RemoveFromSuperview();
                    view.Dispose();
                }
            }
            AddressLabel.Text = "Transfer Complete!";
            DownloadsLabel.Text = "Please check your media";
        }

    }
}

