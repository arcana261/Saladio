using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using System;
using Saladio.Components;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/ic_pie_salad_64")]
    public class StartActivity : RtlActivity
    {
        private class WizardPagerAdapter : PagerAdapter
        {
            private Action<WizardPage, View> mInitializePage;
            private Func<WizardPage, int> mGetLayout;

            public WizardPagerAdapter(Action<WizardPage, View> initializePage, Func<WizardPage, int> getLayout)
            {
                mInitializePage = initializePage;
                mGetLayout = getLayout;
            }

            public override int Count
            {
                get
                {
                    return (int) WizardPage.Total;
                }
            }

            public override bool IsViewFromObject(View view, Java.Lang.Object obj)
            {
                return view == obj;
            }

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                View view = LayoutInflater.From(container.Context).Inflate(mGetLayout((WizardPage)position), container, false);
                container.AddView(view);
                mInitializePage((WizardPage)position, view);

                return view;
            }

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
            {
                container.RemoveView((View)obj);
            }
        }

        private enum WizardPage
        {
            Welcome = 0,
            SignUp = 1,
            SignUpDetails = 2,
            Total
        }

        private ManualViewPager mWizardContainer;
        private Button mBtnWizard;
        private View mCurrentPage;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.ActivityStart);

            mWizardContainer = FindViewById<ManualViewPager>(Resource.Id.wizardContainer);
            mBtnWizard = FindViewById<Button>(Resource.Id.btnWizard);

            mWizardContainer.Adapter = new WizardPagerAdapter(OnInitializePage, GetPageLayout);

            mBtnWizard.Click += BtnWizard_Click;
            SwitchToPage(WizardPage.Welcome);
        }

        private void OnInitializePage(WizardPage page, View view)
        {
            
        }

        private string GetPageButtonText(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.Welcome:
                    return Resources.GetString(Resource.String.BtnStart);
                case WizardPage.SignUp:
                case WizardPage.SignUpDetails:
                    return Resources.GetString(Resource.String.BtnNext);
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private void OnWizardButtonClicked(WizardPage page, View view)
        {
            switch (page)
            {
                case WizardPage.Welcome:
                    SwitchToPage(WizardPage.SignUp);
                    break;
                case WizardPage.SignUp:
                    SwitchToPage(WizardPage.SignUpDetails);
                    break;
                case WizardPage.SignUpDetails:
                    break;
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private int GetPageLayout(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.Welcome:
                    return Resource.Layout.PageWelcome;
                case WizardPage.SignUp:
                    return Resource.Layout.PageSignUp;
                case WizardPage.SignUpDetails:
                    return Resource.Layout.PageSignUpDetails;
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private void SwitchToPage(WizardPage page)
        {
            mWizardContainer.SetCurrentItem((int)page, true);
            mBtnWizard.Text = GetPageButtonText(page);
        }

        private void BtnWizard_Click(object sender, EventArgs e)
        {
            OnWizardButtonClicked((WizardPage)mWizardContainer.CurrentItem, mWizardContainer.GetChildAt(mWizardContainer.CurrentItem));
        }
    }
}

