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
using Saladio.Models;
using System.Globalization;
using Android.Graphics;

namespace Saladio.Adapters
{
    public class OrderScheduleCalendarAdapter : BaseAdapter<OrderSchedule>
    {
        private Context mContext;
        private IList<OrderSchedule> mOrderSchedules;

        public OrderScheduleCalendarAdapter(Context context, IList<OrderSchedule> orderSchedules)
        {
            mContext = context;
            mOrderSchedules = orderSchedules;
        }

        public override OrderSchedule this[int position]
        {
            get
            {
                return mOrderSchedules[position];
            }
        }

        public override int Count
        {
            get
            {
                return mOrderSchedules.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowOrderScheduleCalendar, parent, false);
            }

            PersianCalendar persianCalendar = new PersianCalendar();
            OrderSchedule item = this[position];
            LinearLayout layoutDinnerIcons = row.FindViewById<LinearLayout>(Resource.Id.layoutDinnerIcons);
            LinearLayout layoutLaunchIcons = row.FindViewById<LinearLayout>(Resource.Id.layoutLaunchIcons);
            TextView txtDayOfWeek = row.FindViewById<TextView>(Resource.Id.txtDayOfWeek);
            TextView txtDayOfMonth = row.FindViewById<TextView>(Resource.Id.txtDayOfMonth);
            TextView txtMonth = row.FindViewById<TextView>(Resource.Id.txtMonth);

            DateTime itemDate = persianCalendar.ToDateTime(item.Year, item.Month, item.Day, 1, 1, 1, 1);
            DayOfWeek dayOfWeek = persianCalendar.GetDayOfWeek(itemDate);
            string weekDay = mContext.Resources.GetStringArray(Resource.Array.PersianWeekDays)[(int)dayOfWeek];

            txtDayOfWeek.Text = weekDay;
            txtDayOfMonth.Text = item.Day.ToString();
            txtMonth.Text = mContext.Resources.GetStringArray(Resource.Array.PersianMonths)[item.Month - 1];

            layoutLaunchIcons.RemoveAllViews();
            layoutDinnerIcons.RemoveAllViews();

            for (int i = 0; i < item.LaunchCount; i++)
            {
                layoutLaunchIcons.AddView(NewImage());
            }

            for (int i = 0; i < item.DinnerCount; i++)
            {
                layoutDinnerIcons.AddView(NewImage());
            }

            return row;
        }

        private ImageView NewImage()
        {
            ImageView ret = new ImageView(mContext);

            ret.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            ret.SetImageBitmap(BitmapFactory.DecodeResource(mContext.Resources, Resource.Drawable.ic_pie_salad_64));
            ret.SetAdjustViewBounds(true);
            ret.SetScaleType(ImageView.ScaleType.CenterCrop);

            return ret;
        }
    }
}