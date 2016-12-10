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
    public class FragmentCalendar : Fragment
    {
        private TextView[][] mItems; //[row][column]
        private TextView mTxtCalendarCurrentMonth;
        private int mCurrentYear;
        private int mCurrentMonth;
        private Button mBtnCalendarNextMonth;
        private Button mBtnCalendarPrevMonth;

        private int GetWeekDayColumn(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 5;
                case DayOfWeek.Monday:
                    return 4;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 2;
                case DayOfWeek.Thursday:
                    return 1;
                case DayOfWeek.Friday:
                    return 0;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    throw new ArgumentException("unexpected value: " + dayOfWeek.ToString(), "dayOfWeek");
            }
        }

        private int IncrementColumn(int column)
        {
            return column - 1;
        }

        private int ResetColumn()
        {
            return 6;
        }

        private int DecrementColumn(int column)
        {
            column = column + 1;

            if (column > 6)
            {
                return -1;
            }

            return column;
        }

        private TextView GetItem(int row, int column)
        {
            return mItems[row][column];
        }

        private void PaintMonth(int year, int month)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            DateTime date = persianCalendar.ToDateTime(year, month, 1, 1, 1, 1, 1);

            int daysOfMonth = persianCalendar.GetDaysInMonth(year, month);

            int row = 0;
            int column = GetWeekDayColumn(persianCalendar.GetDayOfWeek(date));

            for (int i = ResetColumn(); i != column; i = IncrementColumn(i))
            {
                GetItem(row, i).Text = "-";
            }

            for (int i = 1; i <= daysOfMonth; i++)
            {
                GetItem(row, column).Text = i.ToString();

                column = IncrementColumn(column);
                if (column < 0)
                {
                    column = ResetColumn();
                    row = row + 1;
                }
            }

            while (column >= 0)
            {
                GetItem(row, column).Text = "-";
                column = IncrementColumn(column);
            }

            mTxtCalendarCurrentMonth.Text = Resources.GetStringArray(Resource.Array.PersianMonths)[month - 1] + " " + year;

            mCurrentYear = year;
            mCurrentMonth = month;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mTxtCalendarCurrentMonth = view.FindViewById<TextView>(Resource.Id.txtCalendarCurrentMonth);
            mBtnCalendarNextMonth = view.FindViewById<Button>(Resource.Id.btnCalendarNextMonth);
            mBtnCalendarPrevMonth = view.FindViewById<Button>(Resource.Id.btnCalendarPrevMonth);

            mItems = new TextView[5][];
            for (int i = 0; i < mItems.Length; i++)
            {
                mItems[i] = new TextView[7];
            }

            mItems[0][0] = view.FindViewById<TextView>(Resource.Id.txtRow1Column1);
            mItems[0][1] = view.FindViewById<TextView>(Resource.Id.txtRow1Column2);
            mItems[0][2] = view.FindViewById<TextView>(Resource.Id.txtRow1Column3);
            mItems[0][3] = view.FindViewById<TextView>(Resource.Id.txtRow1Column4);
            mItems[0][4] = view.FindViewById<TextView>(Resource.Id.txtRow1Column5);
            mItems[0][5] = view.FindViewById<TextView>(Resource.Id.txtRow1Column6);
            mItems[0][6] = view.FindViewById<TextView>(Resource.Id.txtRow1Column7);

            mItems[1][0] = view.FindViewById<TextView>(Resource.Id.txtRow2Column1);
            mItems[1][1] = view.FindViewById<TextView>(Resource.Id.txtRow2Column2);
            mItems[1][2] = view.FindViewById<TextView>(Resource.Id.txtRow2Column3);
            mItems[1][3] = view.FindViewById<TextView>(Resource.Id.txtRow2Column4);
            mItems[1][4] = view.FindViewById<TextView>(Resource.Id.txtRow2Column5);
            mItems[1][5] = view.FindViewById<TextView>(Resource.Id.txtRow2Column6);
            mItems[1][6] = view.FindViewById<TextView>(Resource.Id.txtRow2Column7);

            mItems[2][0] = view.FindViewById<TextView>(Resource.Id.txtRow3Column1);
            mItems[2][1] = view.FindViewById<TextView>(Resource.Id.txtRow3Column2);
            mItems[2][2] = view.FindViewById<TextView>(Resource.Id.txtRow3Column3);
            mItems[2][3] = view.FindViewById<TextView>(Resource.Id.txtRow3Column4);
            mItems[2][4] = view.FindViewById<TextView>(Resource.Id.txtRow3Column5);
            mItems[2][5] = view.FindViewById<TextView>(Resource.Id.txtRow3Column6);
            mItems[2][6] = view.FindViewById<TextView>(Resource.Id.txtRow3Column7);

            mItems[3][0] = view.FindViewById<TextView>(Resource.Id.txtRow4Column1);
            mItems[3][1] = view.FindViewById<TextView>(Resource.Id.txtRow4Column2);
            mItems[3][2] = view.FindViewById<TextView>(Resource.Id.txtRow4Column3);
            mItems[3][3] = view.FindViewById<TextView>(Resource.Id.txtRow4Column4);
            mItems[3][4] = view.FindViewById<TextView>(Resource.Id.txtRow4Column5);
            mItems[3][5] = view.FindViewById<TextView>(Resource.Id.txtRow4Column6);
            mItems[3][6] = view.FindViewById<TextView>(Resource.Id.txtRow4Column7);

            mItems[4][0] = view.FindViewById<TextView>(Resource.Id.txtRow5Column1);
            mItems[4][1] = view.FindViewById<TextView>(Resource.Id.txtRow5Column2);
            mItems[4][2] = view.FindViewById<TextView>(Resource.Id.txtRow5Column3);
            mItems[4][3] = view.FindViewById<TextView>(Resource.Id.txtRow5Column4);
            mItems[4][4] = view.FindViewById<TextView>(Resource.Id.txtRow5Column5);
            mItems[4][5] = view.FindViewById<TextView>(Resource.Id.txtRow5Column6);
            mItems[4][6] = view.FindViewById<TextView>(Resource.Id.txtRow5Column7);

            mBtnCalendarNextMonth.Click += BtnCalendarNextMonth_Click;
            mBtnCalendarPrevMonth.Click += BtnCalendarPrevMonth_Click;

            PersianCalendar persianCalendar = new PersianCalendar();
            DateTime now = DateTime.Now;

            int year = persianCalendar.GetYear(now);
            int month = persianCalendar.GetMonth(now);

            PaintMonth(year, month);
        }

        private void BtnCalendarPrevMonth_Click(object sender, EventArgs e)
        {
            mCurrentMonth = mCurrentMonth - 1;

            if (mCurrentMonth < 1)
            {
                mCurrentMonth = 12;
                mCurrentYear = mCurrentYear - 1;
            }

            PaintMonth(mCurrentYear, mCurrentMonth);
        }

        private void BtnCalendarNextMonth_Click(object sender, EventArgs e)
        {
            mCurrentMonth = mCurrentMonth + 1;

            if (mCurrentMonth > 12)
            {
                mCurrentMonth = 1;
                mCurrentYear = mCurrentYear + 1;
            }

            PaintMonth(mCurrentYear, mCurrentMonth);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FragmentCalendar, container, false);
        }
    }
}