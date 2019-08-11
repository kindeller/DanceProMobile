
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

using DancePro.ViewModels;
using DancePro.Models;

namespace DancePro.Droid
{
    public class TransferMediaFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        TransferViewModel ViewModel;
        Button btnEnable;
        TextView connectedText;

        public void BecameVisible()
        {

        }

        public static TransferMediaFragment NewInstance() => new TransferMediaFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = new TransferViewModel(App.NetworkService);

            View view = inflater.Inflate(Resource.Layout.fragment_TransferMedia, container, false);
            connectedText = view.FindViewById<TextView>(Resource.Id.textConnected);
            connectedText.Text = ViewModel.GetDeviceText();
            btnEnable = view.FindViewById<Button>(Resource.Id.btnEnable);
            btnEnable.Click += (sender, e) => {

                ViewModel.ToggleConnection();
                ToggleButtonText();
            };
            return view;
        }

        private void ToggleButtonText()
        {
            var text = ViewModel.isNetworkListening() ? "Disable" : "Enable";
            btnEnable.Text = text;
        }
    }
}
