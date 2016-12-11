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

namespace Saladio.Fragments
{
    public class CalendarDatePickedEventArgs : CalendarDateSelectedEventArgs
    {
        public CalendarDatePickedEventArgs(int year, int month, int day) : base(year, month, day)
        {
        }
    }
}