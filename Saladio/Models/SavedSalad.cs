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
    public class SavedSalad
    {
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public int Callorie { get; set; }
        public int Price { get; set; }
        public byte[] Image { get; set; }
    }
}