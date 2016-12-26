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
using IO.Swagger.Model;

namespace Saladio.Contexts
{
    public class GroupedOrderSchedule
    {
        public IList<Order> LaunchOrders { get; set; }
        public IList<Order> DinnerOrders { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}