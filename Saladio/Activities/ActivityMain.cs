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
using Saladio.Adapters;
using Saladio.Contexts;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/ic_pie_salad_64")]
    public class ActivityMain : RtlActivity
    {
        private enum MainTabs
        {
            CustomSalad = 0,
            About,
            Total
        }

        private class TabAdapter : SlidingTabsAdapter
        {
            private Activity mActivity;

            public TabAdapter(Activity activity)
            {
                mActivity = activity;
            }

            public override int Count
            {
                get
                {
                    return (int)MainTabs.Total;
                }
            }

            public override string GetHeaderTitle(int position)
            {
                MainTabs tab = (MainTabs)position;

                switch (tab)
                {
                    case MainTabs.CustomSalad:
                        return mActivity.Resources.GetString(Resource.String.TabCustomSalad);
                    case MainTabs.About:
                        return mActivity.Resources.GetString(Resource.String.TabAbout);
                    default:
                        throw new ArgumentException("unsupported tab: " + tab.ToString());
                }
            }

            protected override View CreateItem(Context context, ViewGroup container, int position)
            {
                MainTabs tab = (MainTabs)position;

                switch (tab)
                {
                    case MainTabs.CustomSalad:
                        using (SaladioContext saladioContext = new SaladioContext())
                        {
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabOrderCustomSalad, container, false);
                            ListView lstCustomSalad = view.FindViewById<ListView>(Resource.Id.lstCustomSalad);

                            lstCustomSalad.Adapter = new SaladComponentGroupAdapter(mActivity, saladioContext.SaladComponentGroups);
                            lstCustomSalad.SetListViewHeightBasedOnChildren();

                            return view;
                        }
                    case MainTabs.About:
                        return LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabAbout, container, false);
                    default:
                        throw new ArgumentException("unsupported tab: " + tab.ToString());
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityMain);

            // Create your application here
            SlidingTabsFragment slidingTabs = new SlidingTabsFragment();
            slidingTabs.Adapter = new TabAdapter(this);

            using (var transaction = FragmentManager.BeginTransaction())
            {
                transaction.Replace(Resource.Id.contentFragment, slidingTabs);
                transaction.Commit();
            }
        }
    }
}