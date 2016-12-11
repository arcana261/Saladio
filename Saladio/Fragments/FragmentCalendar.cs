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
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using System;

namespace Saladio.Fragments
{
    public class FragmentCalendar : Fragment
    {
        private TextView[][] mItems; //[row][column]
        private TextView mTxtCalendarCurrentMonth;
        private int mCurrentYear;
        private int mCurrentMonth;
        private int mCurrentDay = -1;
        private TextView mCurrentSelectedCell;
        private Button mBtnCalendarNextMonth;
        private Button mBtnCalendarPrevMonth;
        private Dictionary<int, TextView> mDayToCell;

        private int mTodayYear;
        private int mTodayMonth;
        private int mTodayDay;

        public event EventHandler<CalendarDateSelectedEventArgs> CalendarDateSelected;
        public event EventHandler<CalendarDateClearedEventArgs> CalendarDateCleared;

        public FragmentCalendar()
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            DateTime now = DateTime.Now;

            mTodayYear = persianCalendar.GetYear(now);
            mTodayMonth = persianCalendar.GetMonth(now);
            mTodayDay = persianCalendar.GetDayOfMonth(now);
        }

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

        private Color ToRgb(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        private void SetTag(TextView item, int day, bool status)
        {
            if (!status)
            {
                item.Tag = "disabled";
            }
            else
            {
                item.Tag = day.ToString();
            }
        }

        private void SetTagEnabled(TextView item, int day)
        {
            SetTag(item, day, true);
        }

        private void SetTagDisabled(TextView item)
        {
            SetTag(item, -1, false);
        }

        private bool IsTagEnabled(TextView item)
        {
            return !((string)item.Tag).Equals("disabled");
        }

        private int GetTaggedDay(TextView item)
        {
            string value = (string)item.Tag;

            if (value.Equals("disabled"))
            {
                return -1;
            }

            return int.Parse(value);
        }

        private void PaintMonth(int year, int month)
        {
            Color colorEnabled = ToRgb(ContextCompat.GetColor(Context, Resource.Color.CalendarItemEnabled));
            Color colorDisabled = ToRgb(ContextCompat.GetColor(Context, Resource.Color.CalendarItemDisabled));
            bool isTodayMonth = year == mTodayYear && month == mTodayMonth;

            PersianCalendar persianCalendar = new PersianCalendar();
            DateTime date = persianCalendar.ToDateTime(year, month, 1, 1, 1, 1, 1);

            int daysOfMonth = persianCalendar.GetDaysInMonth(year, month);
            int prevDaysOfMonth;

            if (month == 1)
            {
                prevDaysOfMonth = persianCalendar.GetDaysInMonth(year - 1, 12);
            }
            else
            {
                prevDaysOfMonth = persianCalendar.GetDaysInMonth(year, month - 1);
            }

            int row = 0;
            int column = GetWeekDayColumn(persianCalendar.GetDayOfWeek(date));

            for (int i = DecrementColumn(column); i >= 0; i = DecrementColumn(i))
            {
                TextView item = GetItem(row, i);
                item.Text = prevDaysOfMonth.ToString().ToPersianNumbers();
                item.SetTextColor(colorDisabled);
                SetTagDisabled(item);

                prevDaysOfMonth = prevDaysOfMonth - 1;
            }

            mDayToCell = new Dictionary<int, TextView>();
            for (int i = 1; i <= daysOfMonth; i++)
            {
                TextView item = GetItem(row, column);
                item.Text = i.ToString().ToPersianNumbers();
                item.SetTextColor(colorEnabled);
                SetTagEnabled(item, i);
                mDayToCell.Add(i, item);

                if (isTodayMonth && i == mTodayDay)
                {
                    item.Background = ContextCompat.GetDrawable(Context, Resource.Drawable.CalendarCellTodayStyle);
                }
                else
                {
                    item.Background = null;
                }

                column = IncrementColumn(column);
                if (column < 0)
                {
                    column = ResetColumn();
                    row = row + 1;
                }
            }

            if (row == mItems.Length - 1 && column != ResetColumn())
            {
                ChangeLastRowViewState(ViewStates.Visible);
            }
            else
            {
                ChangeLastRowViewState(ViewStates.Gone);
            }

            int nextMonth = 1;
            while (column >= 0)
            {
                TextView item = GetItem(row, column);
                item.Text = nextMonth.ToString().ToPersianNumbers();
                item.SetTextColor(colorDisabled);
                SetTagDisabled(item);

                column = IncrementColumn(column);
                nextMonth = nextMonth + 1;
            }

            mTxtCalendarCurrentMonth.Text = Resources.GetStringArray(Resource.Array.PersianMonths)[month - 1] + " " + (year.ToString().ToPersianNumbers());

            mCurrentYear = year;
            mCurrentMonth = month;

            if (mCurrentDay >= 0)
            {
                if (mCurrentDay > daysOfMonth)
                {
                    if (mCurrentSelectedCell != null)
                    {
                        DeselectCell(mCurrentSelectedCell);
                        mCurrentSelectedCell = null;
                        CalendarDateCleared?.Invoke(this, new CalendarDateClearedEventArgs());
                    }
                    mCurrentDay = -1;
                }
                else
                {
                    TextView item = mDayToCell[mCurrentDay];
                    OnCellSelected(item, mCurrentDay);
                }
            }
            else if (mCurrentSelectedCell != null)
            {
                DeselectCell(mCurrentSelectedCell);
                mCurrentSelectedCell = null;
            }
        }

        private void ChangeLastRowViewState(ViewStates viewState)
        {
            int index = mItems.Length - 1;
            for (int i = 0; i < mItems[index].Length; i++)
            {
                mItems[index][i].Visibility = viewState;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mTxtCalendarCurrentMonth = view.FindViewById<TextView>(Resource.Id.txtCalendarCurrentMonth);
            mBtnCalendarNextMonth = view.FindViewById<Button>(Resource.Id.btnCalendarNextMonth);
            mBtnCalendarPrevMonth = view.FindViewById<Button>(Resource.Id.btnCalendarPrevMonth);

            mItems = new TextView[6][];
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

            mItems[5][0] = view.FindViewById<TextView>(Resource.Id.txtRow6Column1);
            mItems[5][1] = view.FindViewById<TextView>(Resource.Id.txtRow6Column2);
            mItems[5][2] = view.FindViewById<TextView>(Resource.Id.txtRow6Column3);
            mItems[5][3] = view.FindViewById<TextView>(Resource.Id.txtRow6Column4);
            mItems[5][4] = view.FindViewById<TextView>(Resource.Id.txtRow6Column5);
            mItems[5][5] = view.FindViewById<TextView>(Resource.Id.txtRow6Column6);
            mItems[5][6] = view.FindViewById<TextView>(Resource.Id.txtRow6Column7);

            foreach (var row in mItems)
            {
                foreach (var cell in row)
                {
                    cell.Click += Cell_Click;
                }
            }

            mBtnCalendarNextMonth.Click += BtnCalendarNextMonth_Click;
            mBtnCalendarPrevMonth.Click += BtnCalendarPrevMonth_Click;

            PersianCalendar persianCalendar = new PersianCalendar();
            DateTime now = DateTime.Now;

            int year = persianCalendar.GetYear(now);
            int month = persianCalendar.GetMonth(now);

            PaintMonth(year, month);
        }

        private void DeselectCell(TextView item)
        {
            if (mCurrentYear == mTodayYear && mCurrentMonth == mTodayMonth && GetTaggedDay(item) == mTodayDay)
            {
                item.Background = ContextCompat.GetDrawable(Context, Resource.Drawable.CalendarCellTodayStyle);
            }
            else
            {
                item.Background = null;
            }

            item.SetTextColor(ToRgb(ContextCompat.GetColor(Context, Resource.Color.CalendarItemDeselectedForeground)));
        }

        private void OnCellSelected(TextView item, int day)
        {
            if (mCurrentSelectedCell != null)
            {
                DeselectCell(mCurrentSelectedCell);
            }

            item.Background = ContextCompat.GetDrawable(Context, Resource.Drawable.CalendarCellSelectedStyle);
            item.SetTextColor(ToRgb(ContextCompat.GetColor(Context, Resource.Color.CalendarItemSelectedForeground)));

            mCurrentSelectedCell = item;
            mCurrentDay = day;
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            TextView item = (TextView)sender;
            int day = GetTaggedDay(item);

            if (day >= 0)
            {
                OnCellSelected(item, day);

                CalendarDateSelected?.Invoke(this, new CalendarDateSelectedEventArgs(mCurrentYear, mCurrentMonth, mCurrentDay));
            }
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

        public bool HasValue
        {
            get
            {
                return mCurrentDay >= 0;
            }
        }

        public int? SelectedYear
        {
            get
            {
                if (mCurrentDay >= 0)
                {
                    return mCurrentYear;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    if (Context != null)
                    {
                        PaintMonth((int)value, mCurrentMonth);
                    }
                    else
                    {
                        mCurrentYear = (int)value;
                    }
                }
            }
        }

        public int? SelectedMonth
        {
            get
            {
                if (mCurrentDay >= 0)
                {
                    return mCurrentMonth;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    if (Context != null)
                    {
                        PaintMonth(mCurrentYear, (int)value);
                    }
                    else
                    {
                        mCurrentMonth = (int)value;
                    }
                }
            }
        }

        public int? SelectedDay
        {
            get
            {
                if (mCurrentDay >= 0)
                {
                    return mCurrentDay;
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    if (mCurrentSelectedCell != null)
                    {
                        DeselectCell(mCurrentSelectedCell);
                        mCurrentSelectedCell = null;
                    }

                    mCurrentDay = -1;
                }
                else
                {
                    mCurrentDay = (int)value;

                    if (Context != null)
                    {
                        PaintMonth(mCurrentYear, mCurrentMonth);
                    }
                }
            }
        }
    }
}