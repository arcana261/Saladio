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

namespace Saladio.Adapters
{
    public class ItemSelectorAdapter : BaseAdapter<string>
    {
        private Context mContext;
        private IList<string> mItems;

        public override string this[int position]
        {
            get
            {
                return mItems[position];
            }
        }

        public override int Count
        {
            get
            {
                return mItems.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowItemSelector, parent, false);
            }

            RadioButton radioItemSelected = row.FindViewById<RadioButton>(Resource.Id.radioItemSelected);
            string item = this[position];

            radioItemSelected.Text = item;

            return row;
        }
    }
}