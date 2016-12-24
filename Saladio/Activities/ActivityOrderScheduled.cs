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
using System.Threading.Tasks;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class ActivityOrderScheduled : SharedActivity
    {
        private int? mDeliveryYear;
        private int? mDeliveryMonth;
        private int? mDeliveryDay;

        public ActivityOrderScheduled()
        {
            mDeliveryYear = null;
            mDeliveryMonth = null;
            mDeliveryDay = null;
        }

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
        private TextView mTxtActionBarTitle;

        private void SetTitle(string title)
        {
            mTxtActionBarTitle.Text = title;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ActivityOrderScheduled);

            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom);
            ActionBar.SetCustomView(Resource.Layout.ActionBarTitled);
            mTxtActionBarTitle = ActionBar.CustomView.FindViewById<TextView>(Resource.Id.txtActionBarTitle);

            mWizardContainer = FindViewById<ManualViewPager>(Resource.Id.wizardContainer);
            mBtnWizard = FindViewById<Button>(Resource.Id.btnWizard);

            mWizardContainer.Adapter = new WizardPagerAdapter(OnInitializePage, GetPageLayout);

            mBtnWizard.Click += BtnWizard_Click;

            Intent intent = Intent;

            if (intent.HasExtra("year"))
            {
                mDeliveryYear = intent.GetIntExtra("year", -1);
            }

            if (intent.HasExtra("month"))
            {
                mDeliveryMonth = intent.GetIntExtra("month", -1);
            }

            if (intent.HasExtra("day"))
            {
                mDeliveryDay = intent.GetIntExtra("day", -1);
            }

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

        private class DeliveryScheduleGroupComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (!x.Equals(y))
                {
                    if (x.Equals("ناهار"))
                    {
                        return -1;
                    }

                    return 1;
                }

                return 0;
            }
        }

        private SaladListItem mSelectedSalad;

        private void OnInitializePage(WizardPage page, View view)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                    using (SaladioContext context = new SaladioContext())
                    {
                        ListView lstSelectSalad = view.FindViewById<ListView>(Resource.Id.lstSelectSalad);
                        mSelectedSalad = null;

                        Task.Factory.StartNew(() =>
                        {
                            using (OpenLoadingFromThread())
                            {
                                IList<SaladListItemGroup> savedGroups = SaladListItemGroup.GetGroups(DataContext, DataContext.SavedSalads);
                                IList<SaladListItemGroup> classicGroups = SaladListItemGroup.GetGroups(DataContext, DataContext.ClassicSaladCatagories);

                                SaladListItemGroup flatClassicGroup = new SaladListItemGroup()
                                {
                                    Name = "سالادهای کلاسیک",
                                    Items = new List<SaladListItem>()
                                };

                                foreach (var group in classicGroups)
                                {
                                    foreach (var item in group.Items)
                                    {
                                        flatClassicGroup.Items.Add(item);
                                    }
                                }

                                savedGroups.Add(flatClassicGroup);

                                SaladGroupAdapter adapter = new SaladGroupAdapter(this, savedGroups);

                                RunOnUiThread(() =>
                                {
                                    lstSelectSalad.Adapter = adapter;

                                    adapter.SavedSaladSelected += (sender, args) =>
                                    {
                                        adapter.ClearSelectedStateSalad();
                                        adapter.SetSelectedStateSalad(args.SavedSalad, true);
                                        mSelectedSalad = args.SavedSalad;

                                        adapter.NotifyDataSetChanged();
                                    };
                                });
                            }
                        });
                    }
                    break;
                case WizardPage.SelectDeliveryDate:
                    using (FragmentTransaction transaction = FragmentManager.BeginTransaction())
                    {
                        FragmentCalendar calendar = new FragmentCalendar();

                        transaction.Replace(Resource.Id.layoutDatePickerContainer, calendar);
                        transaction.Commit();
                    }
                    break;
                case WizardPage.SelectDeliveryHour:
                    {
                        ListView lstSelectDeliveryHour = view.FindViewById<ListView>(Resource.Id.lstSelectDeliveryHour);

                        Task.Factory.StartNew(() =>
                        {
                            GroupedItemSelectorAdapter adapter = new GroupedItemSelectorAdapter(this,
                                DataContext.DeliveryHours.Select(x => new KeyValuePair<string, string>(
                                    x.Catagory.Value == IO.Swagger.Model.DeliverySchedule.CatagoryEnum.Dinner ? "ناهار" : "شام",
                                    "ساعت " + x.FromHour.ToString().ToPersianNumbers() + " الی " + x.ToHour.ToString().ToPersianNumbers())).ToList(),
                                new DeliveryScheduleGroupComparer());

                            RunOnUiThread(() =>
                            {
                                lstSelectDeliveryHour.Adapter = adapter;
                            });
                        });
                    }
                    break;
                case WizardPage.SelectDeliveryAddress:
                    {
                        ListView lstSelectDeliveryAddress = view.FindViewById<ListView>(Resource.Id.lstSelectDeliveryAddress);

                        Task.Factory.StartNew(() =>
                        {
                            ItemSelectorAdapter adapter = new ItemSelectorAdapter(this, DataContext.DeliveryAddresses);

                            RunOnUiThread(() =>
                            {
                                lstSelectDeliveryAddress.Adapter = adapter;
                            });
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        private string GetPageTitle(WizardPage page)
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

        private string GetPageButtonText(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                case WizardPage.SelectDeliveryDate:
                case WizardPage.SelectDeliveryHour:
                case WizardPage.SelectDeliveryAddress:
                    return Resources.GetString(Resource.String.BtnNext);
                default:
                    throw new ArgumentException("invalid requested page: " + page.ToString(), "page");
            }
        }

        private void OnWizardButtonClicked(WizardPage page, View view)
        {
            switch (page)
            {
                case WizardPage.SelectSalad:
                    if (mDeliveryDay == null || mDeliveryMonth == null || mDeliveryYear == null)
                    {
                        SwitchToPage(WizardPage.SelectDeliveryDate);
                    }
                    else
                    {
                        SwitchToPage(WizardPage.SelectDeliveryHour);
                    }
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
            string title = GetPageTitle(page);

            if (text != null)
            {
                mBtnWizard.Visibility = ViewStates.Visible;
                mBtnWizard.Text = text;
            }
            else
            {
                mBtnWizard.Visibility = ViewStates.Gone;
            }

            if (title != null)
            {
                SetTitle(title);
            }
            else
            {
                SetTitle(Resources.GetString(Resource.String.ApplicationName));
            }
        }

        private void BtnWizard_Click(object sender, EventArgs e)
        {
            OnWizardButtonClicked((WizardPage)mWizardContainer.CurrentItem, mWizardContainer.GetChildAt(mWizardContainer.CurrentItem));
        }
    }
}