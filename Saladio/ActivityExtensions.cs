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

namespace Saladio
{
    public static class ActivityExtensions
    {
        public static void ForceRtlIfSupported(this Activity activity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                activity.Window.DecorView.LayoutDirection = LayoutDirection.Rtl;
            }
        }
    }
}