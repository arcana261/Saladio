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
        private Func<SaladComponentGroup, BaseAdapter<SaladComponent>> mAdapterGenerator;

        public SaladComponentGroupAdapter(Context context, IList<SaladComponentGroup> groups)
            : this(context, groups, x => new SaladComponentAdapter(context, x.Components)) { }

        public SaladComponentGroupAdapter(Context context, IList<SaladComponentGroup> groups, Func<SaladComponentGroup, BaseAdapter<SaladComponent>> adapterGenerator)
        {
            mContext = context;
            mGroups = groups;
            mAdapterGenerator = adapterGenerator;
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

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowSaladComponentGroup, parent, false);
            }

            TextView txtSaladComponentGroup = row.FindViewById<TextView>(Resource.Id.txtSaladComponentGroup);
            ListView lstSaladComponents = row.FindViewById<ListView>(Resource.Id.lstSaladComponents);
            SaladComponentGroup item = this[position];

            txtSaladComponentGroup.Text = item.Name;

            lstSaladComponents.Adapter = mAdapterGenerator(item);

            return row;
        }
    }
}