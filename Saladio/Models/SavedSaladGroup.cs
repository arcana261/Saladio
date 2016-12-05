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
    public class SavedSaladGroup
    {
        private IList<SavedSalad> mSalads;
        public string Name { get; set; }
        public IList<SavedSalad> Salads
        {
            get
            {
                if (mSalads == null)
                {
                    mSalads = new List<SavedSalad>();
                }

                return mSalads;
            }
        }
    }
}