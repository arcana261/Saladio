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
using Saladio.Utility;

namespace Saladio.Adapters
{
    public class SavedSaladGroupAdapter : BaseAdapter<SavedSaladGroup>
    {
        private Context mContext;
        private IList<SavedSaladGroup> mSavedSalads;
        private ISet<int> mExpandableGroups;
        private ISet<int> mInitiallyClosed;

        public SavedSaladGroupAdapter(Context context, IList<SavedSaladGroup> savedSalads)
        {
            mContext = context;
            mSavedSalads = savedSalads;
        }

        public event EventHandler<SavedSaladSelectedEventArgs> SavedSaladSelected;

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

        public ISet<int> InitiallyClosed
        {
            get
            {
                if (mInitiallyClosed == null)
                {
                    mInitiallyClosed = new HashSet<int>();
                }

                return mInitiallyClosed;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSavedSaladGroup, parent, false);
            }

            row.Tag = position;
            SavedSaladGroup item = this[position];
            TextView txtSavedSaladGroup = row.FindViewById<TextView>(Resource.Id.txtSavedSaladGroup);
            LinearLayout layoutSavedSalads = row.FindViewById<LinearLayout>(Resource.Id.layoutSavedSalads);

            txtSavedSaladGroup.Text = item.Name;

            if (ExpandableGroups.Contains(position))
            {
                if (!InitiallyClosed.Contains(position))
                {
                    txtSavedSaladGroup.Text = mContext.Resources.GetString(Resource.String.ExpandableArrowDown) + " " + txtSavedSaladGroup.Text;
                }
                else
                {
                    txtSavedSaladGroup.Text = mContext.Resources.GetString(Resource.String.ExpandableArrowUp) + " " + txtSavedSaladGroup.Text;
                    layoutSavedSalads.Tag = true;
                    layoutSavedSalads.ScaleX = 0.0f;
                    layoutSavedSalads.ScaleY = 0.0f;
                }
                txtSavedSaladGroup.Click -= TxtSavedSaladGroup_Click;
                txtSavedSaladGroup.Click += TxtSavedSaladGroup_Click;
                txtSavedSaladGroup.Tag = row;
            }

            int dividerMargin = (int)mContext.Resources.GetDimension(Resource.Dimension.DividerMargin);
            View lastSubRow = null;
            layoutSavedSalads.RemoveAllViews();
            for (int i = 0; i < item.Salads.Count; i++)
            {
                SavedSalad savedSalad = item.Salads[i];

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

                subRow.Tag = position.ToString() + ":" + i;
                subRow.Click += SubRow_Click;

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

        private void SubRow_Click(object sender, EventArgs e)
        {
            View subRow = (View)sender;
            string tag = (string)subRow.Tag;
            string[] parts = tag.Split(':');

            int position = int.Parse(parts[0]);
            int itemIndex = int.Parse(parts[1]);

            SavedSalad savedSalad = this[position].Salads[itemIndex];

            SavedSaladSelected(this, new SavedSaladSelectedEventArgs(savedSalad));
        }

        private void TxtSavedSaladGroup_Click(object sender, EventArgs e)
        {
            TextView txtSavedSaladGroup = (TextView)sender;
            View row = (View)txtSavedSaladGroup.Tag;
            LinearLayout layoutSavedSalads = row.FindViewById<LinearLayout>(Resource.Id.layoutSavedSalads);

            int position = (int)row.Tag;
            SavedSaladGroup item = this[position];

            if (layoutSavedSalads.Tag == null || ((bool)layoutSavedSalads.Tag) == false)
            {
                txtSavedSaladGroup.Text = mContext.Resources.GetString(Resource.String.ExpandableArrowUp) + " " + item.Name;

                layoutSavedSalads.Tag = true;
                layoutSavedSalads.Animate().ScaleX(0.0f).ScaleY(0.0f).SetDuration(200).Start();
            }
            else
            {
                txtSavedSaladGroup.Text = mContext.Resources.GetString(Resource.String.ExpandableArrowDown) + " " + item.Name;

                layoutSavedSalads.Tag = false;
                layoutSavedSalads.Animate().ScaleX(1.0f).ScaleY(1.0f).SetDuration(200).Start();
            }
        }
    }
}