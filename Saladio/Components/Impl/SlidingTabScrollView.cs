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
using Android.Support.V4.View;
using Android.Util;
using Android.Graphics;

namespace Saladio.Components.Impl
{
    public class SlidingTabScrollView : HorizontalScrollView
    {
        private const int TITLE_OFFSET_DIPS = 24;
        private const int TAB_VIEW_PADDING_DIPS = 16;
        private const int TAB_VIEW_TEXT_SIZE_SP = 12;

        private int titleOffset;

        private int tabViewLayoutId;
        private int tabViewTextViewId;

        private ViewPager viewPager;
        private ViewPager.IOnPageChangeListener viewPagerPageChangeListener;

        private SlidingTabStrip tabStrip;

        private int scrollState;

        public interface TabColorizer
        {
            int GetIndicatorColor(int position);

            int GetDividerColor(int position);
        }

        public SlidingTabScrollView(Context context) : this(context, null) { }
        public SlidingTabScrollView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }

        public SlidingTabScrollView(Context context, IAttributeSet attrs, int defaultStyle) : base(context, attrs, defaultStyle)
        {
            // Disable the scrollbar
            HorizontalScrollBarEnabled = false;

            // Make sure the tab strips fill the view
            FillViewport = true;
            SetBackgroundColor(Color.Rgb(0xE5, 0xE5, 0xE5)); // Grey color

            float density = Resources.DisplayMetrics.Density;

            titleOffset = (int)(TITLE_OFFSET_DIPS * density);

            tabStrip = new SlidingTabStrip(context);
            AddView(tabStrip, LayoutParams.MatchParent, LayoutParams.MatchParent);
        }

        public TabColorizer CustomTabColorizer
        {
            get { return tabStrip.CustomTabColorizer; }
            set { tabStrip.CustomTabColorizer = value; }
        }

        public int[] SelectedIndicatorColors
        {
            get { return tabStrip.SelectedIndicatorColors; }
            set { tabStrip.SelectedIndicatorColors = value; }
        }

        public int[] DividerColors
        {
            get { return tabStrip.DividerColors; }
            set { tabStrip.DividerColors = value; }
        }

        public ViewPager.IOnPageChangeListener OnPageChangeListener
        {
            get { return viewPagerPageChangeListener; }
            set { viewPagerPageChangeListener = value; }
        }

        public ViewPager ViewPager
        {
            get { return viewPager; }
            set
            {
                tabStrip.RemoveAllViews();

                if (viewPager != null)
                {
                    viewPager.PageSelected -= ViewPager_PageSelected;
                    viewPager.PageScrollStateChanged -= ViewPager_PageScrollStateChanged;
                    viewPager.PageScrolled -= ViewPager_PageScrolled;
                }

                viewPager = value;
                if (value != null)
                {
                    viewPager.PageSelected += ViewPager_PageSelected;
                    viewPager.PageScrollStateChanged += ViewPager_PageScrollStateChanged;
                    viewPager.PageScrolled += ViewPager_PageScrolled;
                    PopulateTabStrip();
                }
            }
        }

        private void ViewPager_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            int tabCount = tabStrip.ChildCount;

            if ((tabCount == 0) || (e.Position < 0) || (e.Position >= tabCount))
            {
                // If any of these conditions apply, return, no need to scroll
                return;
            }

            tabStrip.OnViewerPagerPageChanged(e.Position, e.PositionOffset);

            View selectedTitle = tabStrip.GetChildAt(e.Position);

            int extraOffset = (selectedTitle != null ? (int)(e.Position * selectedTitle.Width) : 0);

            ScrollToTab(e.Position, extraOffset);

            if (OnPageChangeListener != null)
            {
                OnPageChangeListener.OnPageScrolled(e.Position, e.PositionOffset, e.PositionOffsetPixels);
            }
        }

        private void ViewPager_PageScrollStateChanged(object sender, ViewPager.PageScrollStateChangedEventArgs e)
        {
            scrollState = e.State;

            if (OnPageChangeListener != null)
            {
                OnPageChangeListener.OnPageScrollStateChanged(e.State);
            }
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (scrollState == ViewPager.ScrollStateIdle)
            {
                tabStrip.OnViewerPagerPageChanged(e.Position, 0.0f);
                ScrollToTab(e.Position, 0);
            }

            if (OnPageChangeListener != null)
            {
                OnPageChangeListener.OnPageSelected(e.Position);
            }
        }

        private void PopulateTabStrip()
        {
            SlidingTabsAdapter adapter = (SlidingTabsAdapter)ViewPager.Adapter;

            for (int i = 0; i < adapter.Count; i++)
            {
                TextView tabView = CreateDefaultTabView(Context);
                tabView.Text = adapter.GetHeaderTitle(i);
                tabView.SetTextColor(Color.Black);
                tabView.Tag = i;
                tabView.Click += TabView_Click;
                tabStrip.AddView(tabView);
            }
        }

        private void TabView_Click(object sender, EventArgs e)
        {
            TextView clickTab = (TextView)sender;
            int pageToScrollTo = (int)clickTab.Tag;
            ViewPager.CurrentItem = pageToScrollTo;
        }

        private TextView CreateDefaultTabView(Context context)
        {
            TextView textView = new TextView(context);
            textView.Gravity = GravityFlags.Center;
            textView.SetTextSize(ComplexUnitType.Sp, TAB_VIEW_TEXT_SIZE_SP);
            textView.Typeface = Typeface.DefaultBold;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                TypedValue outValue = new TypedValue();
                context.Theme.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground, outValue, false);
                textView.SetBackgroundResource(outValue.ResourceId);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                textView.SetAllCaps(true);
            }

            int padding = (int)(TAB_VIEW_PADDING_DIPS * context.Resources.DisplayMetrics.Density);
            textView.SetPadding(padding, padding, padding, padding);

            return textView;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (ViewPager != null)
            {
                ScrollToTab(ViewPager.CurrentItem, 0);
            }
        }

        private void ScrollToTab(int tabIndex, int extraOffset)
        {
            int tabCount = tabStrip.ChildCount;

            if (tabCount == 0 || tabIndex < 0 || tabIndex >= tabCount)
            {
                // No need to go further, don't scroll
                return;
            }

            View selectedChild = tabStrip.GetChildAt(tabIndex);
            if (selectedChild != null)
            {
                int scrollAmountX = selectedChild.Left + extraOffset;

                if (tabIndex > 0 || extraOffset > 0)
                {
                    scrollAmountX -= titleOffset;
                }

                ScrollTo(scrollAmountX, 0);
            }
        }
    }
}