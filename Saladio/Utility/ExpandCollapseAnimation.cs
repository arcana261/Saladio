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
using Android.Views.Animations;

namespace Saladio.Utility
{
    public class ExpandCollapseAnimation : Animation
    {
        private View view;
        private int originalHeight;
        private int targetHeight;
        private int growBy;

        public ExpandCollapseAnimation(View view, int targetHeight)
        {
            this.view = view;
            originalHeight = view.Height;
            this.targetHeight = targetHeight;
            growBy = targetHeight - originalHeight;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            var p = view.LayoutParameters;
            p.Height = (int)(originalHeight + (growBy * interpolatedTime));
            view.LayoutParameters = p;

            view.RequestLayout();
        }

        public override bool WillChangeBounds()
        {
            return true;
        }
    }
}