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
using System.Threading.Tasks;
using Saladio.Adapters;
using Saladio.Contexts;

namespace Saladio.Fragments
{
    public class DialogSaladBodyNeeds : DialogFragment
    {
        private ListView mLstBodyNeedGroups;
        private SaladBodyNeedGroupAdapter mAdapter;
        private IList<SaladBodyNeedGroup> mItems;

        public DialogSaladBodyNeeds()
        {
            mItems = new List<SaladBodyNeedGroup>();
        }

        public DialogSaladBodyNeeds(IList<SaladBodyNeedGroup> items)
        {
            mItems = items;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            if (savedInstanceState != null && savedInstanceState.ContainsKey("mItems"))
            {
                mItems = savedInstanceState.GetString("mItems").DeSerializeObject<IList<SaladBodyNeedGroup>>();
            }

            return inflater.Inflate(Resource.Layout.DialogSaladBodyNeeds, container, false);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString("mItems", mItems.SerializeObject());
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mLstBodyNeedGroups = view.FindViewById<ListView>(Resource.Id.lstBodyNeedGroups);
            mAdapter = new SaladBodyNeedGroupAdapter(Application.Context, mItems);
            mLstBodyNeedGroups.Adapter = mAdapter;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}