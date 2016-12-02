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

namespace Saladio.Components
{
    public abstract class SlidingTabsAdapter : PagerAdapter
    {
        protected abstract View CreateItem(Context context, ViewGroup container, int position);

        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view = CreateItem(container.Context, container, position);
            container.AddView(view);

            return view;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
        {
            container.RemoveView((View)obj);
        }

        public abstract string GetHeaderTitle(int position);
    }
}