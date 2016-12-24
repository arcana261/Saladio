using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using System;
using Saladio.Components;
using Saladio.Fragments;
using Android.Content.Res;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IO.Swagger.Api;
using Saladio.Config;
using IO.Swagger.Client;
using System.Threading.Tasks;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true)]
    public class StartActivity : SharedActivity
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
        private WizardPage mCurrentPage;
        private IList<WizardPage> mWizardHistory = new List<WizardPage>();

        private View welcomePortraitContainer1;
        private View welcomePortraitContainer2;
        private View welcomeLandscapeContainer;

        private TextView radioFemaleText;
        private RadioButton radioFemaleSelected;
        private TextView radioMaleText;
        private RadioButton radioMaleSelected;

        private void SwitchPortrait()
        {
            welcomePortraitContainer1.Visibility = ViewStates.Visible;
            welcomePortraitContainer2.Visibility = ViewStates.Visible;
            welcomeLandscapeContainer.Visibility = ViewStates.Gone;
        }

        private void SwitchLandscape()
        {
            welcomeLandscapeContainer.Visibility = ViewStates.Visible;
            welcomePortraitContainer1.Visibility = ViewStates.Gone;
            welcomePortraitContainer2.Visibility = ViewStates.Gone;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.ActivityStart);

            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom);
            ActionBar.SetCustomView(Resource.Layout.ActionBar);

            mWizardContainer = FindViewById<ManualViewPager>(Resource.Id.wizardContainer);
            mBtnWizard = FindViewById<Button>(Resource.Id.btnWizard);

            mWizardContainer.Adapter = new WizardPagerAdapter(OnInitializePage, GetPageLayout);

            mBtnWizard.Click += BtnWizard_Click;

            SwitchToPage(WizardPage.Welcome);
        }

        public override void OnBackPressed()
        {
            if (mWizardHistory.Count < 2)
            {
                base.OnBackPressed();
            }

            mWizardHistory.RemoveAt(mWizardHistory.Count - 1);
            SwitchToPage(mWizardHistory[mWizardHistory.Count - 1], false);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.FindItem(Resource.Id.iconLogo).SetEnabled(false);

            return base.OnPrepareOptionsMenu(menu);
        }

        private void OnInitializePage(WizardPage page, View view)
        {
            switch (page)
            {
                case WizardPage.Welcome:
                    {
                        welcomePortraitContainer1 = view.FindViewById<View>(Resource.Id.welcomePortraitContainer1);
                        welcomePortraitContainer2 = view.FindViewById<View>(Resource.Id.welcomePortraitContainer2);
                        welcomeLandscapeContainer = view.FindViewById<View>(Resource.Id.welcomeLandscapeContainer);

                        if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape)
                        {
                            SwitchLandscape();
                        }
                        else
                        {
                            SwitchPortrait();
                        }
                    }
                    break;
                case WizardPage.SignUpDetails:
                    {
                        CalendarPickerEditText etBirthDate = view.FindViewById<CalendarPickerEditText>(Resource.Id.etBirthDate);

                        radioFemaleText = view.FindViewById<TextView>(Resource.Id.radioFemaleText);
                        radioFemaleSelected = view.FindViewById<RadioButton>(Resource.Id.radioFemaleSelected);

                        radioMaleText = view.FindViewById<TextView>(Resource.Id.radioMaleText);
                        radioMaleSelected = view.FindViewById<RadioButton>(Resource.Id.radioMaleSelected);

                        radioFemaleText.Click -= RadioFemaleText_Click;
                        radioFemaleSelected.CheckedChange -= RadioFemaleSelected_CheckedChange;
                        radioFemaleText.Click += RadioFemaleText_Click;
                        radioFemaleSelected.CheckedChange += RadioFemaleSelected_CheckedChange;

                        radioMaleText.Click -= RadioMaleText_Click;
                        radioMaleSelected.CheckedChange -= RadioMaleSelected_CheckedChange;
                        radioMaleText.Click += RadioMaleText_Click;
                        radioMaleSelected.CheckedChange += RadioMaleSelected_CheckedChange;
                    }
                    break;
                default:
                    break;
            }   
        }

        private void RadioMaleSelected_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                radioFemaleSelected.Checked = false;
            }
        }

        private void RadioMaleText_Click(object sender, EventArgs e)
        {
            radioMaleSelected.Checked = true;
        }

        private void RadioFemaleSelected_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                radioMaleSelected.Checked = false;
            }
        }

        private void RadioFemaleText_Click(object sender, EventArgs e)
        {
            radioFemaleSelected.Checked = true;
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
                    {
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                using (var handle = OpenLoading())
                                {
                                    if (SharedConfig.IsRegistrationValid)
                                    {
                                        RunOnUiThread(() =>
                                        {
                                            StartActivity(typeof(ActivityMain));
                                            Finish();
                                        });
                                    }
                                    else
                                    {
                                        RunOnUiThread(() =>
                                        {
                                            SwitchToPage(WizardPage.SignUp);
                                        });
                                    }
                                }
                            }
                            catch(System.Exception e)
                            {
                                RunOnUiThread(() =>
                                {
                                    ShowMessageDialogForException(e);
                                });
                            }
                        });
                    }
                    break;
                case WizardPage.SignUp:
                    {
                        EditText etEmail = FindViewById<EditText>(Resource.Id.etEmail);
                        EditText etPassword = FindViewById<EditText>(Resource.Id.etPassword);
                        EditText etPasswordConfirmation = FindViewById<EditText>(Resource.Id.etPasswordConfirmation);
                        EditText etPhone = FindViewById<EditText>(Resource.Id.etPhone);

                        Regex emailMatcher = new Regex("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$");
                        Regex phoneNumberMatcher = new Regex("^(\\+?989\\d{2}|09\\d{2})\\d{7}$");

                        if (!emailMatcher.IsMatch(etEmail.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastInvalidEmail);
                            break;
                        }

                        if (!etPassword.Text.Equals(etPasswordConfirmation.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastPasswordsDoNotMatch);
                            break;
                        }

                        if (etPassword.Text.Length < 8)
                        {
                            ShowMessageDialog(Resource.String.ToastPasswordMinLength);
                            break;
                        }

                        if (!phoneNumberMatcher.IsMatch(etPhone.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastInvalidPhoneNumber);
                            break;
                        }
                        
                        SwitchToPage(WizardPage.SignUpDetails);
                    }
                    break;
                case WizardPage.SignUpDetails:
                    {
                        EditText etEmail = FindViewById<EditText>(Resource.Id.etEmail);
                        EditText etPassword = FindViewById<EditText>(Resource.Id.etPassword);
                        EditText etPasswordConfirmation = FindViewById<EditText>(Resource.Id.etPasswordConfirmation);
                        EditText etPhone = FindViewById<EditText>(Resource.Id.etPhone);

                        EditText etFirstName = FindViewById<EditText>(Resource.Id.etFirstName);
                        EditText etLastName = FindViewById<EditText>(Resource.Id.etLastName);
                        RadioButton radioMaleSelected = FindViewById<RadioButton>(Resource.Id.radioMaleSelected);
                        RadioButton radioFemaleSelected = FindViewById<RadioButton>(Resource.Id.radioFemaleSelected);
                        CalendarPickerEditText etBirthDate = FindViewById<CalendarPickerEditText>(Resource.Id.etBirthDate);
                        EditText etWeight = FindViewById<EditText>(Resource.Id.etWeight);
                        EditText etAddress1 = FindViewById<EditText>(Resource.Id.etAddress1);
                        EditText etAddress2 = FindViewById<EditText>(Resource.Id.etAddress2);

                        if (string.IsNullOrEmpty(etFirstName.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastFirstNameEmpty);
                            break;
                        }

                        if (string.IsNullOrEmpty(etLastName.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastLastNameEmpty);
                            break;
                        }

                        if (string.IsNullOrEmpty(etAddress1.Text) && string.IsNullOrEmpty(etAddress2.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastAddressEmpty);
                            break;
                        }

                        if (string.IsNullOrEmpty(etBirthDate.Text))
                        {
                            ShowMessageDialog(Resource.String.ToastBirthDateEmpty);
                            break;
                        }

                        List<string> addresses = new List<string>();
                        if (!string.IsNullOrEmpty(etAddress1.Text))
                        {
                            addresses.Add(etAddress1.Text);
                        }
                        if (!string.IsNullOrEmpty(etAddress2.Text))
                        {
                            addresses.Add(etAddress2.Text);
                        }

                        try
                        {
                            UsersApi api = new UsersApi(SharedConfig.UnAuthorizedApiConfig);
                            var res = api.Signup(new IO.Swagger.Model.User(null, etEmail.Text, etPhone.Text,
                                etPassword.Text, etFirstName.Text, etLastName.Text,
                                new IO.Swagger.Model.PersianDate(etBirthDate.SelectedYear, etBirthDate.SelectedMonth, etBirthDate.SelectedDay),
                                radioMaleSelected.Checked ? IO.Swagger.Model.User.GenderEnum.Male : IO.Swagger.Model.User.GenderEnum.Female,
                                !string.IsNullOrEmpty(etWeight.Text) ? (decimal?)decimal.Parse(etWeight.Text) : null, addresses));

                            SharedConfig.UserName = res.UserName;
                            SharedConfig.Password = etPassword.Text;
                        }
                        catch(ApiException e)
                        {
                            if (e.ErrorCode == 409)
                            {
                                ShowMessageDialog(Resource.String.ToastDuplicateEmail);
                                break;
                            }

                            ShowMessageDialogForException(e);
                            break;
                        }
                        catch(System.Exception e)
                        {
                            ShowMessageDialogForException(e);
                            break;
                        }

                        StartActivity(typeof(ActivityMain));
                        Finish();
                    }
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

        private void SwitchToPage(WizardPage page, bool stack = true)
        {
            mWizardContainer.SetCurrentItem((int)page, true);
            mBtnWizard.Text = GetPageButtonText(page);
            mCurrentPage = page;

            if (stack)
            {
                mWizardHistory.Add(page);
            }
        }

        private void BtnWizard_Click(object sender, EventArgs e)
        {
            OnWizardButtonClicked((WizardPage)mWizardContainer.CurrentItem, mWizardContainer.GetChildAt(mWizardContainer.CurrentItem));
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (mCurrentPage == WizardPage.Welcome)
            {
                if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
                {
                    SwitchLandscape();
                }
                else
                {
                    SwitchPortrait();
                }
            }
        }
    }
}

