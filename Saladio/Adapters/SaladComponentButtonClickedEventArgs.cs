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

namespace Saladio.Adapters
{
    public class SaladComponentButtonClickedEventArgs : EventArgs
    {
        public SaladComponent SaladComponent { get; private set; }
        public int OldQuantity { get; private set; }
        public int Quantity { get; private set; }

        public SaladComponentButtonClickedEventArgs(SaladComponent saladComponent, int oldQuantity, int quantity)
        {
            SaladComponent = saladComponent;
            OldQuantity = oldQuantity;
            Quantity = quantity;
        }
    }
}