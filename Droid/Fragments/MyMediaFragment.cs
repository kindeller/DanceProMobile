
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
using Android.Provider;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;

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
                if(DetailsViewObject.MediaType == MediaTypes.Folder)
                {
                    AlertDialog a = null;
                    Alert = new AlertDialog.Builder(Context);
                    Alert.SetPositiveButton("Yes", new EventHandler<DialogClickEventArgs>((object obj, DialogClickEventArgs ea) => {
                        var medialist = App.MediaService.GetMediaFromFolder(DetailsViewObject.FilePath);
                        List<MediaObject> failedList = new List<MediaObject>();
                        foreach(var item in medialist)
                        {
                            if (item.MediaType == MediaTypes.Folder || item.MediaType == MediaTypes.Other) continue;
                            if (!SaveToCameraRoll(item))
                            {
                                failedList.Add(item);
                            }
                        }

                        //TOAST LIST OF FAILED SAVES
                    }));
                    Alert.SetTitle("Save Folder?");
                    a = Alert.Create();
                    a.Show();
                    return;
                }
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

        private bool SaveToCameraRoll (MediaObject obj)
        {

            if(Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
               if(ContextCompat.CheckSelfPermission(Context,Android.Manifest.Permission.WriteExternalStorage) != Permission.Granted || ContextCompat.CheckSelfPermission(Context, Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(Activity, new string[] { Android.Manifest.Permission.WriteExternalStorage, Android.Manifest.Permission.ReadExternalStorage }, 0);
                }
            }

            switch (obj.MediaType)
            {
                case MediaTypes.Image:
                    ImageObject Image = obj as ImageObject;
                    try
                    {
                        if (Image != null)
                        {
                            //SAVE IMAGE
                            //Reduced quality save
                            //var bitmap = App.MediaService.GetScaledBitmap(Image, 1);
                            //var returned = MediaStore.Images.Media.InsertImage(Context.ContentResolver,bitmap, Image.FileName, "DancePro-" + Directory.GetParent(Image.FilePath) + "-" + Image.FileName);
                        //This commented out method works, but on the test device causes an out of memory exception. I think the files need to be compressed similar to the way the display cycles through compressing until it can do so without reaching max memory.
                            var returned = MediaStore.Images.Media.InsertImage(Context.ContentResolver, Image.FilePath, Image.FileName, "DancePro-" + Directory.GetParent(Image.FilePath) + "-" + Image.FileName);
                            Console.WriteLine(returned);
                        }
                    }
                    catch (Exception e)
                    {
                        //ERROR toast?
                        Console.WriteLine(e.Message);
                        return false;
                    }
                    break;
                case MediaTypes.Video:
                    try
                    {
                        VideoObject Video = obj as VideoObject;
                        if (Video != null)
                        {
                            //Setup Content URI in gallery
                            var cv = new ContentValues();
                            cv.Put(MediaStore.MediaColumns.Title, Video.FileName);
                            cv.Put(MediaStore.MediaColumns.MimeType, Video.GetMimeType());
                            var time = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                            cv.Put(MediaStore.MediaColumns.DateAdded, time / 1000);
                            cv.Put(MediaStore.MediaColumns.DateModified, time);
                            cv.Put(MediaStore.MediaColumns.Data, Video.FilePath);
                            Android.Net.Uri collection = MediaStore.Video.Media.GetContentUri(Android.OS.Environment.DirectoryMovies);
                            Android.Net.Uri uriSavedVideo = Context.ContentResolver.Insert(MediaStore.Video.Media.ExternalContentUri, cv);

                            //Save File to output URI using OutputStream
                            var pfd = Context.ContentResolver.OpenFileDescriptor(uriSavedVideo, "w");
                            Java.IO.FileInputStream fs = new Java.IO.FileInputStream(pfd.FileDescriptor);
                            var output = Context.ContentResolver.OpenOutputStream(uriSavedVideo);

                            byte[] buf = new byte[8192];
                            int len;
                            while ((len = fs.Read(buf)) > 0)
                            {
                                output.Write(buf, 0, len);
                            }
                            output.Close();
                            fs.Close();
                            pfd.Close();
                        }
                            
                    }
                    catch (Exception e)
                    {
                        //ERROR toast?
                        Console.WriteLine(e.Message);
                        return false;
                    }

                    break;
                default:
                    Console.WriteLine("Cannot Save this type of media.");
                    return false;
            }

            return true;

        }
    }
}
