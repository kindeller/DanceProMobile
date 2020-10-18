using System;
using System.Collections.Generic;
using Foundation;
using DancePro.Services;
using DancePro.Models;

using UIKit;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Threading;
using CoreFoundation;

namespace DancePro.iOS.ViewControllers
{
    public partial class MyMediaViewController : UIViewController, IUICollectionViewDragDelegate, IUICollectionViewDropDelegate
    {
        List<MediaObject> MediaObjects = new List<MediaObject>();
        DirectoryInfo CurrentDirectory;
        int DragItemSourceIndex = 0;
        //UIAlertController CurrentNetworkAlert;
        

        public MyMediaViewController(IntPtr intPtr) : base(intPtr)
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Analytics.TrackEvent("[MyMedia] Network Address Changed");
            //if (CurrentNetworkAlert != null)
            //{
            //    if (NetworkService.isListening)
            //    {
            //        CurrentNetworkAlert.Title = NetworkService.Address + ":" + NetworkService.Port;
            //        CurrentNetworkAlert.Message = "For Dance Pro kiosk use only.";
            //    }
            //    else
            //    {
            //        CurrentNetworkAlert.DismissViewController(true, null);
            //        //ConnectSwitch.On = false;
            //    }
            //}
        }


        //partial void OnConnectSwitchChanged(UISwitch sender)
        //{
        //    //If switch turned on
        //    if (sender.On)
        //    {
        //        //check wifi connection
        //        if (!NetworkService.ValidateNetwork())
        //        {
        //            //disconnect if not on wifi
        //            sender.SetState(false, true);
        //            return;
        //        }


        //        List<UIAlertAction> actions = new List<UIAlertAction>();
        //        var action = UIAlertAction.Create("Finish", UIAlertActionStyle.Cancel, alert => { 
        //            NetworkService.Disconnect();
        //            sender.SetState(false, true);
        //            GetMedia();
        //        var disconnectOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) => { });
        //            Alert("Disconnect!", "Please Remember to disconnect your device from our network.", new List<UIAlertAction>() { disconnectOk });
        //        });
        //        actions.Add(action);
        //        NetworkService.Connect();
        //        CurrentNetworkAlert = Alert(NetworkService.Address + ":" + NetworkService.Port, "(For Dance Pro kiosk use only)", actions);
        //    }
        //    else //disconnected programmatically
        //    {
        //        NetworkService.Disconnect();
        //        GetMedia();
        //    }
        //}


