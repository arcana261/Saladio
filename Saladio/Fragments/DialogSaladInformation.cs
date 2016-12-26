
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Saladio.Adapters;
using Saladio.Contexts;

namespace Saladio.Fragments
{
    public class DialogSaladInformation : DialogFragment
    {
        private SaladListItem mSavedSalad;
        private TextView mTxtSaladTitle;
        private SaladComponentGroupAdapter mComponentAdapter;
        private ListView mLstSaladComponents;
        private bool? mIsEditableInit;

        public DialogSaladInformation(SaladListItem savedSalad)
        {
            mSavedSalad = savedSalad;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.DialogSaladInformation, container, false);
        }

        public bool IsEditable
        {
            get
            {
                return mComponentAdapter.IsEditable;
            }
            set
            {
                if (mComponentAdapter != null)
                {
                    mComponentAdapter.IsEditable = value;
                }
                else
                {
                    mIsEditableInit = value;
                }
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            using (SaladioContext saladioContext = new SaladioContext())
            {
                mComponentAdapter = new SaladComponentGroupAdapter(Context, saladioContext.SaladComponentGroups);
                if (mIsEditableInit != null)
                {
                    mComponentAdapter.IsEditable = mIsEditableInit.Value;
                }
            }

            mTxtSaladTitle = view.FindViewById<TextView>(Resource.Id.txtSaladTitle);
            mLstSaladComponents = view.FindViewById<ListView>(Resource.Id.lstSaladComponents);

            mLstSaladComponents.Adapter = mComponentAdapter;

            mTxtSaladTitle.Text = mSavedSalad.Name;
        }
    }
}