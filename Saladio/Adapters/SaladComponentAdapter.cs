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
    public class SaladComponentAdapter : BaseAdapter<SaladComponent>
    {
        private Context mContext;
        private IList<SaladComponent> mComponents;

        public SaladComponentAdapter(Context context, IList<SaladComponent> components)
        {
            mContext = context;
            mComponents = components;
        }

        public override SaladComponent this[int position]
        {
            get
            {
                return mComponents[position];
            }
        }

        public override int Count
        {
            get
            {
                return mComponents.Count;
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladComponent, parent, false);
            }

            TextView txtSaladComponent = row.FindViewById<TextView>(Resource.Id.txtSaladComponent);
            SaladComponent item = this[position];

            txtSaladComponent.Text = item.Name;

            return row;
        }
    }
}