using System;
using UIKit;

namespace DancePro.iOS
{
    public partial class TabBarController : UITabBarController
    {
        MenuModel Model = new MenuModel();
        public TabBarController(IntPtr handle) : base(handle)
        {
            TabBar.Items[0].Title = "Home";
            TabBar.Items[1].Title = "My Media";
            TabBar.Items[2].Title = "Photo";
            TabBar.Items[3].Title = "Video";
            TabBar.Items[4].Title = "Contact";



        }


    }
}
