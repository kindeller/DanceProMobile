using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreGraphics;
using DancePro.Models;
using UIKit;
using Xamarin.iOS.iCarouselBinding;

namespace DancePro.iOS.ViewControllers
{
    public partial class MediaObjectViewController : UIViewController, INotifyPropertyChanged, IMediaObjectController
    {
        public List<MediaObject> MediaList { get; set; }
        public MediaObject MediaObject { get; set; }
        UIScrollView ScrollView;
        UIImageView ImageView;

        public event PropertyChangedEventHandler PropertyChanged;

        public MediaObjectViewController(IntPtr intPtr) : base(intPtr)
        {
            AppDelegate.CanRotate = true;
            MediaList = new List<MediaObject>();
            PropertyChanged += MediaObjectViewController_PropertyChanged;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            UpdateMediaObjectView();
            UpdateMediaObjectsUI();
            NavigationController.NavigationBar.Hidden = true;
            TabBarController.TabBar.Hidden = true;

            List<MediaObject> remove = new List<MediaObject>();
            foreach (var media in MediaList)
            {
                if(media.MediaType != MediaTypes.Image)
                {
                    remove.Add(media);
                }
            }

            foreach (var item in remove)
            {
                MediaList.Remove(item);
            }

        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            UpdateMediaObjectView();
        }

        public override void ViewDidDisappear(bool animated)
        {
            AppDelegate.CanRotate = false;
        }

        private void UpdateMediaObject(MediaObject obj)
        {
            MediaObject = obj;
            OnPropertyChanged("MediaObject");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UpdateMediaObjectsUI()
        {
            //TODO: Add Carousel View to bottom of view for quick access to other item
        }



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        void MediaObjectViewController_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if(e.PropertyName.Equals("MediaObject"))
            {
                UpdateMediaObjectView();
            }
        }


        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if(ImageView != null && ScrollView != null)
            {
                UpdateMediaObjectView();
            }

        }

        private void RemoveSubviews()
        {
            foreach( var subview in View.Subviews)
            {
                subview.RemoveFromSuperview();
                subview.Dispose();
            }
        }


        private void UpdateMediaObjectView()
        {
            RemoveSubviews();
            //View.Add(MediaObject.GetDetailView(this));
            var view = GetSubview();
            if(view != null)
            {
                View.AddSubview(view);
            }
        }

        private UIView GetSubview()
        {
            //Verify MediaObject as ImageObject
            ImageObject imageObject = (ImageObject)MediaObject;
            if (imageObject == null) return null;


            //CGRect rect = new CGRect(0, NavigationController.NavigationBar.Frame.Height, View.Frame.Width, View.Frame.Height - (TabBarController.TabBar.Frame.Height + NavigationController.NavigationBar.Frame.Height));
            CGRect rect = View.Frame;
            ScrollView = new UIScrollView(rect);
            ImageView = new UIImageView(new CGRect(0, 0, rect.Width, rect.Height));
            //ImageView = new UIImageView(imageObject.Image);
            ScrollView.ShowsHorizontalScrollIndicator = false;
            ScrollView.ShowsVerticalScrollIndicator = false;
            ImageView.Image = imageObject.Image;
            ScrollView.AddSubview(ImageView);
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ScrollView.ContentSize = new CGSize(View.Frame.Width, View.Frame.Height);
            ScrollView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageView.Center = ScrollView.Center;
            ScrollView.MinimumZoomScale = 1f;
            ScrollView.MaximumZoomScale = 10f;
            ScrollView.ViewForZoomingInScrollView += (view) => { return ImageView; };

            ScrollView.DidZoom += (object sender, EventArgs e) => {
                var scale = ScrollView.ZoomScale;
            };

            UISwipeGestureRecognizer recognizer = new UISwipeGestureRecognizer();
            recognizer.Direction = UISwipeGestureRecognizerDirection.Right;
            recognizer.AddTarget((obj) => {

                var r = obj as UISwipeGestureRecognizer;
                HandleSwipe(r);
            
            });
            ScrollView.AddGestureRecognizer(recognizer);
            recognizer = new UISwipeGestureRecognizer();
            recognizer.Direction = UISwipeGestureRecognizerDirection.Left;
            recognizer.AddTarget((obj) => {

                var r = obj as UISwipeGestureRecognizer;
                HandleSwipe(r);

            });
            ScrollView.AddGestureRecognizer(recognizer);
            //recognizer = new UISwipeGestureRecognizer();
            //recognizer.Direction = UISwipeGestureRecognizerDirection.Down;
            //recognizer.AddTarget((obj) => {

            //    DismissViewController(true, null);

            //});
            //ScrollView.AddGestureRecognizer(recognizer);
            ScrollView.AddGestureRecognizer(new UITapGestureRecognizer((e) => {

                //TODO: Perform Hide and reveal of Image UI
                //ToolbarsView.Hidden = !ToolbarsView.Hidden;
                NavigationController.NavigationBar.Hidden = !NavigationController.NavigationBar.Hidden;
                TabBarController.TabBar.Hidden = !TabBarController.TabBar.Hidden;

            }));
            return ScrollView;
        }

        private void HandleSwipe(UISwipeGestureRecognizer rec)
        {
            if(ScrollView.ZoomScale == 1)
            {
                switch (rec.Direction)
                {
                    case UISwipeGestureRecognizerDirection.Left:
                        ChangeImage(GetNextImage());
                        break;
                    case UISwipeGestureRecognizerDirection.Right:
                        ChangeImage(GetPreviousImage());

                        break;
                    case UISwipeGestureRecognizerDirection.Up:
                        DismissViewController(true, null);
                        break;

                }
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }


        public void ChangeImage(MediaObject obj)
        {
            if (obj == null) return;
            MediaObject = obj;
            ImageView.Image = new UIImage(MediaObject.FilePath);
        }

        public MediaObject GetNextImage()
        {
            for (int i = 0; i < MediaList.Count; i++)
            {
                if (MediaList[i].FilePath == MediaObject.FilePath)
                {
                    if (++i >= MediaList.Count) return null;

                    return MediaList[i];
                }
            }

            return null;
        }

        public MediaObject GetPreviousImage()
        {
            for (int i = 0; i < MediaList.Count; i++)
            {
                if (MediaList[i].FilePath == MediaObject.FilePath)
                {
                    if (i == 0) return null;

                    return MediaList[--i];
                }
            }

            return null;
        }



    }
}

