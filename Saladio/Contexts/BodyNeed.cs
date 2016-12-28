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

namespace Saladio.Contexts
{
    public class BodyNeed
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
    }
}