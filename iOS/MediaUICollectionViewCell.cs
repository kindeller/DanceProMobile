using System;

using DancePro.ViewModels;
using UIKit;

namespace DancePro.iOS
{
    public partial class MediaUICollectionViewCell : UICollectionViewCell
    {

        
NSString
        MediaItemViewModel ViewModel = new MediaItemViewModel();

        public MediaUICollectionViewCell() : base()
        {

        }

        public MediaUICollectionViewCell(MediaItemViewModel model)
        {
            ViewModel = model;
            TitleLabel.Text = ViewModel.Title;
        }
    }
}

