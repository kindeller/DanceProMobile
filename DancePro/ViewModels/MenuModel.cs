using System.Windows.Input;

namespace DancePro
{
    public class MenuModel : BaseViewModel
    {
        public MenuModel()
        {
            Title = "Home";

            OpenPhotosCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://photos.dancepro.com.au/"));
            OpenVideosCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://www.dancepro.com.au/video"));
        }

        public ICommand OpenPhotosCommand { get; }
        public ICommand OpenVideosCommand { get; }
    }
}
