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
    public class SavedSaladSelectedEventArgs : EventArgs
    {
        public SavedSalad SavedSalad { get; private set; }
        
        public SavedSaladSelectedEventArgs(SavedSalad savedSalad)
        {
            this.SavedSalad = savedSalad;
        }
    }
}