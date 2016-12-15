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

namespace Saladio.Fragments
{
    public class DialogCalendar : DialogFragment
    {
        private FragmentCalendar mFragmentCalendar;
        private Button mBtnOk;
        private Button mBtnCancel;
        private bool mDismissLock = false;

        public event EventHandler<CalendarDatePickedEventArgs> CalendarDatePicked;
        public event EventHandler<CalendarDateCanceledEventArgs> CalendarDateCanceled;
        public event EventHandler<CalendarDateSelectedEventArgs> CalendarDateSelected;
        public event EventHandler<CalendarDateClearedEventArgs> CalendarDateCleared;

        // Fragment.Context was added in API Level 23 (6.0 Marshmellow)
        public new Context Context
        {
            get
            {
                return Activity;
            }
        }

        public int? mYear;
        public int? mMonth;
        public int? mDay;

        public DialogCalendar()
        {
            mYear = null;
            mMonth = null;
            mDay = null;
        }

        public DialogCalendar(int year, int month, int day)
        {
            mYear = year;
            mMonth = month;
            mDay = day;
        }

        public DialogCalendar(int year, int month)
        {
            mYear = year;
            mMonth = month;
            mDay = null;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.DialogCalendar, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mFragmentCalendar = FragmentManager.FindFragmentByTag<FragmentCalendar>("dialogCalendarCalendarFragment");

            if (mFragmentCalendar == null)
            {
                using (FragmentTransaction transaction = this.ChildFragmentManager.BeginTransaction())
                {
                    mFragmentCalendar = new FragmentCalendar();

                    transaction.Add(Resource.Id.frameCalendarContainer, mFragmentCalendar, "dialogCalendarCalendarFragment");
                    transaction.Commit();
                }
            }

            mBtnOk = view.FindViewById<Button>(Resource.Id.btnOk);
            mBtnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);

            if (mYear != null)
            {
                mFragmentCalendar.SelectedYear = mYear.Value;
            }

            if (mMonth != null)
            {
                mFragmentCalendar.SelectedMonth = mMonth.Value;
            }

            if (mDay != null)
            {
                mFragmentCalendar.SelectedDay = mDay.Value;
                mBtnOk.Visibility = ViewStates.Visible;
            }
            else
            {
                mBtnOk.Visibility = ViewStates.Invisible;
            }

            mFragmentCalendar.CalendarDateSelected += FragmentCalendar_CalendarDateSelected;
            mFragmentCalendar.CalendarDateCleared += FragmentCalendar_CalendarDateCleared;

            mBtnOk.Click += BtnOk_Click;
            mBtnCancel.Click += BtnCancel_Click;
        }

        private bool HasInitializationValue
        {
            get
            {
                return mYear != null && mMonth != null && mDay != null;
            }
        }

        public bool HasValue
        {
            get
            {
                return HasInitializationValue || mFragmentCalendar.HasValue;
            }
        }

        public int? SelectedYear
        {
            get
            {
                if (mFragmentCalendar != null && mFragmentCalendar.HasValue)
                {
                    return mFragmentCalendar.SelectedYear;
                }

                if (HasInitializationValue)
                {
                    return mYear;
                }

                return null;
            }
            set
            {
                if (mFragmentCalendar != null)
                {
                    mFragmentCalendar.SelectedYear = value;
                }
                else
                {
                    mYear = value;
                }
            }
        }

        public int? SelectedMonth
        {
            get
            {
                if (mFragmentCalendar != null && mFragmentCalendar.HasValue)
                {
                    return mFragmentCalendar.SelectedMonth;
                }

                if (HasInitializationValue)
                {
                    return mMonth;
                }

                return null;
            }
            set
            {
                if (mFragmentCalendar != null)
                {
                    mFragmentCalendar.SelectedMonth = value;
                }
                else
                {
                    mMonth = value;
                }
            }
        }

        public int? SelectedDay
        {
            get
            {
                if (mFragmentCalendar != null && mFragmentCalendar.HasValue)
                {
                    return mFragmentCalendar.SelectedDay;
                }

                if (HasInitializationValue)
                {
                    return mDay;
                }

                return null;
            }
            set
            {
                if (mFragmentCalendar != null)
                {
                    mFragmentCalendar.SelectedDay = value;
                }
                else
                {
                    mDay = value;
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CalendarDateCanceled?.Invoke(this, new CalendarDateCanceledEventArgs());
            mDismissLock = true;
            Dismiss();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (mFragmentCalendar.HasValue)
            {
                CalendarDatePicked?.Invoke(this, new CalendarDatePickedEventArgs(mFragmentCalendar.SelectedYear.Value, mFragmentCalendar.SelectedMonth.Value, mFragmentCalendar.SelectedDay.Value));
                mDismissLock = true;
                Dismiss();
            }
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);

            if (!mDismissLock)
            {
                CalendarDateCanceled?.Invoke(this, new CalendarDateCanceledEventArgs());
            }
        }

        private void FragmentCalendar_CalendarDateCleared(object sender, CalendarDateClearedEventArgs e)
        {
            mBtnOk.Visibility = ViewStates.Invisible;
            CalendarDateCleared?.Invoke(this, new CalendarDateClearedEventArgs());
        }

        private void FragmentCalendar_CalendarDateSelected(object sender, CalendarDateSelectedEventArgs e)
        {
            mBtnOk.Visibility = ViewStates.Visible;
            CalendarDateSelected?.Invoke(this, new CalendarDateSelectedEventArgs(e.Year, e.Month, e.Day));
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}