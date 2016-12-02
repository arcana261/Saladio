using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Saladio.Fragments
{
    public class FragmentWizard : Fragment
    {
        private Fragment mFragment;
        private Button mBtnWizard;

        public event EventHandler RequestedNext;

        public FragmentWizard(Fragment fragment)
        {
            mFragment = fragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mBtnWizard = view.FindViewById<Button>(Resource.Id.btnWizard);
            mBtnWizard.Click += BtnWizard_Click;

            using (var transaction = FragmentManager.BeginTransaction())
            {
                transaction.Replace(Resource.Id.layoutContent, mFragment);
                transaction.Commit();
            }
        }

        private void BtnWizard_Click(object sender, EventArgs e)
        {
            RequestedNext(this, e);
        }

        public Button GetButton(WizardButton button)
        {
            switch (button)
            {
                case WizardButton.Next:
                    return mBtnWizard;
                default:
                    throw new ArgumentException("requested unknown button: " + button.ToString(), "button");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FragmentWizard, container, false);
        }
    }
}