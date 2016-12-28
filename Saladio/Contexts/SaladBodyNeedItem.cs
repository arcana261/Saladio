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
    [Serializable]
    public class SaladBodyNeedItem
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public decimal? Required { get; set; }
        public decimal? Provided { get; set; }
        public string Unit { get; set; }
    }
}