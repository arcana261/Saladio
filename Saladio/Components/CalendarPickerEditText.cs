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
using Android.Util;
using Saladio.Fragments;
using Android.Views.InputMethods;
using System.Globalization;
using Android.Graphics;

namespace Saladio.Components
{
    public class CalendarPickerEditText : EditText
    {
        private int? mYear;
        private int? mMonth;
        private int? mDay;
        private bool mIsDialogOpen = false;
        private bool mIsFocusFromTouch = false;

        public CalendarPickerEditText(Context context) : base(context)
        {
            Setup();
        }
        public CalendarPickerEditText(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Setup();
        }
        public CalendarPickerEditText(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Setup();
        }
        public CalendarPickerEditText(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Setup();
        }

        private void Setup()
        {
            FocusChange += OnFocusChange;
        }

        public Activity Activity
        {
            get;
            set;
        }

        private void OpenDialog()
        {
            if (mIsDialogOpen)
            {
                return;
            }

            if (Activity == null)
            {
                throw new InvalidOperationException("Please set 'Activity' Property on CalendarPickerEditText to point to current activity");
            }

            using (FragmentTransaction transaction = Activity.FragmentManager.BeginTransaction())
            {
                DialogCalendar dialogCalendar = new DialogCalendar();
                dialogCalendar.SelectedYear = mYear;
                dialogCalendar.SelectedMonth = mMonth;
                dialogCalendar.SelectedDay = mDay;

                dialogCalendar.CalendarDatePicked += DialogCalendar_CalendarDatePicked;
                dialogCalendar.CalendarDateCanceled += DialogCalendar_CalendarDateCanceled;

                dialogCalendar.Show(transaction, "dialogCalendar");
                mIsDialogOpen = true;
            }
        }

        private void DialogCalendar_CalendarDateCanceled(object sender, CalendarDateCanceledEventArgs e)
        {
            mIsDialogOpen = false;
        }

        private void OnFocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                OpenDialog();   
            }
        }

        private void FormatText()
        {
            if (mYear != null && mMonth != null && mDay != null)
            {
                PersianCalendar persianCalendar = new PersianCalendar();
                DateTime dateTime = persianCalendar.ToDateTime((int)mYear, (int)mMonth, (int)mDay, 1, 1, 1, 1);
                string dayOfWeek;

                switch (persianCalendar.GetDayOfWeek(dateTime))
                {
                    case DayOfWeek.Sunday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianSunday);
                        break;
                    case DayOfWeek.Monday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianMonday);
                        break;
                    case DayOfWeek.Tuesday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianTuesday);
                        break;
                    case DayOfWeek.Wednesday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianWednesday);
                        break;
                    case DayOfWeek.Thursday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianThursday);
                        break;
                    case DayOfWeek.Friday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianFriday);
                        break;
                    case DayOfWeek.Saturday:
                        dayOfWeek = Resources.GetString(Resource.String.PersianSaturday);
                        break;
                    default:
                        dayOfWeek = "-";
                        break;
                }

                Text = dayOfWeek + ", " + (mDay.Value.ToString().ToPersianNumbers()) + " " +
                    Resources.GetStringArray(Resource.Array.PersianMonths)[mMonth.Value - 1] +
                    " " + (mYear.Value.ToString().ToPersianNumbers());
            }
            else
            {
                Text = "";
            }
        }

        private void DialogCalendar_CalendarDatePicked(object sender, CalendarDatePickedEventArgs e)
        {
            mYear = e.Year;
            mMonth = e.Month;
            mDay = e.Day;

            FormatText();
            mIsDialogOpen = false;

            View nextFocus = FocusSearch(FocusSearchDirection.Right);
            if (nextFocus == null)
            {
                nextFocus = FocusSearch(FocusSearchDirection.Down);
            }

            if (nextFocus != null)
            {
                nextFocus.FocusChange += NextFocus_FocusChange;
                nextFocus.FocusableInTouchMode = true;
                nextFocus.RequestFocus();
                nextFocus.RequestFocusFromTouch();
            }
        }

        private void NextFocus_FocusChange(object sender, FocusChangeEventArgs e)
        {
            View nextFocus = (View)sender;
            nextFocus.FocusChange -= NextFocus_FocusChange;

            nextFocus.PostDelayed(() =>
            {
                InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);

                bool res = imm.ShowSoftInput(nextFocus, ShowFlags.Implicit);

                //if (!res)
                //{
                //    res = imm.ShowSoftInput(nextFocus, ShowFlags.Forced);
                //}

                //if (nextFocus is EditText)
                //{
                //    if (!res)
                //    {
                //        imm.ToggleSoftInputFromWindow(Activity.Window.DecorView.ApplicationWindowToken, ShowSoftInputFlags.Forced, HideSoftInputFlags.None);
                //    }
                //}
            }, 100);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (HasFocus)
            {
                OpenDialog();
            }
            else
            {
                RequestFocusFromTouch();
            }
            return true;
        }
    }
}