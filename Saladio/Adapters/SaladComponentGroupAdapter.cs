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
    public class SaladComponentGroupAdapter : BaseAdapter<SaladComponentGroup>
    {
        private Context mContext;
        private IList<SaladComponentGroup> mGroups;
        private bool mEditable;
        private Dictionary<int, int> mQuantities = new Dictionary<int, int>();

        public SaladComponentGroupAdapter(Context context, IList<SaladComponentGroup> groups)
        {
            mContext = context;
            mGroups = groups;
            mEditable = true;
        }

        public event EventHandler<SaladComponentButtonClickedEventArgs> SaladComponetButtonPlusClicked;
        public event EventHandler<SaladComponentButtonClickedEventArgs> SaladComponetButtonMinusClicked;

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

        private string CreateButtonTag(int position, int index)
        {
            return position.ToString() + ":" + index.ToString();
        }

        private void ParseButtonTag(object value, out int position, out int index)
        {
            string[] parts = (value.ToString()).Split(':');
            position = int.Parse(parts[0]);
            index = int.Parse(parts[1]);
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
            for (int i = 0; i < item.Items.Count; i++)
            {
                SaladComponent saladComponent = item.Items[i];

                View subRow = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladComponent, lstSaladComponents, false);
                if (lastSubRow == null)
                {
                    subRow.SetPadding(0, dividerMargin, 0, 0);
                }
                lastSubRow = subRow;

                TextView txtSaladComponent = subRow.FindViewById<TextView>(Resource.Id.txtSaladComponent);
                Button btnPlus = subRow.FindViewById<Button>(Resource.Id.btnPlus);
                Button btnMinus = subRow.FindViewById<Button>(Resource.Id.btnMinus);
                TextView txtQuantity = subRow.FindViewById<TextView>(Resource.Id.txtQuantity);

                if (!mQuantities.ContainsKey(saladComponent.Id.Value))
                {
                    mQuantities[saladComponent.Id.Value] = 0;
                }

                txtQuantity.Text = mQuantities[saladComponent.Id.Value].ToString().ToPersianNumbers();

                btnPlus.Tag = CreateButtonTag(position, i);
                btnMinus.Tag = CreateButtonTag(position, i);

                btnPlus.Click -= BtnPlus_Click;
                btnMinus.Click -= BtnMinus_Click;

                btnPlus.Click += BtnPlus_Click;
                btnMinus.Click += BtnMinus_Click;

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

        public int GetQuantity(SaladComponent saladComponent)
        {
            if (!mQuantities.ContainsKey(saladComponent.Id.Value))
            {
                return 0;
            }

            return mQuantities[saladComponent.Id.Value];
        }

        public void SetQuantity(SaladComponent saladComponent, int count)
        {
            mQuantities[saladComponent.Id.Value] = count;
            NotifyDataSetChanged();
        }

        private void BtnMinus_Click(object sender, EventArgs e)
        {
            Button btnMinus = (Button)sender;
            int position;
            int index;

            ParseButtonTag(btnMinus.Tag, out position, out index);
            SaladComponent saladComponent = this[position].Items[index];
            int prevQuantity = GetQuantity(saladComponent);
            int nextQuantity = prevQuantity - 1;
            if (nextQuantity >= 0)
            {
                SetQuantity(saladComponent, nextQuantity);
            }
            else
            {
                nextQuantity = prevQuantity;
            }

            SaladComponetButtonMinusClicked?.Invoke(this, new SaladComponentButtonClickedEventArgs(saladComponent, prevQuantity, nextQuantity));
        }

        private void BtnPlus_Click(object sender, EventArgs e)
        {
            Button btnPlus = (Button)sender;
            int position;
            int index;

            ParseButtonTag(btnPlus.Tag, out position, out index);
            SaladComponent saladComponent = this[position].Items[index];
            int prevQuantity = GetQuantity(saladComponent);
            int nextQuantity = prevQuantity + 1;
            SetQuantity(saladComponent, nextQuantity);

            SaladComponetButtonPlusClicked?.Invoke(this, new SaladComponentButtonClickedEventArgs(saladComponent, prevQuantity, nextQuantity));
        }
    }
}