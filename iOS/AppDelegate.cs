using Foundation;
using ObjCRuntime;
using UIKit;
using DancePro.Services;
using DancePro.iOS.ViewControllers;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;

namespace DancePro.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        public static bool CanRotate { get; set; }
        //TODO: [REMOVE] - Move this reference to App.NetworkService to unify iOS and Android
        public static NetworkServiceIOS NetworkService { get; set; }
        public readonly static UIColor DanceProBlue = UIColor.FromRGB(18, 141, 215);

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            App.Initialize();
            //TODO: [REMOVE] - Move this reference to App.NetworkService to unify iOS and Android
            NetworkService = new NetworkServiceIOS();
            AppCenter.Start("6709f283-3526-4c2e-8716-5a230f687988", typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("*** SESSION STARTED ***", new Dictionary<string, string>
            {
                {"Device Name",UIDevice.CurrentDevice.Name}
            });
            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, [Transient] UIWindow forWindow)
        {
            return CanRotate ? UIInterfaceOrientationMask.All : UIInterfaceOrientationMask.Portrait;
        }

        public static NetworkService RefreshNetworkService()
        {
            return NetworkService = new NetworkServiceIOS();
        }


        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var newFolderDir = App.MediaService.UnzipURL(url.Path);
                //TODO: Do something to show download success or fail.
                var controller = (TabBarController)Window.RootViewController;
                

                if (controller != null)
                {
                    controller.SelectedIndex = 2;
                    var nav = (UINavigationController)controller.SelectedViewController;
                    var media = (MyMediaViewController)nav.VisibleViewController;
                    if (media != null)
                    {
                        if (newFolderDir != null)
                        {
                            media.ChangeDirectory(newFolderDir);
                            return true;
                        }
                    }
                }

                




            return false;
        }

        static public bool CheckVersion(int MajorVersionInt)
        {
            string[] version = UIDevice.CurrentDevice.SystemVersion.Split('.');

            if (MajorVersionInt.ToString() == version[0]) return true;

            return false;
        }
    }
}
