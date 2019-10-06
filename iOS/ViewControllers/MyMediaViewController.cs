using System;
using System.Collections.Generic;
using Foundation;
using DancePro.Services;
using DancePro.Models;

using UIKit;
using System.IO;
using System.Net.NetworkInformation;

namespace DancePro.iOS.ViewControllers
{
    public partial class MyMediaViewController : UIViewController, IUICollectionViewDragDelegate, IUICollectionViewDropDelegate
    {
        List<MediaObject> MediaObjects = new List<MediaObject>();
        DirectoryInfo CurrentDirectory;
        UIAlertController CurrentNetworkAlert;
        NetworkServiceIOS NetworkService;

        public MyMediaViewController(IntPtr intPtr) : base(intPtr)
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            if (CurrentNetworkAlert != null)
            {
                if (NetworkService.isListening)
                {
                    CurrentNetworkAlert.Title = NetworkService.Address + ":" + NetworkService.Port;
                    CurrentNetworkAlert.Message = "For Dance Pro kiosk use only.";
                }
                else
                {
                    CurrentNetworkAlert.DismissViewController(true, null);
                    //ConnectSwitch.On = false;
                }
            }
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
            NetworkService = AppDelegate.NetworkService;
            GetMedia();


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
                var alert = UIAlertController.Create("New Folder", "New folder name...", UIAlertControllerStyle.Alert);
                var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
                var actionOk = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) =>
                {
                    var folderName = alert.TextFields[0].Text;
                    if (App.MediaService.isValidFolderName(folderName))
                    {
                        App.MediaService.CreateFolder(folderName, CurrentDirectory.FullName);
                        GetMedia();
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
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
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
            }
            else
            {
                throw new IOException("Failed to change directory, no such directory exists with path" + info.FullName);
            }

        }

        /// <summary>
        /// Updates the MediaObject List from the Media Service using the current directory.
        /// </summary>
        public void GetMedia()
        {
            MediaObjects.Clear();
            if (CurrentDirectory != null && CurrentDirectory.Exists)
            {

                MediaObjects = App.MediaService.GetMediaFromFolder(CurrentDirectory.FullName);
                ReloadMediaList();
            }
            else
            {
                ChangeDirectory(App.MediaService.GetMediaPathDirectory());
            }
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
            MediaCollectionView.ReloadData();
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
                            controller.MediaObject = (ImageObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            segue.DestinationViewController.Title = controller.MediaObject.FileName;
                            break;
                        case MediaTypes.Audio:
                            controller.MediaObject = (AudioObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            segue.DestinationViewController.Title = controller.MediaObject.FileName;
                            break;
                        case MediaTypes.Video:
                            controller.MediaObject = (VideoObject)cell.MediaObject;
                            controller.MediaList.AddRange(MediaObjects);
                            segue.DestinationViewController.Title = controller.MediaObject.FileName;
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
                        foreach(var item in coordinator.Items)
                        {
                            MyMediaViewCell mediaViewCell = (MyMediaViewCell)item.DragItem.LocalObject;
                            if(mediaViewCell != null)
                            {
                                if(mediaViewCell.MediaObject.MediaType == MediaTypes.Folder) //Item is a folder
                                {
                                    //Attempt move Folder
                                    if(App.MediaService.MoveFolder(mediaViewCell.MediaObject, cell.MediaObject))
                                    {
                                        //If successful change directory to new directory.
                                        ChangeDirectory(cell.MediaObject.FilePath);
                                    }
                                }
                                else
                                {
                                    //Attempt move Media Object
                                    if (App.MediaService.MoveMediaObject(mediaViewCell.MediaObject, cell.MediaObject.FilePath))
                                    {
                                        //If successful change directory to new directory.
                                        ChangeDirectory(cell.MediaObject.FilePath);
                                    }
                                }


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

