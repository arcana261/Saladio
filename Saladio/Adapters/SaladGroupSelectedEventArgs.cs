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
using Saladio.Models;

namespace Saladio.Adapters
{
    public class SaladGroupSelectedEventArgs : EventArgs
    {
        public SaladListItem SavedSalad { get; private set; }
        
        public SaladGroupSelectedEventArgs(SaladListItem savedSalad)
        {
            this.SavedSalad = savedSalad;
        }
    }
}