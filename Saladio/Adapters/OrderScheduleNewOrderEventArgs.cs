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

namespace Saladio.Adapters
{
    public class OrderScheduleNewOrderEventArgs
    {
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }

        public OrderScheduleNewOrderEventArgs(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}