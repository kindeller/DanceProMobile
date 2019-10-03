
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using DancePro.Models;
using System.IO;
using Xamarin.Essentials;

namespace DancePro.Droid
{
    public class MyMediaFragment : Android.Support.V4.App.Fragment, IFragmentVisible, iShowDetails
    {
        MediaFolder MediaFolder;
        MediaAdapter Adapter;
        RecyclerView.LayoutManager LayoutManager;
        RecyclerView MediaRecyclerView;
        LinearLayout DetailsView;
        MediaObject DetailsViewObject;

        private AlertDialog.Builder Alert;

        public void BecameVisible()
        {
            RefreshMedia();
        }

        public static MyMediaFragment NewInstance() => new MyMediaFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_mymedia, container, false);
            view.Click += (object o, EventArgs e) =>
            {
                if (DetailsView.Enabled) DetailsView.Enabled = false;
            };

            DetailsView = view.FindViewById<LinearLayout>(Resource.Id.DetailsTab);
            DetailsView.Visibility = ViewStates.Invisible;
            DetailsView.Click += (object sender, EventArgs e) =>
            {
                DetailsView.Visibility = ViewStates.Invisible;
            };

            var newfolderbtn = view.FindViewById<ImageButton>(Resource.Id.NewfolderButton);
            newfolderbtn.Click += (object o, EventArgs e) =>
            {
                if (this.Alert != null) return;
                EditText folderNameText = new EditText(Context);
                folderNameText.Text = "New Folder";
                Alert = new AlertDialog.Builder(Context);
                Alert.SetPositiveButton("Done", new EventHandler<DialogClickEventArgs>((object obj, DialogClickEventArgs ea) => {
                    MediaFolder.AddFolder(folderNameText.Text.ToString());
                    Alert = null;
                    RefreshMedia();
                }));
                Alert.SetTitle("New Folder Name");
                Alert.SetView(folderNameText);
                Alert.Show();
            };

            // Create Media / TODO: Get Media
            MediaFolder = new MediaFolder();
            //Create Adapter
            Adapter = new MediaAdapter(MediaFolder, this);
            //Get Recycler View
            MediaRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.MyMediaRecycleView);
            //Set Recyeler View in Adapter
            Adapter.SetRecyclerView(MediaRecyclerView);
            //Create Layout
            LayoutManager = new GridLayoutManager(Context, 3);
            //Set Adapter
            MediaRecyclerView.SetAdapter(Adapter);
            //Set Layout
            MediaRecyclerView.SetLayoutManager(LayoutManager);


            view.FindViewById<ImageButton>(Resource.Id.DetailsShareButton).Click += (object o, EventArgs e) =>
            {
                //TODO context menu for sharing media objects ... make it save for now?
                ShareFile sf = new ShareFile(Path.GetFullPath(DetailsViewObject.FilePath));
                string mime;
                Services.HttpRequestHandler.mimeTypes.TryGetValue(Path.GetExtension(DetailsViewObject.FilePath), out mime);
                sf.ContentType = mime;
                Share.RequestAsync(new ShareFileRequest
                {
                    Title = DetailsViewObject.FileName,
                    File = sf
                });
                DetailsView.Visibility = ViewStates.Invisible;
                RefreshMedia();
            };
            view.FindViewById<ImageButton>(Resource.Id.DetailsDeleteButton).Click += (object o, EventArgs e) =>
            {
                App.MediaService.DeleteMediaObject(DetailsViewObject);
                DetailsView.Visibility = ViewStates.Invisible;
                RefreshMedia();
            };
            view.FindViewById<ImageButton>(Resource.Id.DetailsDuplicateButton).Click += (object o, EventArgs e) =>
            {
                App.MediaService.DuplicateMediaObject(DetailsViewObject);
                DetailsView.Visibility = ViewStates.Invisible;
                RefreshMedia();
            };
            view.FindViewById<ImageButton>(Resource.Id.DetailsRenameButton).Click += (object o, EventArgs e) =>
            {

                if (this.Alert != null) return;
                EditText et = new EditText(Context);

                et.Text = Path.GetFileNameWithoutExtension(DetailsViewObject.FilePath);
                Alert = new AlertDialog.Builder(Context);
                Alert.SetPositiveButton("Done", new EventHandler<DialogClickEventArgs>((object obj, DialogClickEventArgs ea) => {
                    if (App.MediaService.RenameMediaObject(DetailsViewObject, et.Text.ToString())) DetailsView.Visibility = ViewStates.Invisible;
                    RefreshMedia();
                    Alert = null;
                    DetailsView.Visibility = ViewStates.Invisible;

                }));
                Alert.SetTitle("Rename");
                Alert.SetView(et);
                Alert.Create().Show();



            };

            view.FindViewById<Button>(Resource.Id.DetailsCancelButton).Click += (object o, EventArgs e) =>
            {
                DetailsView.Visibility = ViewStates.Invisible;
            };

            return view;

        }

        public void ShowDetailsLayoutFor(MediaObject mediaObject)
        {
            DetailsViewObject = mediaObject;
            DetailsView.Visibility = ViewStates.Visible;
        }

        private void RefreshMedia()
        {
            if (Adapter == null) return;
            Adapter.MediaFolder.RefreshMedia();
            Adapter.NotifyDataSetChanged();
        }

        public void ItemClicked(int position)
        {

        }
    }
}
