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

namespace Saladio.Models
{
    public class OrderSchedule
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int LaunchCount { get; set; }
        public int DinnerCount { get; set; }
    }
}