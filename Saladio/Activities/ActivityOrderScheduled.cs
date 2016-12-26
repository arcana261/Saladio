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
using Saladio.Fragments;
using System.Threading.Tasks;
using IO.Swagger.Model;
using IO.Swagger.Api;
using Saladio.Config;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class ActivityOrderScheduled : SharedActivity
    {
        private int? mDeliveryYear;
        private int? mDeliveryMonth;
        private int? mDeliveryDay;
        private List<PickedSaladComponent> mPickedComponents;
        private string mCustomSaladDescription;

        public ActivityOrderScheduled()
        {
            mDeliveryYear = null;
            mDeliveryMonth = null;
            mDeliveryDay = null;
            mPickedComponents = null;
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

            if (intent.HasExtra("saladComponentIds"))
            {
                mPickedComponents = intent.GetStringExtra("saladComponentIds").Split(',')
                    .Select(x => int.Parse(x))
                    .Select(x => new PickedSaladComponent(x, intent.GetIntExtra("quantity-" + x, 0)))
                    .ToList();
                mCustomSaladDescription = intent.GetStringExtra("customSaladDescription");
            }

            if (mPickedComponents == null)
            {
                SwitchToPage(WizardPage.SelectSalad);
            }
            else
            {
                if (mDeliveryDay == null || mDeliveryMonth == null || mDeliveryYear == null)
                {
                    SwitchToPage(WizardPage.SelectDeliveryDate);
                }
                else
                {
                    SwitchToPage(WizardPage.SelectDeliveryHour);
                }
            }
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
        private FragmentCalendar mFragmentCalendar;
        private GroupedItemSelectorAdapter mDeliveryScheduleAdapter;
        private ItemSelectorAdapter mDeliveryAddressAdapter;

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
                                        item.Group = flatClassicGroup;
                                    }
                                }

                                savedGroups.Add(flatClassicGroup);

                                SaladGroupAdapter adapter = new SaladGroupAdapter(this, savedGroups.Where(x => x.Items.Count > 0).ToList());

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
                        mFragmentCalendar = new FragmentCalendar();

                        transaction.Replace(Resource.Id.layoutDatePickerContainer, mFragmentCalendar);
                        transaction.Commit();
                    }
                    break;
                case WizardPage.SelectDeliveryHour:
                    {
                        ListView lstSelectDeliveryHour = view.FindViewById<ListView>(Resource.Id.lstSelectDeliveryHour);

                        Task.Factory.StartNew(() =>
                        {
                            mDeliveryScheduleAdapter = new GroupedItemSelectorAdapter(this,
                                DataContext.DeliveryHours.Select(x => new KeyValuePair<string, string>(
                                    x.Catagory.Value == DeliverySchedule.CatagoryEnum.Dinner ? "ناهار" : "شام",
                                    "ساعت " + x.FromHour.ToString().ToPersianNumbers() + " الی " + x.ToHour.ToString().ToPersianNumbers())).ToList(),
                                new DeliveryScheduleGroupComparer());

                            RunOnUiThread(() =>
                            {
                                lstSelectDeliveryHour.Adapter = mDeliveryScheduleAdapter;
                            });
                        });
                    }
                    break;
                case WizardPage.SelectDeliveryAddress:
                    {
                        ListView lstSelectDeliveryAddress = view.FindViewById<ListView>(Resource.Id.lstSelectDeliveryAddress);

                        Task.Factory.StartNew(() =>
                        {
                            mDeliveryAddressAdapter = new ItemSelectorAdapter(this, DataContext.DeliveryAddresses);

                            RunOnUiThread(() =>
                            {
                                lstSelectDeliveryAddress.Adapter = mDeliveryAddressAdapter;
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
                    {
                        if (mSelectedSalad == null)
                        {
                            ShowMessageDialog(Resource.String.ToastSaladNotSelected);
                            break;
                        }

                        if (mDeliveryDay == null || mDeliveryMonth == null || mDeliveryYear == null)
                        {
                            SwitchToPage(WizardPage.SelectDeliveryDate);
                        }
                        else
                        {
                            SwitchToPage(WizardPage.SelectDeliveryHour);
                        }
                    }
                    break;
                case WizardPage.SelectDeliveryDate:
                    {
                        if (!mFragmentCalendar.HasValue)
                        {
                            ShowMessageDialog(Resource.String.ToastDeliveryDateNotSelected);
                            break;
                        }

                        SwitchToPage(WizardPage.SelectDeliveryHour);
                    }
                    break;
                case WizardPage.SelectDeliveryHour:
                    {
                        if (!mDeliveryScheduleAdapter.HasValue)
                        {
                            ShowMessageDialog(Resource.String.ToastDeliveryScheduleNotSelected);
                            break;
                        }

                        SwitchToPage(WizardPage.SelectDeliveryAddress);
                    }
                    break;
                case WizardPage.SelectDeliveryAddress:
                    {
                        if (!mDeliveryAddressAdapter.HasValue)
                        {
                            ShowMessageDialog(Resource.String.ToastDeliveryAddressNotSelected);
                            break;
                        }

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                OrdersApi api = new OrdersApi(SharedConfig.AuthorizedApiConfig);

                                if (mPickedComponents != null)
                                {
                                    using (OpenLoadingFromThread())
                                    {
                                        api.OrderNewCustomSalad(new OrderCustomSalad(mPickedComponents,
                                            new PersianDate(mFragmentCalendar.SelectedYear, mFragmentCalendar.SelectedMonth, mFragmentCalendar.SelectedDay),
                                            DataContext.DeliveryHours[mDeliveryScheduleAdapter.SelectedIndex].Id.Value,
                                            mDeliveryAddressAdapter.SelectedItem,
                                            "یک سالاد خوشمزه!"));
                                    }
                                }
                                else
                                {
                                    using (OpenLoadingFromThread())
                                    {
                                        api.OrderNewSalad(new Order(null, mSelectedSalad.Id,
                                            new PersianDate(mFragmentCalendar.SelectedYear, mFragmentCalendar.SelectedMonth, mFragmentCalendar.SelectedDay),
                                            DataContext.DeliveryHours[mDeliveryScheduleAdapter.SelectedIndex].Id.Value,
                                            mDeliveryAddressAdapter.SelectedItem,
                                            null));
                                    }
                                }

                                DataContext.ClearOrderScheduleCache(mFragmentCalendar.SelectedYear.Value, mFragmentCalendar.SelectedMonth.Value);
                                DataContext.ClearSavedSaladsCache();
                            }
                            catch (Exception e)
                            {
                                ShowMessageDialogForExceptionFromThread(e);
                            }

                            ShowMessageDialog(Resource.String.ToastOrderPlaced, () =>
                            {
                                Finish();
                            });
                        });
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