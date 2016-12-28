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
    public class SaladBodyNeedGroup
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public IList<SaladBodyNeedItem> Items { get; private set; }

        public SaladBodyNeedGroup()
        {
            Items = new List<SaladBodyNeedItem>();
        }
    }
}