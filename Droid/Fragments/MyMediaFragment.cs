
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

namespace DancePro.Droid
{
    public class MyMediaFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public void BecameVisible()
        {
          
        }

        public static MyMediaFragment NewInstance() => new MyMediaFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_mymedia, container, false);
            return view;
        }
    }
}
