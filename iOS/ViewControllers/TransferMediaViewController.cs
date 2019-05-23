using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancePro.Services;
using DancePro.ViewModels;
using UIKit;


namespace DancePro.iOS.ViewControllers
{
    public partial class TransferMediaViewController : UIViewController
    {
        TransferViewModel Model;

        public TransferMediaViewController(IntPtr intPtr) : base(intPtr)
        {
            Model = new TransferViewModel(AppDelegate.NetworkService);
            AppDelegate.NetworkService.OnStoppedListening += NetworkService_OnStoppedListening;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Model.DownloadsUpdated += Model_DownloadsUpdated;
            InitTransfer();
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
            
            InvokeOnMainThread(() => {
                TransferUICollectionSource source = new TransferUICollectionSource();
                source.DownloadList = mediaList;
                DownloadsCollectionView.Source = source;

                //DownloadsCollectionView.ReloadData();
                TotalLabel.Text = $"({Model.GetCompletedCount()} / {Model.GetDownloadCount()})";
            });

        }

        partial void ToggleButton_UpInside(UIButton sender)
        {
            if (Model.isNetworkListening())
            {
                ToggleButton.Enabled = false;
                Model.ToggleConnection();
            }
            else
            {
                Model.ToggleConnection();
                ToggleButtonText();
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
            SetDeviceText();
            ToggleButtonText();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetDeviceText();

        }

        private void SetDeviceText()
        {
            var id = Model.GetDeviceID();
            AddressLabel.Text = (string.IsNullOrEmpty(id)) ? "Not Connected!" : "Device ID:" + id;
        }

        private void ToggleButtonText()
        {
            var text = Model.isNetworkListening() ? "Disable" : "Enable";
            ToggleButton.SetTitle(text, UIControlState.Normal);
        }
    }
}

