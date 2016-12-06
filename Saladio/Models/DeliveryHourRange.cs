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
    public class DeliveryHourRange
    {
        public int From { get; set; }
        public int To { get; set; }
    }
}