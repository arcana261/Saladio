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
using Android.Util;
using Android.Graphics;

namespace Saladio.Components
{
    public class WrapContentListView : ListView
    {
        public WrapContentListView(Context context) : this(context, null) { }
        public WrapContentListView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public WrapContentListView(Context context, IAttributeSet attrs, int defaultStyle) : base(context, attrs, defaultStyle) { }

        protected override void OnDraw(Canvas canvas)
        {
            int newHeight = 1;
            if (Count > 0)
            {
                newHeight += GetChildAt(0).Height * Count;
            }

            LayoutParameters.Height = newHeight;

            base.OnDraw(canvas);
        }
    }
}