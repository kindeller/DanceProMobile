using System.Windows.Input;

namespace DancePro
{
    public class MenuModel : BaseViewModel
    {
        public MenuModel()
        {
            Title = "Home";
            OpenWebsiteCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://www.dancepro.com.au/"));
        }
        public ICommand OpenWebsiteCommand { get; }
    }
}
