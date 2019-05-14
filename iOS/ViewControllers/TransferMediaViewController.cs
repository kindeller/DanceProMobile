using System;
using System.Collections.Generic;
using DancePro.Services;
using DancePro.ViewModels;
using UIKit;


namespace DancePro.iOS.ViewControllers
{
    public partial class TransferMediaViewController : UIViewController
    {
        NetworkService NetworkService;
        TransferViewModel Model;

        public TransferMediaViewController(IntPtr intPtr) : base(intPtr)
        {
            Model = new TransferViewModel(AppDelegate.NetworkService);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Model.DownloadsUpdated += Model_DownloadsUpdated;
            InitTransfer();
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

        partial void Clear_TouchUpInside(UIButton sender)
        {
            Model.ClearDownloads();
        }

        private void InitTransfer()
        {
            TransferUICollectionSource source = new TransferUICollectionSource();
            DownloadsCollectionView.Source = source;
            DownloadsCollectionView.ReloadData();
            NetworkService = AppDelegate.NetworkService;
            NetworkService.Connect();
            AddressLabel.Text = $"{NetworkService.Address}:{NetworkService.Port}";
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

