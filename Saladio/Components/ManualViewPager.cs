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
using Android.Views.Animations;

namespace Saladio.Components
{
    public class ManualViewPager : ViewPager
    {
        public ManualViewPager(Context context) : this(context, null) { }
        public ManualViewPager(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return false;
        }
    }
}
