using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using DancePro.Models;

namespace DancePro.iOS.ViewControllers
{
    public partial class TransferUICollectionSource : UICollectionViewSource
    {
        public List<NewDownloadModel> DownloadList = new List<NewDownloadModel>();


        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return DownloadList.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (TransferCollectionViewCell)collectionView.DequeueReusableCell(TransferCollectionViewCell.CellID, indexPath);
            cell.UpdateRow(DownloadList[indexPath.Row]);
            return cell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            return base.GetViewForSupplementaryElement(collectionView, elementKind, indexPath);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //TODO: If required implimentation
            TransferCollectionViewCell cell = (TransferCollectionViewCell)collectionView.CellForItem(indexPath);
            string path = cell.GetMediaObjectPath();
            path = System.IO.Directory.GetParent(path).FullName;
            var controller = (TabBarController)UIApplication.SharedApplication.KeyWindow.RootViewController;
            if (controller != null)
            {
                controller.SelectedIndex = 2;
                var nav = (UINavigationController)controller.SelectedViewController;
                var media = (MyMediaViewController)nav.VisibleViewController;
                if (media != null)
                {
                    if (path != null)
                    {
                        media.ChangeDirectory(path);
                    }
                }
            }
        }

    }
}

