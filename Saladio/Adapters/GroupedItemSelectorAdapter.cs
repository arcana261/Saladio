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
    public class GroupedItemSelectorAdapter : BaseAdapter<KeyValuePair<string, IList<string>>>
    {
        private Context mContext;
        private List<KeyValuePair<string, string>> mData;
        private List<string> mGroups;
        private Dictionary<string, IList<string>> mGroupItems;
        private int? mSelectedPosition;
        private int? mSelectedIndex;

        public GroupedItemSelectorAdapter(Context context, List<KeyValuePair<string, string>> data, IComparer<string> groupComparer)
        {
            mContext = context;
            mData = data;

            mGroups = data.Select(x => x.Key).Distinct().ToList();
            if (groupComparer != null)
            {
                mGroups.Sort(groupComparer);
            }

            mGroupItems = mGroups.Select(x => new KeyValuePair<string, IList<string>>(x, data.Where(y => y.Key.Equals(x)).Select(y => y.Value).ToList()))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public GroupedItemSelectorAdapter(Context context, List<KeyValuePair<string, string>> data)
            : this(context, data, null)
        {
        }

        public override int Count
        {
            get
            {
                return mGroups.Count;
            }
        }

        public override KeyValuePair<string, IList<string>> this[int position]
        {
            get
            {
                string key = mGroups[position];
                return new KeyValuePair<string, IList<string>>(key, mGroupItems[key]);
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowItemSelectorGroup, parent, false);
            }

            TextView txtGroupTitle = row.FindViewById<TextView>(Resource.Id.txtGroupTitle);
            LinearLayout layoutChoices = row.FindViewById<LinearLayout>(Resource.Id.layoutChoices);
            KeyValuePair<string, IList<string>> item = this[position];

            txtGroupTitle.Text = item.Key;

            int dividerMargin = (int)mContext.Resources.GetDimension(Resource.Dimension.DividerMargin);
            View lastSubRow = null;
            layoutChoices.RemoveAllViews();
            int index = 0;
            foreach (string choice in item.Value)
            {
                View subRow = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowItemSelector, layoutChoices, false);
                if (lastSubRow == null)
                {
                    subRow.SetPadding(0, dividerMargin, 0, 0);
                }
                lastSubRow = subRow;

                TextView radioItemText = subRow.FindViewById<TextView>(Resource.Id.radioItemText);
                RadioButton radioItemSelected = subRow.FindViewById<RadioButton>(Resource.Id.radioItemSelected);

                radioItemText.Text = choice;
                radioItemText.Click -= RadioItemText_Click;
                radioItemText.Click += RadioItemText_Click;
                radioItemText.Tag = radioItemSelected;

                radioItemSelected.Tag = position.ToString() + "-" + index.ToString();
                radioItemSelected.CheckedChange -= RadioItemSelected_CheckedChange;
                if (mSelectedPosition.HasValue && mSelectedPosition.Value == position && mSelectedIndex.HasValue && mSelectedIndex.Value == index)
                {
                    radioItemSelected.Checked = true;
                }
                else
                {
                    radioItemSelected.Checked = false;
                }
                radioItemSelected.CheckedChange += RadioItemSelected_CheckedChange;

                layoutChoices.AddView(subRow);

                index = index + 1;
            }

            if (lastSubRow != null)
            {
                lastSubRow.SetPadding(0, 0, 0, dividerMargin);

                LinearLayout layoutContainer = lastSubRow.FindViewById<LinearLayout>(Resource.Id.layoutContainer);

                layoutContainer.RemoveView(lastSubRow.FindViewById<View>(Resource.Id.divider));
            }

            return row;
        }

        private void RadioItemSelected_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                RadioButton radioItemSelected = (RadioButton)sender;
                string[] properties = ((string)radioItemSelected.Tag).Split('-');
                int position = int.Parse(properties[0]);
                int index = int.Parse(properties[1]);

                mSelectedPosition = position;
                mSelectedIndex = index;

                NotifyDataSetChanged();
            }
        }

        private void RadioItemText_Click(object sender, EventArgs e)
        {
            TextView radioItemText = (TextView)sender;
            RadioButton radioItemSelected = (RadioButton)radioItemText.Tag;

            radioItemSelected.Checked = true;
        }
    }
}