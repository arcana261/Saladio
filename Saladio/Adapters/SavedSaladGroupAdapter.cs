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
    public class SavedSaladGroupAdapter : BaseAdapter<SavedSaladGroup>
    {
        private Context mContext;
        private IList<SavedSaladGroup> mSavedSalads;
        private ISet<int> mExpandableGroups;

        public SavedSaladGroupAdapter(Context context, IList<SavedSaladGroup> savedSalads)
        {
            mContext = context;
            mSavedSalads = savedSalads;
        }

        public override SavedSaladGroup this[int position]
        {
            get
            {
                return mSavedSalads[position];
            }
        }

        public override int Count
        {
            get
            {
                return mSavedSalads.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public ISet<int> ExpandableGroups
        {
            get
            {
                if (mExpandableGroups == null)
                {
                    mExpandableGroups = new HashSet<int>();
                }

                return mExpandableGroups;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSavedSaladGroup, parent, false);
            }

            SavedSaladGroup item = this[position];
            TextView txtSavedSaladGroup = row.FindViewById<TextView>(Resource.Id.txtSavedSaladGroup);
            LinearLayout layoutSavedSalads = row.FindViewById<LinearLayout>(Resource.Id.layoutSavedSalads);

            txtSavedSaladGroup.Text = item.Name;

            int dividerMargin = (int)mContext.Resources.GetDimension(Resource.Dimension.DividerMargin);
            View lastSubRow = null;
            layoutSavedSalads.RemoveAllViews();
            foreach (SavedSalad savedSalad in item.Salads)
            {
                View subRow = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSavedSalad, layoutSavedSalads, false);
                if (lastSubRow == null)
                {
                    subRow.SetPadding(0, dividerMargin, 0, 0);
                }
                lastSubRow = subRow;

                TextView txtSaladTitle = subRow.FindViewById<TextView>(Resource.Id.txtSaladTitle);
                TextView txtSaladDescription = subRow.FindViewById<TextView>(Resource.Id.txtSaladDescription);

                txtSaladTitle.Text = savedSalad.Name;
                txtSaladDescription.Text = savedSalad.Ingredients;

                layoutSavedSalads.AddView(subRow);
            }

            if (lastSubRow != null)
            {
                lastSubRow.SetPadding(0, dividerMargin, 0, dividerMargin);

                LinearLayout layoutContainer = lastSubRow.FindViewById<LinearLayout>(Resource.Id.layoutContainer);

                layoutContainer.RemoveView(lastSubRow.FindViewById<View>(Resource.Id.divider));
            }

            return row;
        }
    }
}