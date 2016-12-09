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
using Android.Support.V4.View;
using Saladio.Components;
using Android.Support.V4.Content;
using Saladio.Adapters;
using Saladio.Contexts;
using Saladio.Models;
using Saladio.Fragments;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class ActivityOrderScheduled : Activity
    {
        private enum WizardPage
        {
            SelectSalad = 0,
            SelectDeliveryDate,
            SelectDeliveryHour,
            SelectDeliveryAddress,
            Total
        }

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
                    return (int)WizardPage.Total;
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

        private ManualViewPager mWizardContainer;
        private Button mBtnWizard;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ActivityOrderScheduled);

            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom);
            ActionBar.SetCustomView(Resource.Layout.ActionBar);

            mWizardContainer = FindViewById<ManualViewPager>(Resource.Id.wizardContainer);
            mBtnWizard = FindViewById<Button>(Resource.Id.btnWizard);

            mWizardContainer.Adapter = new WizardPagerAdapter(OnInitializePage, GetPageLayout);

            mBtnWizard.Click += BtnWizard_Click;
            SwitchToPage(WizardPage.SelectSalad);
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

        private SavedSalad mSelectedSalad;

        private void OnInitializePage(WizardPage page, View view)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                    using (SaladioContext context = new SaladioContext())
                    {
                        ListView lstSelectSalad = view.FindViewById<ListView>(Resource.Id.lstSelectSalad);
                        mSelectedSalad = null;

                        SavedSaladGroupAdapter adapter = new SavedSaladGroupAdapter(this, context.SavedSaladGroups);
                        adapter.SavedSaladSelected += (sender, args) =>
                        {
                            adapter.ClearSelectedStateSalad();
                            adapter.SetSelectedStateSalad(args.SavedSalad, true);
                            mSelectedSalad = args.SavedSalad;

                            adapter.NotifyDataSetChanged();
                        };

                        lstSelectSalad.Adapter = adapter;
                    }
                    break;
                case WizardPage.SelectDeliveryDate:
                    using (FragmentTransaction transaction = FragmentManager.BeginTransaction())
                    {
                        FragmentDatePicker datePicker = new FragmentDatePicker();

                        transaction.Replace(Resource.Id.layoutDatePickerContainer, datePicker);
                        transaction.Commit();
                    }
                    break;
                case WizardPage.SelectDeliveryHour:
                    using (SaladioContext context = new SaladioContext())
                    {
                        ListView lstSelectDeliveryHour = view.FindViewById<ListView>(Resource.Id.lstSelectDeliveryHour);

                        ItemSelectorAdapter adapter = new ItemSelectorAdapter(this, context.DeliveryHours.Select(x => x.From + " - " + x.To).ToList());
                        lstSelectDeliveryHour.Adapter = adapter;
                    }
                    break;
                case WizardPage.SelectDeliveryAddress:
                    using (SaladioContext context = new SaladioContext())
                    {
                        ListView lstSelectDeliveryAddress = view.FindViewById<ListView>(Resource.Id.lstSelectDeliveryAddress);

                        ItemSelectorAdapter adapter = new ItemSelectorAdapter(this, context.DeliveryAddresses);
                        lstSelectDeliveryAddress.Adapter = adapter;
                    }
                    break;
                default:
                    break;
            }
        }

        private string GetPageButtonText(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                    return Resources.GetString(Resource.String.SelectSaladToSchedule);
                case WizardPage.SelectDeliveryDate:
                    return Resources.GetString(Resource.String.SelectSaladDeliveryDate);
                case WizardPage.SelectDeliveryHour:
                    return Resources.GetString(Resource.String.SelectSaladDeliveryHour);
                case WizardPage.SelectDeliveryAddress:
                    return Resources.GetString(Resource.String.SelectDeliveryAddress);
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private void OnWizardButtonClicked(WizardPage page, View view)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                    SwitchToPage(WizardPage.SelectDeliveryDate);
                    break;
                case WizardPage.SelectDeliveryDate:
                    SwitchToPage(WizardPage.SelectDeliveryHour);
                    break;
                case WizardPage.SelectDeliveryHour:
                    SwitchToPage(WizardPage.SelectDeliveryAddress);
                    break;
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private int GetPageLayout(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                    return Resource.Layout.PageSelectSalad;
                case WizardPage.SelectDeliveryDate:
                    return Resource.Layout.PageSelectDeliveryDate;
                case WizardPage.SelectDeliveryHour:
                    return Resource.Layout.PageSelectDeliveryHour;
                case WizardPage.SelectDeliveryAddress:
                    return Resource.Layout.PageSelectDeliveryAddress;
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private void SwitchToPage(WizardPage page)
        {
            mWizardContainer.SetCurrentItem((int)page, true);
            string text = GetPageButtonText(page);

            if (text != null)
            {
                mBtnWizard.Visibility = ViewStates.Visible;
                mBtnWizard.Text = text;
            }
            else
            {
                mBtnWizard.Visibility = ViewStates.Gone;
            }
        }

        private void BtnWizard_Click(object sender, EventArgs e)
        {
            OnWizardButtonClicked((WizardPage)mWizardContainer.CurrentItem, mWizardContainer.GetChildAt(mWizardContainer.CurrentItem));
        }
    }
}