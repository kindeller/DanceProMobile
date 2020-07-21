using System;
using System.Collections.Generic;
using System.Windows.Input;
using Foundation;
using UIKit;
using Microsoft.AppCenter.Analytics;

namespace DancePro.iOS
{
    public partial class ContactViewController : UIViewController
    {
        private readonly Dictionary<string, NSUrl> ContactLinks = new Dictionary<string, NSUrl>()
        {
            {"FacebookApp", NSUrl.FromString("fb://profile/217475155027428")},
            {"FacebookWeb", NSUrl.FromString("https://www.facebook.com/DanceProPhotoVideo/")},
            {"InstagramApp", NSUrl.FromString("instagram://user?username=danceprophoto")},
            {"InstagramWeb", NSUrl.FromString("https://instagram.com/danceprophoto")},
            {"Mail", NSUrl.FromString("Mailto:info@dancepro.com.au")},
            {"Website", NSUrl.FromString("https://www.dancepro.com.au/")}
        };

        public ContactViewController(IntPtr handle) : base(handle)
        {
            //OpenContactCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://www.dancepro.com.au/"));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Analytics.TrackEvent("[Contact] Loaded Page");
            NavigationController.NavigationBar.Hidden = true;
            //TODO: Remove temp fix for darkmode issues and add dark mode support
            //if (AppDelegate.CheckVersion(13))
            //{
            //    OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            //}
            
    }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //Open Browser
            //OpenContactCommand.Execute(null);
            ////Get Tab Controller
            //UITabBarController controller = UIApplication.SharedApplication.KeyWindow.RootViewController as UITabBarController;
            //if (controller != null)
            //{
            //    //Return to Home
            //    controller.SelectedIndex = 0;
            //}
        }

        partial void Facebook_TouchUpInside(UIButton sender)
        {
            Analytics.TrackEvent("[Contacts] Clicked Facebook Button");
            using(var app = UIApplication.SharedApplication)
            {
                if (app.CanOpenUrl(ContactLinks.GetValueOrDefault("FacebookApp")))
                {
                    app.OpenUrl(ContactLinks.GetValueOrDefault("FacebookApp"));
                }
                else
                {
                    app.OpenUrl(ContactLinks.GetValueOrDefault("FacebookWeb"));
                }
            }
        }

        partial void Instagram_UpInside(UIButton sender)
        {
            Analytics.TrackEvent("[Contacts] Clicked Instagram Button");
            using (var app = UIApplication.SharedApplication)
            {
                if (app.CanOpenUrl(ContactLinks.GetValueOrDefault("InstagramApp")))
                {
                    app.OpenUrl(ContactLinks.GetValueOrDefault("InstagramApp"));
                }
                else
                {
                    app.OpenUrl(ContactLinks.GetValueOrDefault("InstagramWeb"));
                }
            }
        }

        partial void Mail_UpInside(UIButton sender)
        {
            Analytics.TrackEvent("[Contacts] Clicked Mail Button");
            using (var app = UIApplication.SharedApplication)
            {
                app.OpenUrl(ContactLinks.GetValueOrDefault("Mail"));
            }
        }


        partial void Website_UpInside(UIButton sender)
        {
            Analytics.TrackEvent("[Contacts] Clicked Website Button");
            using (var app = UIApplication.SharedApplication)
            {
                app.OpenUrl(ContactLinks.GetValueOrDefault("Website"));
            }
        }

        partial void WebsiteLink_UpInside(UIButton sender)
        {
            Analytics.TrackEvent("[Contacts] Clicked Website Button");
            using (var app = UIApplication.SharedApplication)
            {
                app.OpenUrl(ContactLinks.GetValueOrDefault("Website"));
            }
        }
    }
}
