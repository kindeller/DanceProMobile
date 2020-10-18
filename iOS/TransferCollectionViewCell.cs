using System;

using Foundation;
using UIKit;
using DancePro.Models;
using DancePro.iOS.ViewControllers;

namespace DancePro.iOS
{
    public partial class TransferCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("TransferCollectionViewCell");
        NewDownloadModel Model;

        static TransferCollectionViewCell()
        {
        }

        protected TransferCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void UpdateRow(NewDownloadModel downloadModel)
        {
            Model = downloadModel;
            TitleLabel.Text = Model.FileName;
            MessageLabel.Text = downloadModel.Message;
            
            switch (downloadModel.Status)
            {
                case NewDownloadModel.DownloadStatus.Completed:
                    ActivityIndicator.Hidden = true;
                    ActivityIndicator.StopAnimating();
                    CompletedImage.Hidden = false;
                    MessageLabel.TextColor = AppDelegate.DanceProBlue;
                    break;
                case NewDownloadModel.DownloadStatus.Downloading:
                    ActivityIndicator.Hidden = false;
                    ActivityIndicator.StartAnimating();
                    CompletedImage.Hidden = true;
                    break;
                case NewDownloadModel.DownloadStatus.Failed:
                    MessageLabel.TextColor = UIColor.Red;
                    CompletedImage.Hidden = true;
                    ActivityIndicator.Hidden = true;
                    ActivityIndicator.StopAnimating();
                    break;
            }
        }

        public string GetMediaObjectPath()
        {
            return Model.FilePath;
        }
    }
}
