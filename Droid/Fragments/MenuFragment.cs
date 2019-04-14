﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DancePro.Droid
{
    public class MenuFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {

        public static MenuFragment NewInstance() => new MenuFragment { Arguments = new Bundle() };

        public static MenuModel ViewModel { get; set; }
        ProgressBar progress;

        public void BecameVisible()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = new MenuModel();

            View view = inflater.Inflate(Resource.Layout.fragment_menu, container, false);
            ImageView imageView = view.FindViewById<ImageView>(Resource.Id.imageViewLogo);
            imageView.SetImageResource(Resource.Drawable.ic_DancePro);
            imageView.SetScaleType(ImageView.ScaleType.FitCenter);
            imageView.SetAdjustViewBounds(true);
            imageView.SetMaxHeight(container.Height / 4);
            imageView.SetMaxWidth(container.Width / 4);
            return view;

        }
    }
}