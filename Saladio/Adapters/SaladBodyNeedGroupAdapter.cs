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
using Saladio.Contexts;

namespace Saladio.Adapters
{
    public class SaladBodyNeedGroupAdapter : BaseAdapter<SaladBodyNeedGroup>
    {
        private Context mContext;
        private IList<SaladBodyNeedGroup> mItems;

        public SaladBodyNeedGroupAdapter(Context context, IList<SaladBodyNeedGroup> items)
        {
            mContext = context;
            mItems = items;
        }

        public override SaladBodyNeedGroup this[int position]
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladMoreInformationGroup, parent, false);
            }

            TextView txtBodyNeedGroup = row.FindViewById<TextView>(Resource.Id.txtBodyNeedGroup);
            LinearLayout layoutBodyNeeds = row.FindViewById<LinearLayout>(Resource.Id.layoutBodyNeeds);
            SaladBodyNeedGroup item = this[position];

            string providedNotAvailable = mContext.Resources.GetString(Resource.String.body_need_provided_not_available);
            string requiredNotAvailable = mContext.Resources.GetString(Resource.String.body_need_required_not_available);
            string percent = mContext.Resources.GetString(Resource.String.body_need_unit_percent);

            txtBodyNeedGroup.Text = item.FriendlyName;

            int dividerMargin = (int)mContext.Resources.GetDimension(Resource.Dimension.DividerMargin);
            View lastSubRow = null;
            layoutBodyNeeds.RemoveAllViews();

            {
                View subRow = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladMoreInformation, layoutBodyNeeds, false);
                if (lastSubRow == null)
                {
                    subRow.SetPadding(0, dividerMargin, 0, 0);
                }
                lastSubRow = subRow;

                TextView txtItemLeft = subRow.FindViewById<TextView>(Resource.Id.txtItemLeft);
                TextView txtItemMiddle = subRow.FindViewById<TextView>(Resource.Id.txtItemMiddle);
                TextView txtItemRight = subRow.FindViewById<TextView>(Resource.Id.txtItemRight);

                txtItemRight.Text = mContext.Resources.GetString(Resource.String.body_need_header_item);
                txtItemMiddle.Text = mContext.Resources.GetString(Resource.String.body_need_header_provided);
                txtItemLeft.Text = mContext.Resources.GetString(Resource.String.body_need_header_required);

                layoutBodyNeeds.AddView(subRow);
            }

            for (int i = 0; i < item.Items.Count; i++)
            {
                SaladBodyNeedItem bodyNeedItem = item.Items[i];

                View subRow = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladMoreInformation, layoutBodyNeeds, false);
                if (lastSubRow == null)
                {
                    subRow.SetPadding(0, dividerMargin, 0, 0);
                }
                lastSubRow = subRow;

                TextView txtItemLeft = subRow.FindViewById<TextView>(Resource.Id.txtItemLeft);
                TextView txtItemMiddle = subRow.FindViewById<TextView>(Resource.Id.txtItemMiddle);
                TextView txtItemRight = subRow.FindViewById<TextView>(Resource.Id.txtItemRight);

                txtItemRight.Text = bodyNeedItem.FriendlyName;

                if (bodyNeedItem.Provided.HasValue)
                {
                    txtItemMiddle.Text = Math.Round(bodyNeedItem.Provided.Value, 2).ToString().ToPersianNumbers() + " " + bodyNeedItem.Unit;
                }
                else
                {
                    txtItemMiddle.Text = providedNotAvailable;
                }

                if (bodyNeedItem.Required.HasValue)
                {
                    if (bodyNeedItem.Provided.HasValue)
                    {
                        if (bodyNeedItem.Provided.Value > 0)
                        {
                            txtItemLeft.Text = Math.Round((bodyNeedItem.Provided.Value / bodyNeedItem.Required.Value) * 100, 2).ToString().ToPersianNumbers() + " " + percent;
                        }
                        else
                        {
                            txtItemLeft.Text = (0).ToString().ToPersianNumbers();
                        }
                    }
                    else
                    {
                        txtItemLeft.Text = providedNotAvailable;
                    }
                }
                else
                {
                    txtItemLeft.Text = requiredNotAvailable;
                }

                layoutBodyNeeds.AddView(subRow);
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