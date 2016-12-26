using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using IO.Swagger.Model;
using Saladio.Adapters;
using Saladio.Components;
using Saladio.Contexts;
using Saladio.Fragments;
using Saladio.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class ActivityMain : SharedActivity
    {
        private enum MainTabs
        {
            CustomSalad = 0,
            ClassicSalads,
            SavedSalads,
            OrderSchedule,
            About,
            Total
        }

        private class TabAdapter : SlidingTabsAdapter
        {
            private SharedActivity mActivity;

            public TabAdapter(SharedActivity activity)
            {
                mActivity = activity;
            }

            public override int Count
            {
                get
                {
                    return (int)MainTabs.Total;
                }
            }

            public override string GetHeaderTitle(int position)
            {
                MainTabs tab = (MainTabs)position;

                switch (tab)
                {
                    case MainTabs.CustomSalad:
                        return mActivity.Resources.GetString(Resource.String.TabCustomSalad);
                    case MainTabs.About:
                        return mActivity.Resources.GetString(Resource.String.TabAbout);
                    case MainTabs.ClassicSalads:
                        return mActivity.Resources.GetString(Resource.String.TabClassicSalads);
                    case MainTabs.SavedSalads:
                        return mActivity.Resources.GetString(Resource.String.TabSavedSalads);
                    case MainTabs.OrderSchedule:
                        return mActivity.Resources.GetString(Resource.String.TabOrderSchedule);
                    default:
                        throw new ArgumentException("unsupported tab: " + tab.ToString());
                }
            }

            protected override View CreateItem(Context context, ViewGroup container, int position)
            {
                MainTabs tab = (MainTabs)position;

                switch (tab)
                {
                    case MainTabs.CustomSalad:
                        { 
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabOrderCustomSalad, container, false);
                            ListView lstCustomSalad = view.FindViewById<ListView>(Resource.Id.lstCustomSalad);
                            TextView txtTotalSaladPrice = view.FindViewById<TextView>(Resource.Id.txtTotalSaladPrice);
                            TextView txtTotalSaladWeight = view.FindViewById<TextView>(Resource.Id.txtTotalSaladWeight);
                            TextView txtTotalSaladCallorie = view.FindViewById<TextView>(Resource.Id.txtTotalSaladCallorie);
                            EditText etCustomSaladDescription = view.FindViewById<EditText>(Resource.Id.etCustomSaladDescription);
                            Button btnOrderCustomSalad = view.FindViewById<Button>(Resource.Id.btnOrderCustomSalad);
                            decimal totalSaladPrice = 0;
                            decimal totalSaladWeight = 0;
                            decimal totalSaladCallorie = 0;
                            SaladComponentGroupAdapter saladComponentAdapter = null;
                            IList<SaladComponentGroup> saladComponentGroups = null;

                            Task.Factory.StartNew(() =>
                            {
                                saladComponentGroups = mActivity.DataContext.SaladComponentGroups;
                                saladComponentAdapter = new SaladComponentGroupAdapter(mActivity, saladComponentGroups);

                                mActivity.RunOnUiThread(() =>
                                {
                                    lstCustomSalad.Adapter = saladComponentAdapter;
                                    lstCustomSalad.SetListViewHeightBasedOnChildren();
                                });

                                saladComponentAdapter.SaladComponetButtonPlusClicked += (sender, args) =>
                                {
                                    totalSaladPrice += (args.Quantity - args.OldQuantity) * args.SaladComponent.Price.Value;
                                    totalSaladWeight += (args.Quantity - args.OldQuantity) * args.SaladComponent.Weight.Value;
                                    totalSaladCallorie += (args.Quantity - args.OldQuantity) * args.SaladComponent.Callorie.Value;

                                    txtTotalSaladPrice.Text = ((int)totalSaladPrice).ToString().ToPersianNumbers();
                                    txtTotalSaladWeight.Text = ((int)totalSaladWeight).ToString().ToPersianNumbers();
                                    txtTotalSaladCallorie.Text = ((int)totalSaladCallorie).ToString().ToPersianNumbers();
                                };

                                saladComponentAdapter.SaladComponetButtonMinusClicked += (sender, args) =>
                                {
                                    totalSaladPrice += (args.Quantity - args.OldQuantity) * args.SaladComponent.Price.Value;
                                    totalSaladWeight += (args.Quantity - args.OldQuantity) * args.SaladComponent.Weight.Value;
                                    totalSaladCallorie += (args.Quantity - args.OldQuantity) * args.SaladComponent.Callorie.Value;

                                    txtTotalSaladPrice.Text = ((int)totalSaladPrice).ToString().ToPersianNumbers();
                                    txtTotalSaladWeight.Text = ((int)totalSaladWeight).ToString().ToPersianNumbers();
                                    txtTotalSaladCallorie.Text = ((int)totalSaladCallorie).ToString().ToPersianNumbers();
                                };
                            });

                            btnOrderCustomSalad.Click += (sender, args) =>
                            {
                                if (totalSaladPrice < 1 || totalSaladWeight < 1 || totalSaladCallorie < 1)
                                {
                                    mActivity.ShowMessageDialog(Resource.String.ToastSaladComponentNotSelected);
                                }
                                else
                                {
                                    Intent intent = new Intent(mActivity, typeof(ActivityOrderScheduled));

                                    List<int> saladComponentIds = new List<int>();

                                    foreach (var saladComponentGroup in saladComponentGroups)
                                    {
                                        foreach (var saladComponent in saladComponentGroup.Items)
                                        {
                                            int quantity = saladComponentAdapter.GetQuantity(saladComponent);

                                            if (quantity > 0)
                                            {
                                                saladComponentIds.Add(saladComponent.Id.Value);
                                                intent.PutExtra("quantity-" + saladComponent.Id.Value, quantity);
                                            }
                                        }
                                    }

                                    StringBuilder builder = new StringBuilder();
                                    for (int i = 0; i < saladComponentIds.Count; i++)
                                    {
                                        if (i > 0)
                                        {
                                            builder.Append(",");
                                        }

                                        builder.Append(saladComponentIds[i]);
                                    }
                                    intent.PutExtra("saladComponentIds", builder.ToString());
                                    intent.PutExtra("customSaladDescription", etCustomSaladDescription.Text);

                                    mActivity.StartActivity(intent);
                                }
                            };

                            return view;
                        }
                    case MainTabs.About:
                        return LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabAbout, container, false);
                    case MainTabs.ClassicSalads:
                        using (SaladioContext saladioContext = new SaladioContext())
                        {
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabClassicSalads, container, false);
                            ListView lstClassicSalads = view.FindViewById<ListView>(Resource.Id.lstClassicSalads);

                            Task.Factory.StartNew(() =>
                            {
                                using (mActivity.OpenLoadingFromThread())
                                {
                                    IList<SaladListItemGroup> groups = SaladListItemGroup.GetGroups(mActivity.DataContext, mActivity.DataContext.ClassicSaladCatagories);
                                    SaladGroupAdapter adapter = new SaladGroupAdapter(mActivity, groups);

                                    mActivity.RunOnUiThread(() =>
                                    {
                                        lstClassicSalads.Adapter = adapter;

                                        adapter.SavedSaladSelected += (sender, args) =>
                                        {
                                            using (FragmentTransaction transaction = mActivity.FragmentManager.BeginTransaction())
                                            {
                                                DialogSaladInformation dialog = new DialogSaladInformation(args.SavedSalad);
                                                dialog.IsEditable = false;
                                                dialog.Show(transaction, "classicSaladInformation");
                                            }
                                        };
                                    });
                                }
                            });

                            return view;
                        }
                    case MainTabs.SavedSalads:
                        using (SaladioContext saladioContext = new SaladioContext())
                        {
                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabSavedSalads, container, false);
                            ListView lstSavedSalads = view.FindViewById<ListView>(Resource.Id.lstSavedSalads);

                            Task.Factory.StartNew(() =>
                            {
                                using (mActivity.OpenLoadingFromThread())
                                {
                                    IList<SaladListItemGroup> savedGroups = SaladListItemGroup.GetGroups(mActivity.DataContext, mActivity.DataContext.SavedSalads);
                                    IList<SaladListItemGroup> classicGroups = SaladListItemGroup.GetGroups(mActivity.DataContext, mActivity.DataContext.ClassicSaladCatagories);

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

                                    SaladGroupAdapter adapter = new SaladGroupAdapter(mActivity, savedGroups.Where(x => x.Items.Count > 0).ToList());

                                    mActivity.RunOnUiThread(() =>
                                    {
                                        lstSavedSalads.Adapter = adapter;

                                        adapter.SavedSaladSelected += (sender, args) =>
                                        {
                                            using (FragmentTransaction transaction = mActivity.FragmentManager.BeginTransaction())
                                            {
                                                DialogSaladInformation dialog = new DialogSaladInformation(args.SavedSalad);
                                                dialog.IsEditable = false;
                                                dialog.Show(transaction, "classicSaladInformation");
                                            }
                                        };
                                    });
                                }
                            });

                            return view;
                        }
                    case MainTabs.OrderSchedule:
                        {
                            PersianCalendar persianCalendar = new PersianCalendar();
                            DateTime now = DateTime.Now;
                            int currentMonth = persianCalendar.GetMonth(now);
                            int currentYear = persianCalendar.GetYear(now);

                            View view = LayoutInflater.From(mActivity).Inflate(Resource.Layout.TabOrderSchedule, container, false);
                            ListView lstOrderSchedule = view.FindViewById<ListView>(Resource.Id.lstOrderSchedule);
                            Button btnCalendarNextMonth = view.FindViewById<Button>(Resource.Id.btnCalendarNextMonth);
                            Button btnCalendarPrevMonth = view.FindViewById<Button>(Resource.Id.btnCalendarPrevMonth);
                            TextView txtCalendarCurrentMonth = view.FindViewById<TextView>(Resource.Id.txtCalendarCurrentMonth);

                            string[] monthNames = mActivity.Resources.GetStringArray(Resource.Array.PersianMonths);

                            Action<int, int> updateList = new Action<int, int>((year, month) =>
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    IList<GroupedOrderSchedule> orders = mActivity.DataContext.GetGroupedOrderSchedules(year, month);

                                    OrderScheduleCalendarAdapter adapter = new OrderScheduleCalendarAdapter(mActivity, orders);

                                    mActivity.RunOnUiThread(() =>
                                    {
                                        lstOrderSchedule.Adapter = adapter;
                                        txtCalendarCurrentMonth.Text = year.ToString().ToPersianNumbers() + " " + monthNames[month - 1];
                                    });

                                    adapter.NewOrder += (sender, args) =>
                                    {
                                        Intent intent = new Intent(mActivity, typeof(ActivityOrderScheduled));
                                        intent.PutExtra("year", args.Year);
                                        intent.PutExtra("month", args.Month);
                                        intent.PutExtra("day", args.Day);

                                        mActivity.StartActivity(intent);
                                    };
                                });
                            });

                            btnCalendarNextMonth.Click += (sender, args) =>
                            {
                                currentMonth = currentMonth + 1;
                                if (currentMonth > 12)
                                {
                                    currentMonth = 1;
                                    currentYear = currentYear + 1;
                                }

                                updateList(currentYear, currentMonth);
                            };

                            btnCalendarPrevMonth.Click += (sender, args) =>
                            {
                                currentMonth = currentMonth - 1;
                                if (currentMonth < 1)
                                {
                                    currentMonth = 12;
                                    currentYear = currentYear - 1;
                                }

                                updateList(currentYear, currentMonth);
                            };

                            updateList(currentYear, currentMonth);

                            return view;
                        }
                    default:
                        throw new ArgumentException("unsupported tab: " + tab.ToString());
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityMain);

            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom);
            ActionBar.SetCustomView(Resource.Layout.ActionBarMain);

            Button actionBarOrderScheduled = ActionBar.CustomView.FindViewById<Button>(Resource.Id.btnOrderScheduledSalad);
            actionBarOrderScheduled.Click += ActionBarOrderScheduled_Click;

            // Create your application here
            SlidingTabsFragment slidingTabs = new SlidingTabsFragment();
            slidingTabs.Adapter = new TabAdapter(this);

            using (var transaction = FragmentManager.BeginTransaction())
            {
                transaction.Replace(Resource.Id.contentFragment, slidingTabs);
                transaction.Commit();
            }
        }

        private void ActionBarOrderScheduled_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityOrderScheduled));
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
    }
}