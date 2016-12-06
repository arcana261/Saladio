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
using System.Globalization;

namespace Saladio.Fragments
{
    public class FragmentDatePicker : Fragment
    {
        private static readonly int DefaultYearsBefore = 100;
        private static readonly int DefaultYearsAfter = 0;

        private Spinner mSpinYear;
        private Spinner mSpinMonth;
        private Spinner mSpinDay;

        private int mYearsBefore;
        private int mYearsAfter;

        private IList<int> mSpinYearList;
        private IList<int> mSpinDayList;

        public FragmentDatePicker()
        {
            mYearsBefore = DefaultYearsBefore;
            mYearsAfter = DefaultYearsAfter;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            DateTime now = DateTime.Now;

            mSpinYear = view.FindViewById<Spinner>(Resource.Id.spinYear);
            mSpinMonth = view.FindViewById<Spinner>(Resource.Id.spinMonth);
            mSpinDay = view.FindViewById<Spinner>(Resource.Id.spinDay);

            mSpinMonth.ItemSelected += SpinMonth_ItemSelected;

            PersianCalendar calendar = new PersianCalendar();
            int year = calendar.GetYear(now);
            int month = calendar.GetMonth(now);
            int day = calendar.GetDayOfMonth(now);

            RefreshYearSpinner(year);
            JalaliMonth = month;
            RefreshDaySpinner(month, day);
        }

        private void SpinMonth_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            RefreshDaySpinner(e.Position + 1, JalaliDay);
        }

        private void RefreshDaySpinner(int month, int day)
        {
            mSpinDayList = new List<int>();
            int daysOfMonth = month <= 6 ? 31 : 30;

            for (int d = 0; d < daysOfMonth; d++)
            {
                mSpinDayList.Add(d + 1);
            }

            if (day > daysOfMonth)
            {
                day = 1;
            }

            mSpinDay.Adapter = new ArrayAdapter<int>(Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, mSpinDayList);
            JalaliDay = day;
        }

        private int YearsBefore
        {
            get
            {
                return mYearsBefore;
            }
            set
            {
                mYearsBefore = value;

                if (mSpinYear != null)
                {
                    RefreshYearSpinner(JalaliYear);
                }
            }
        }

        private int YearsAfter
        {
            get
            {
                return mYearsAfter;
            }
            set
            {
                mYearsAfter = value;

                if (mSpinYear != null)
                {
                    RefreshYearSpinner(JalaliYear);
                }
            }
        }

        public int JalaliYear
        {
            get
            {
                return mSpinYearList[mSpinYear.SelectedItemPosition];
            }
            set
            {
                RefreshYearSpinner(value);
            }
        }

        public int JalaliMonth
        {
            get
            {
                return mSpinMonth.SelectedItemPosition + 1;
            }
            set
            {
                mSpinMonth.SetSelection(value - 1);
            }
        }

        public int JalaliDay
        {
            get
            {
                return mSpinDay.SelectedItemPosition + 1;
            }
            set
            {
                mSpinDay.SetSelection(value - 1);
            }
        }

        private void RefreshYearSpinner(int year)
        {
            mSpinYearList = new List<int>();
            for (int y = year - YearsBefore; y <= year + YearsAfter; y++)
            {
                mSpinYearList.Add(y);
            }

            mSpinYear.Adapter = new ArrayAdapter<int>(Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, mSpinYearList);
            mSpinYear.SetSelection(YearsBefore);
        }

        private void RefreshYearSpinner(DateTime now)
        {
            PersianCalendar calendar = new PersianCalendar();
            int year = calendar.GetYear(now);

            RefreshYearSpinner(year);
        }

        private void RefreshYearSpinner()
        {
            RefreshYearSpinner(DateTime.Now);
        }

        public void SetDate(DateTime now)
        {
            PersianCalendar calendar = new PersianCalendar();

            int year = calendar.GetYear(now);
            int month = calendar.GetMonth(now);
            int day = calendar.GetDayOfMonth(now);

            JalaliYear = year;
            JalaliMonth = month;
            JalaliDay = day;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FragmentDatePicker, container, false);
        }
    }
}