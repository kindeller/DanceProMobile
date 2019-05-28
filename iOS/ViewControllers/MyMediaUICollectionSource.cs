
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using DancePro.Models;
using AVKit;
using AVFoundation;

namespace DancePro.iOS.ViewControllers
{
    public partial class MyMediaUICollectionSource : UICollectionViewSource
    {
        public List<MediaObject> MediaObjects { get; set; }

        private MyMediaViewController ViewController { get; set; }

        public MyMediaUICollectionSource(List<MediaObject> list, MyMediaViewController controller) : base()
        {
            MediaObjects = list;
            ViewController = controller;
        }



        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return MediaObjects.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {

            var cell = (MyMediaViewCell)collectionView.DequeueReusableCell(MyMediaViewCell.CellID, indexPath);
            MediaObject mediaObject = MediaObjects[indexPath.Row];
            cell.UpdateRow(mediaObject,ViewController);
            return cell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            return base.GetViewForSupplementaryElement(collectionView, elementKind, indexPath);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MyMediaViewCell cell = (MyMediaViewCell)collectionView.CellForItem(indexPath);


            if(cell.MediaObject.MediaType == MediaTypes.Folder)
            {
                ViewController.ChangeDirectory(cell.MediaObject.FilePath);
                return;
            }



            if (cell != null && cell.MediaObject.MediaType != MediaTypes.Other)
            {
                if(cell.MediaObject.MediaType == MediaTypes.Video)
                {
                    var video = (VideoObject)cell.MediaObject;
                    var AVPlayerController = new AVPlayerViewController();
                    AVPlayerController.Player = new AVFoundation.AVPlayer(video.PlayerItem);
                    try
                    {
                        using(var session = AVAudioSession.SharedInstance())
                        {
                            session.SetCategory(AVAudioSessionCategory.Playback);
                            session.SetActive(true);
                        }

                    }
                    catch
                    {
                        AVPlayerController.Player.Muted = false;
                    }
                    AVPlayerController.Player.Play();
                    ViewController.PresentViewController(AVPlayerController, true, null);
                }
                else
                {
                    NSString segueString = cell.MediaObject.SegueString;

                    if (!string.IsNullOrEmpty(segueString))
                    {
                        ViewController.PerformSegue(segueString, cell);
                    }
                }
            }
            else
            {
                ViewController.ChangeDirectory(cell.MediaObject.FilePath);
            }
        }

    }
}