        private UIAlertController Alert(string title, string message, List<UIAlertAction> actions)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            foreach (var action in actions)
            {
                alert.AddAction(action);
            }
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            vc.PresentViewController(alert, true, null);
            return alert;
        }

        

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            //MediaCollectionView.Delegate = new MyMediaViewDelegate();
            Analytics.TrackEvent("[MyMedia] Loading View");
            


            //Attempting to change the Estimated size to allow dyamic sizing for cell in collection.

            //var layout = MediaCollectionView.CollectionViewLayout as UICollectionViewFlowLayout;
            //if(layout != null)
            //{
            //    layout.EstimatedItemSize = new CoreGraphics.CGSize(113, 134);
            //}

            MediaCollectionView.DragDelegate = this;
            MediaCollectionView.DropDelegate = this;
            MediaCollectionView.DragInteractionEnabled = true;

            
            //NavigationController.NavigationBar.TopItem.RightBarButtonItem = new UIBarButtonItem(ConnectSwitch);
            var newFolderButton = new UIBarButtonItem("+", UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Analytics.TrackEvent("[MyMedia] Pressed Add Folder Button");
                var alert = UIAlertController.Create("New Folder", "New folder name...", UIAlertControllerStyle.Alert);
                var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
                var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) =>
                {
                    var folderName = alert.TextFields[0].Text;
                    if (App.MediaService.isValidFolderName(folderName))
                    {
                        App.MediaService.CreateFolder(folderName, CurrentDirectory.FullName);
                        GetMedia();
                        Analytics.TrackEvent("[MyMedia] Created New Folder: ",new Dictionary<string, string>()
                        {
                            {"FolderName",folderName},
                            {"DirectoryPath",CurrentDirectory.FullName}
                        });
                    }


                });
                alert.AddTextField((obj) => obj.Placeholder = "Folder Name");
                alert.AddAction(actionCancel);
                alert.AddAction(actionOk);
                PresentViewController(alert, true, null);
            })
            {
                TintColor = UIColor.DarkTextColor
            };
            NavigationController.NavigationBar.TopItem.LeftBarButtonItem = newFolderButton;

            //TODO: Search button version - Not ideal update UI to have a search bar
            //var searchButton = new UIBarButtonItem(UIImage.FromBundle("DanceProLogoIcon"), UIBarButtonItemStyle.Plain, (sender, e) =>
            //  {

            //      Analytics.TrackEvent("[MyMedia] Pressed Search Button");
            //      var alert = UIAlertController.Create("Search", "Enter Search Criteria", UIAlertControllerStyle.Alert);
            //      var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            //      var actionOk = UIAlertAction.Create("Go", UIAlertActionStyle.Default, (obj) =>
            //      {
            //          var search = alert.TextFields[0].Text;
            //          SearchMedia(search);

            //      });
            //      alert.AddTextField((obj) => obj.Placeholder = "");
            //      alert.AddAction(actionCancel);
            //      alert.AddAction(actionOk);
            //      PresentViewController(alert, true, null);

            //  });
            //NavigationController.NavigationBar.TopItem.RightBarButtonItem = searchButton;
            UISearchBar searchBar = new UISearchBar();
            var height = MediaCollectionView.Frame.Size.Height / 12;
            CoreGraphics.CGRect frame = new CoreGraphics.CGRect(
                MediaCollectionView.Frame.Location.X,
                MediaCollectionView.Frame.Location.Y,
                MediaCollectionView.Frame.Size.Width,
                height);
            searchBar.Frame = frame;
            frame = new CoreGraphics.CGRect(
                MediaCollectionView.Frame.Location.X,
                MediaCollectionView.Frame.Location.Y - height,
                MediaCollectionView.Frame.Size.Width,
                MediaCollectionView.Frame.Size.Height - height
                );
            MediaCollectionView.Frame = frame;
            MediaCollectionView.AddSubview(searchBar);
            searchBar.Hidden = true;

        }

        private async void SearchMedia(string searchText)
        {

            //TODO: Add String validation
            if (string.IsNullOrWhiteSpace(searchText))
            { 
                return;
            }

            List<MediaObject> objects = await App.MediaService.SearchAsync("",searchText);
            MediaObjects = objects;
            ReloadMediaList();

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
            {
                GetMedia();
            });
            Analytics.TrackEvent("[MyMedia] Appeared");
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.

            Analytics.TrackEvent("[MyMedia] Recieved Memory Warning");
        }
        public void ChangeDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            ChangeDirectory(dir);
        }

        public void ChangeDirectory(DirectoryInfo info)
        {
            if (info.Exists)
            {
                CurrentDirectory = info;
                //Title = CurrentDirectory.Name;
                GetMedia();
                Analytics.TrackEvent("[MyMedia] Changed Directory");
            }
            else
            {
                IOException e = new IOException("Failed to change directory, no such directory exists with path" + info.FullName);
                Crashes.TrackError(e);
                throw e;
            }

        }

        /// <summary>
        /// Updates the MediaObject List from the Media Service using the current directory.
        /// </summary>
        public void GetMedia()
        {
            
            //ReloadMediaList();
            DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
            {
                MediaObjects.Clear();
                if (CurrentDirectory != null && CurrentDirectory.Exists)
                {

                    MediaObjects = App.MediaService.GetMediaFromFolder(CurrentDirectory.FullName);
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        InvokeOnMainThread(()=> ReloadMediaList());
                    });
                }
                else
                {
                    ChangeDirectory(App.MediaService.GetMediaPathDirectory());
                }
            });
        }

        /// <summary>
        /// Reloads the media list from the media Objects, adding Return object if not in root directory.
        /// </summary>
        private void ReloadMediaList()
        {
            //var currPath = Path.GetFullPath(CurrentDirectory.FullName);
            //var root = Path.GetFullPath(App.MediaService.GetMediaPath());
            //var result = string.Compare(currPath, root);

            //if (result > 0)
            //{
            //    MediaObject up = new MediaObject(CurrentDirectory.Parent.FullName)
            //    {
            //        MediaType = MediaTypes.Other,
            //        Thumb = UIImage.FromBundle("Back")
            //    };
            //    up.FileName = "Back: " + up.FileName;
            //    MediaObjects.Insert(0, up);
            //    //MediaObjects.Add(up);
            //}

            if (MediaObjects.Count > 0)
            {
                MyMediaUICollectionSource source = new MyMediaUICollectionSource(MediaObjects, this);
                MediaCollectionView.ShowsVerticalScrollIndicator = false;
                MediaCollectionView.Source = source;

            }


            InvokeOnMainThread(()=> MediaCollectionView.ReloadData());
        }

        /// <summary>
        /// Prepares for segue: Passing information necessary to the correct controller.
        /// </summary>
        /// <param name="segue">Segue.</param>
        /// <param name="sender">Sender.</param>
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            
            //Open Media Object 
                IMediaObjectController controller = segue.DestinationViewController as IMediaObjectController;
                if (controller != null)
                {
                    var cell = (MyMediaViewCell)sender;
                    if (cell != null)
                    {
                    switch (cell.MediaObject.MediaType)
                    {
                        case MediaTypes.Image:
                            Analytics.TrackEvent("[MyMedia] Opening Image Media");
                            controller.MediaObject = (ImageObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            segue.DestinationViewController.Title = controller.MediaObject.FileName;
                            break;
                        case MediaTypes.Audio:
                            Analytics.TrackEvent("[MyMedia] Opening Audio Media");
                            controller.MediaObject = (AudioObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            segue.DestinationViewController.Title = controller.MediaObject.FileName;
                            break;
                        case MediaTypes.Video:
                            Analytics.TrackEvent("[MyMedia] Opening Video Media");
                            controller.MediaObject = (VideoObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            segue.DestinationViewController.Title = controller.MediaObject.FileName;
                            break;
                        default:
                            Analytics.TrackEvent("[MyMedia] Unknown Media Opened");
                            break;
                    }


                    }
                }
                //End Open Media Object


                //Edit Options Menu
                MediaObjectEditController c = segue.DestinationViewController as MediaObjectEditController;

                if (c != null)
                {
                    var cell = (MyMediaViewCell)sender;
                    if(cell != null)
                    {
                    Analytics.TrackEvent("[MyMedia] Opening Edit Menu",new Dictionary<string, string> { {"FileName",cell.MediaObject.FileName} });
                    c.MediaObject = cell.MediaObject;
                        c.controller = this;
                    }
                }
                //End Edit Options Menu
        }

        /// <summary>
        /// If the view appears: Reloads the Media to view.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            GetMedia();
        }


        /// <summary>
        /// Gets the items for beginning drag session: Handles converting the dragged item and setting the local object.
        /// </summary>
        /// <returns>The items for beginning drag session.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="session">Session.</param>
        /// <param name="indexPath">Index path.</param>
        public UIDragItem[] GetItemsForBeginningDragSession(UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
        {
            MyMediaViewCell cell = collectionView.CellForItem(indexPath) as MyMediaViewCell;
            if(cell != null && cell.MediaObject.MediaType != MediaTypes.Other)
            {
                Console.WriteLine($"[MyMedia] Attempting Drag of Media Item {cell.MediaObject.FileName}");
                Analytics.TrackEvent($"[MyMedia] Attempting Drag of Media Item {cell.MediaObject.FileName}");
                DragItemSourceIndex = int.Parse(indexPath.Item.ToString());
                NSItemProvider provider = new NSItemProvider(cell, "object");
                UIDragItem item = new UIDragItem(provider);
                item.LocalObject = cell;
                return new UIDragItem[] { item };
            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// Performs the drop: 
        /// </summary>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="coordinator">Coordinator.</param>
        public void PerformDrop(UICollectionView collectionView, IUICollectionViewDropCoordinator coordinator)
        {
            switch (coordinator.Proposal.Operation)
            {
                case UIDropOperation.Move:
                    //if destination index is a folder, Call media service and move file.

                    var cell = collectionView.CellForItem(coordinator.DestinationIndexPath) as MyMediaViewCell;

                    if(cell != null && (cell.MediaObject.MediaType == MediaTypes.Folder || cell.MediaObject.MediaType == MediaTypes.Other))
                    {
                        Analytics.TrackEvent($"[MyMedia] Dropping Media Item into {cell.MediaObject.FileName}");
                        //foreach (var item in coordinator.Items)
                        //{
                        //    MyMediaViewCell mediaViewCell = (MyMediaViewCell)item.DragItem.LocalObject;
                        //    if(mediaViewCell != null)
                        //    {
                        //        Console.WriteLine($"[MyMedia] Dropping Media Item {mediaViewCell.MediaObject.FileName} into {cell.MediaObject.FileName}");


                        //        if (mediaViewCell.MediaObject.MediaType == MediaTypes.Folder) //Item is a folder
                        //        {
                        //            //Attempt move Folder
                        //            if(App.MediaService.MoveFolder(mediaViewCell.MediaObject, cell.MediaObject))
                        //            {
                        //                //If successful change directory to new directory.
                        //                ChangeDirectory(CurrentDirectory.FullName);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            //Attempt move Media Object
                        //            if (App.MediaService.MoveMediaObject(mediaViewCell.MediaObject, cell.MediaObject.FilePath))
                        //            {
                        //                //If successful change directory to new directory.
                        //                ChangeDirectory(CurrentDirectory.FullName);
                        //            }
                        //        }


                        //    }
                        //}
                        MyMediaUICollectionSource source = (MyMediaUICollectionSource)collectionView.WeakDelegate;
                        MediaObject dragItemMediaObject = source.MediaObjects[DragItemSourceIndex];

                        if (dragItemMediaObject.MediaType == MediaTypes.Folder) //Item is a folder
                        {
                            //Attempt move Folder
                            if (App.MediaService.MoveFolder(dragItemMediaObject, cell.MediaObject))
                            {
                                //If successful change directory to new directory.
                                ChangeDirectory(CurrentDirectory.FullName);
                            }
                        }
                        else
                        {
                            //Attempt move Media Object
                            if (App.MediaService.MoveMediaObject(dragItemMediaObject, cell.MediaObject.FilePath))
                            {
                                //If successful change directory to new directory.
                                ChangeDirectory(CurrentDirectory.FullName);
                            }
                        }


                    }
                    else
                    {
                        //if not, reorder the object
                        foreach (var item in coordinator.Items)
                        {
                            MyMediaViewCell mediaViewCell = (MyMediaViewCell)item.DragItem.LocalObject;
                            if (mediaViewCell != null)
                            {
                                var oldIndex = collectionView.IndexPathForCell(mediaViewCell);

                                collectionView.MoveItem(oldIndex, coordinator.DestinationIndexPath);
                            }
                        }
                    }
                    break;

                default:
                    break;

            }



        }


        [Export("collectionView:dropSessionDidUpdate:withDestinationIndexPath:")]
        public UICollectionViewDropProposal DropSessionDidUpdate(UICollectionView collectionView, IUIDropSession session, NSIndexPath destinationIndexPath)
        {
            if(session.LocalDragSession != null)
            {
                if (collectionView.HasActiveDrag)
                {
                    return new UICollectionViewDropProposal(UIDropOperation.Move);
                }
                else
                {
                    return new UICollectionViewDropProposal(UIDropOperation.Copy);
                }
            }
            else
            {
                return new UICollectionViewDropProposal(UIDropOperation.Forbidden);
            }
        }

    }
}

