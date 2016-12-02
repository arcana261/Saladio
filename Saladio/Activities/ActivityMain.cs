using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Saladio.Components;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/ic_pie_salad_64")]
    public class ActivityMain : RtlActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityMain);

            // Create your application here
            SlidingTabsFragment slidingTabs = new SlidingTabsFragment();
            using (var transaction = FragmentManager.BeginTransaction())
            {
                transaction.Replace(Resource.Id.contentFragment, slidingTabs);
                transaction.Commit();
            }
        }
    }
}