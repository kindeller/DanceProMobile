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
            MediaList = new List<MediaObject>();
            PropertyChanged += MediaObjectViewController_PropertyChanged;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            UpdateMediaObjectView();
            UpdateMediaObjectsUI();

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


            CGRect rect = new CGRect(0, NavigationController.NavigationBar.Frame.Height, View.Frame.Width, View.Frame.Height - (TabBarController.TabBar.Frame.Height + NavigationController.NavigationBar.Frame.Height));

            ScrollView = new UIScrollView(rect);
            ImageView = new UIImageView(new CGRect(0, 0, rect.Width, rect.Height));
            //ImageView = new UIImageView(imageObject.Image);
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

            };
            ScrollView.AddGestureRecognizer(new UITapGestureRecognizer((e) => {

                //TODO: Perform Hide and reveal of Image UI
                //ToolbarsView.Hidden = !ToolbarsView.Hidden;
                NavigationController.NavigationBar.Hidden = !NavigationController.NavigationBar.Hidden;
                TabBarController.TabBar.Hidden = !TabBarController.TabBar.Hidden;

            }));
            return ScrollView;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }




    }
}

