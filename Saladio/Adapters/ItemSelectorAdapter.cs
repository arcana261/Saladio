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
        private int? mSelectedPosition;

        public ItemSelectorAdapter(Context context, IList<string> items)
        {
            mContext = context;
            mItems = items;
        }

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
            TextView radioItemText = row.FindViewById<TextView>(Resource.Id.radioItemText);
            string item = this[position];

            radioItemText.Text = item;
            radioItemText.Tag = radioItemSelected;
            radioItemText.Click -= RadioItemText_Click;
            radioItemText.Click += RadioItemText_Click;

            radioItemSelected.Tag = position;
            radioItemSelected.CheckedChange -= RadioItemSelected_CheckedChange;
            if (mSelectedPosition.HasValue && mSelectedPosition.Value == position)
            {
                radioItemSelected.Checked = true;
            }
            else
            {
                radioItemSelected.Checked = false;
            }
            radioItemSelected.CheckedChange += RadioItemSelected_CheckedChange;

            return row;
        }

        private void RadioItemSelected_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            RadioButton radioItemSelected = (RadioButton)sender;
            int position = (int)radioItemSelected.Tag;

            mSelectedPosition = position;
            NotifyDataSetChanged();
        }

        private void RadioItemText_Click(object sender, EventArgs e)
        {
            TextView radioItemText = (TextView)sender;
            RadioButton radioItemSelected = (RadioButton)radioItemText.Tag;

            radioItemSelected.Checked = true;
        }
    }
}