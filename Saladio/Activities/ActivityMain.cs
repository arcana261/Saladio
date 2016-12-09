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
    [Activity(Label = "@string/ApplicationName")]
    public class ActivityMain : Activity
    {
        private enum MainTabs
        {
            CustomSalad = 0,
            ClassicSalads,
            SavedSalads,
            OrderSchedule,
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
                    case MainTabs.ClassicSalads:
                        return mActivity.Resources.GetString(Resource.String.TabClassicSalads);
                    case MainTabs.SavedSalads:
                        return mActivity.Resources.GetString(Resource.String.TabSavedSalads);
                    case MainTabs.OrderSchedule:
                        return mActivity.Resources.GetString(Resource.String.TabOrderSchedule);
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
                    case MainTabs.ClassicSalads:
                        using (SaladioContext saladioContext = new SaladioContext())
                        {
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabClassicSalads, container, false);
                            ListView lstClassicSalads = view.FindViewById<ListView>(Resource.Id.lstClassicSalads);

                            lstClassicSalads.Adapter = new SavedSaladGroupAdapter(mActivity, saladioContext.ClassicSaladGroups);

                            return view;
                        }
                    case MainTabs.SavedSalads:
                        using (SaladioContext saladioContext = new SaladioContext())
                        {
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabSavedSalads, container, false);
                            ListView lstSavedSalads = view.FindViewById<ListView>(Resource.Id.lstSavedSalads);

                            SavedSaladGroupAdapter adapter = new SavedSaladGroupAdapter(mActivity, saladioContext.SavedSaladGroups);
                            adapter.ExpandableGroups.Add(1);
                            adapter.InitiallyClosed.Add(1);

                            lstSavedSalads.Adapter = adapter;

                            return view;
                        }
                    case MainTabs.OrderSchedule:
                        using (SaladioContext saladioContext = new SaladioContext()) 
                        {
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabOrderSchedule, container, false);
                            ListView lstOrderSchedule = view.FindViewById<ListView>(Resource.Id.lstOrderSchedule);

                            OrderScheduleCalendarAdapter adapter = new OrderScheduleCalendarAdapter(mActivity, saladioContext.OrderSchedules);
                            lstOrderSchedule.Adapter = adapter;
                            //lstOrderSchedule.SetListViewHeightBasedOnChildren();

                            return view;
                        }
                    default:
                        throw new ArgumentException("unsupported tab: " + tab.ToString());
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityMain);

            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom);
            ActionBar.SetCustomView(Resource.Layout.MainActionBar);

            Button actionBarOrderScheduled = ActionBar.CustomView.FindViewById<Button>(Resource.Id.btnOrderScheduledSalad);
            actionBarOrderScheduled.Click += ActionBarOrderScheduled_Click;

            // Create your application here
            SlidingTabsFragment slidingTabs = new SlidingTabsFragment();
            slidingTabs.Adapter = new TabAdapter(this);

            using (var transaction = FragmentManager.BeginTransaction())
            {
                transaction.Replace(Resource.Id.contentFragment, slidingTabs);
                transaction.Commit();
            }
        }

        private void ActionBarOrderScheduled_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityOrderScheduled));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        { 
            menu.FindItem(Resource.Id.iconLogo).SetEnabled(false);

            return base.OnPrepareOptionsMenu(menu);
        }
    }
}