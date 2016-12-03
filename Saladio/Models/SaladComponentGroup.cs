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
    public class SaladComponentGroup
    {
        private IList<SaladComponent> mComponents;

        public string Name { get; set; }

        public IList<SaladComponent> Components
        {
            get
            {
                if (mComponents == null)
                {
                    mComponents = new List<SaladComponent>();
                }

                return mComponents;
            }
        }
    }
}