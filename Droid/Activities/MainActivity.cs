using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;


namespace DancePro.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait,
        NoHistory = false)]
    public class MainActivity : BaseActivity, iOnFragmentChangeListener
    {
        protected override int LayoutResource => Resource.Layout.activity_main;

        ViewPager pager;
        TabsAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            //Check if Intent is Zip origin

            if (Intent.GetType().Equals("application/zip"))
            {
                //its opened from a zip.
                var data = Intent.Data;
                var newFolderDir = App.MediaService.UnzipURL(data.Path);
            }


            //Continue as normal

            adapter = new TabsAdapter(this, SupportFragmentManager);
            pager = FindViewById<ViewPager>(Resource.Id.viewpager);
            var tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            pager.Adapter = adapter;
            tabs.SetupWithViewPager(pager);
            pager.OffscreenPageLimit = 3;
            

            pager.PageSelected += (sender, args) =>
            {
                var fragment = adapter.InstantiateItem(pager, args.Position) as IFragmentVisible;

                fragment?.BecameVisible();
            };

            //Toolbar.MenuItemClick += (sender, e) =>
            //{
            //    var intent = new Intent(this, typeof(AddItemActivity)); ;
            //    StartActivity(intent);
            //};

            //SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            //SupportActionBar.SetHomeButtonEnabled(false);


            CopyAssets assets = new CopyAssets();
            assets.CopyAllAssets();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void ChangeFragment(int id)
        {
            pager.SetCurrentItem(id, true);
        }
    }

    class TabsAdapter : FragmentStatePagerAdapter
    {
        string[] titles;

        public override int Count => titles.Length;
        

        public TabsAdapter(Context context, Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
            titles = context.Resources.GetTextArray(Resource.Array.sections);
        }


        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) =>
                new Java.Lang.String(titles[position]);


        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return MenuFragment.NewInstance();
                case 1: return TransferMediaFragment.NewInstance();
                case 2: return MyMediaFragment.NewInstance();
                case 3: return ContactFragment.NewInstance();
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag) => PositionNone;
    }

    public interface iOnFragmentChangeListener
    {
        void ChangeFragment(int id);
    }
}
