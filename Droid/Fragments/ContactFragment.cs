
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DancePro.Droid
{
    public class ContactFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        private readonly Dictionary<string, Uri> ContactLinks = new Dictionary<string, Uri>()
        {
            {"FacebookApp", new Uri("fb://profile/217475155027428")},
            {"FacebookWeb", new Uri("https://www.facebook.com/DanceProPhotoVideo/")},
            {"InstagramApp", new Uri("instagram://user?username=danceprophoto")},
            {"InstagramWeb", new Uri("https://instagram.com/danceprophoto")},
            {"Mail", new Uri("Mailto:info@dancepro.com.au")},
            {"Website", new Uri("https://www.dancepro.com.au/")}
        };
        ICommand OpenWebsiteCommand = new Command(() => Plugin.Share.CrossShare.Current.OpenBrowser("https://www.dancepro.com.au/"));

        public void BecameVisible()
        {

        }

        public static ContactFragment NewInstance() => new ContactFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View V = inflater.Inflate(Resource.Layout.fragment_contact, container, false);

            //View Setup
            //TODO: Fix this 
            ImageButton fb = V.FindViewById<ImageButton>(Resource.Id.facebookButton);
            fb.Click += (sender, e) =>
            {
                TryOpenContact("com.facebook.katana", "fb://profile/217475155027428", "http://www.facebook.com/DanceProPhotoVideo/");
            };

            ImageButton insta = V.FindViewById<ImageButton>(Resource.Id.instaButton);
            insta.Click += (sender, e) =>
            {
                TryOpenContact("com.instagram.android", "instagram://user?username=danceprophoto", "https://instagram.com/danceprophoto");
            };

            ImageButton email = V.FindViewById<ImageButton>(Resource.Id.emailButton);
            email.Click += (sender, e) =>
            {
                Intent intent = new Intent(Intent.ActionSend);
                intent.SetType("text/html");
                intent.PutExtra(Intent.ExtraEmail, "info@dancepro.com.au");
                Activity.StartActivity(intent);

            };

            ImageButton website = V.FindViewById<ImageButton>(Resource.Id.websiteButton);
            website.Click += (sender, e) =>
            {
                OpenWebsiteCommand.Execute(null);
            };

            return V;
        }

        void TryOpenContact(string PackageString, string AppLink, string WebLink)
        {
            Intent intent;
            Android.Net.Uri uri;
            try
            {
                //try open FB App
                PackageInfo info = Context.PackageManager.GetPackageInfo(PackageString, 0);
                uri = new Android.Net.Uri.Builder().EncodedPath(AppLink).Build();
                intent = new Intent(Intent.ActionView, uri);
                intent.AddFlags(ActivityFlags.NewTask);
                Context.ApplicationContext.StartActivity(intent);
                //Context.StartActivity(intent);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                //Open Web by default
                Plugin.Share.CrossShare.Current.OpenBrowser(WebLink);
            }
        }
    }
}
