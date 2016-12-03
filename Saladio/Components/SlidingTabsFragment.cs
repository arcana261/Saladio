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
using Android.Support.V4.View;
using Java.Lang;
using Saladio.Components.Impl;

namespace Saladio.Components
{
    public class SlidingTabsFragment : Fragment
    {
        private SlidingTabScrollView slidingTabScrollView;
        private ViewPager viewPager;
        private SlidingTabsAdapter customAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FragmentSlidingTabs, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            slidingTabScrollView = view.FindViewById<SlidingTabScrollView>(Resource.Id.slidingTabs);
            viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);

            viewPager.Adapter = customAdapter != null ? customAdapter : new SamplePagerAdapter();
            slidingTabScrollView.ViewPager = viewPager;
        }

        public SlidingTabsAdapter Adapter
        {
            get { return viewPager != null ? (SlidingTabsAdapter)viewPager.Adapter : customAdapter; }
            set
            {
                if (viewPager == null)
                {
                    customAdapter = value;
                }
                else
                {
                    viewPager.Adapter = value;
                }
            }
        }

        private class SamplePagerAdapter : SlidingTabsAdapter
        {
            private IList<string> items = new List<string>();

            public SamplePagerAdapter()
            {
                items.Add("Xamarin");
                items.Add("Android");
                items.Add("Tutorial");
                items.Add("Part");
                items.Add("12");
                items.Add("Hooray");

            }
            public override int Count
            {
                get
                {
                    return items.Count;
                }
            }

            public override string GetHeaderTitle(int position)
            {
                return items[position];
            }

            protected override View CreateItem(Context context, ViewGroup container, int position)
            {
                View view = LayoutInflater.From(context).Inflate(Resource.Layout.PagerItem, container, false);

                TextView txtTitle = view.FindViewById<TextView>(Resource.Id.txtItemTitle);
                txtTitle.Text = (position + 1).ToString();

                return view;
            }
        }
    }
}