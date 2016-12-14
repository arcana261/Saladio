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
    public class SaladComponentGroupAdapter : BaseAdapter<SaladComponentGroup>
    {
        private Context mContext;
        private IList<SaladComponentGroup> mGroups;
        private bool mEditable;

        public SaladComponentGroupAdapter(Context context, IList<SaladComponentGroup> groups)
        {
            mContext = context;
            mGroups = groups;
            mEditable = true;
        }

        public override SaladComponentGroup this[int position]
        {
            get
            {
                return mGroups[position];   
            }
        }

        public override int Count
        {
            get
            {
                return mGroups.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public bool IsEditable
        {
            get
            {
                return mEditable;
            }
            set
            {
                mEditable = value;
                NotifyDataSetChanged();
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladComponentGroup, parent, false);
            }

            TextView txtSaladComponentGroup = row.FindViewById<TextView>(Resource.Id.txtSaladComponentGroup);
            LinearLayout lstSaladComponents = row.FindViewById<LinearLayout>(Resource.Id.layoutSaladComponents);
            SaladComponentGroup item = this[position];

            txtSaladComponentGroup.Text = item.Name;

            int dividerMargin = (int)mContext.Resources.GetDimension(Resource.Dimension.DividerMargin);
            View lastSubRow = null;
            lstSaladComponents.RemoveAllViews();
            foreach (SaladComponent saladComponent in item.Components)
            {
                View subRow = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladComponent, lstSaladComponents, false);
                if (lastSubRow == null)
                {
                    subRow.SetPadding(0, dividerMargin, 0, 0);
                }
                lastSubRow = subRow;

                TextView txtSaladComponent = subRow.FindViewById<TextView>(Resource.Id.txtSaladComponent);
                Button btnPlus = subRow.FindViewById<Button>(Resource.Id.btnPlus);
                Button btnMinus = subRow.FindViewById<Button>(Resource.Id.btnMinus);

                if (mEditable)
                {
                    btnPlus.Visibility = ViewStates.Visible;
                    btnMinus.Visibility = ViewStates.Visible;
                }
                else
                {
                    btnPlus.Visibility = ViewStates.Invisible;
                    btnMinus.Visibility = ViewStates.Invisible;
                }

                txtSaladComponent.Text = saladComponent.Name;

                lstSaladComponents.AddView(subRow);
            }

            if (lastSubRow != null)
            {
                lastSubRow.SetPadding(0, 0, 0, dividerMargin);

                LinearLayout layoutContainer = lastSubRow.FindViewById<LinearLayout>(Resource.Id.layoutContainer);

                layoutContainer.RemoveView(lastSubRow.FindViewById<View>(Resource.Id.divider));
            }

            return row;
        }
    }
}