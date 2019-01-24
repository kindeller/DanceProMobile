using System;
using Foundation;
using UIKit;

namespace DancePro.iOS.ViewControllers
{
    public partial class MyMediaUICollectionDataSource : UICollectionViewDataSource
    {
        public MyMediaUICollectionDataSource() : base()
        {
        }



        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            throw new NotImplementedException();
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            throw new NotImplementedException();
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            return base.GetViewForSupplementaryElement(collectionView, elementKind, indexPath);
        }

    }
}

