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

namespace Saladio.Contexts
{
    public class SaladioContext : IDisposable
    {
        private IList<SaladComponentGroup> mGroups;
        private IList<SavedSaladGroup> mClassicSalads;
        private IList<SavedSaladGroup> mSavedSalads;
        private IList<DeliveryHourRange> mDeliveryHours;
        private IList<string> mDeliveryAddresses;
        private IList<OrderSchedule> mOrderSchedules;

        public SaladioContext()
        {
        }

        public IList<SavedSaladGroup> ClassicSaladGroups
        {
            get
            {
                if (mClassicSalads == null)
                {
                    mClassicSalads = new List<SavedSaladGroup>();

                    SavedSaladGroup lettuce = new SavedSaladGroup() { Name = "سالادهای پایه کاهو" };
                    lettuce.Salads.Add(new SavedSalad()
                    {
                        Name = "سالاد سزار",
                        Ingredients = "کاهو رسمی، سینه مرغ طعم دار شده، زیتون سیاه، گوجه گیلاسی، سس سزار",
                        Callorie = 760,
                        Price = 17000,
                        Group = lettuce
                    });
                    lettuce.Salads.Add(new SavedSalad()
                    {
                        Name = "سالاد چاینیز",
                        Ingredients = "کاهو، سینه مرغ طعم دار شده، کنجد، گوجه گیلاسی، فلفل دلمه ای، سس کره بادام زمینی",
                        Callorie = 760,
                        Price = 17000,
                        Group = lettuce
                    });

                    SavedSaladGroup bandSalads = new SavedSaladGroup() { Name = "باند سالادها" };
                    bandSalads.Salads.Add(new SavedSalad()
                    {
                        Name = "چیکن باند سالاد",
                        Ingredients = "سینه مرغ، کرفس، بادام زمینی، سس مخصوص",
                        Callorie = 760,
                        Price = 17000,
                        Group = bandSalads
                    });

                    mClassicSalads.Add(lettuce);
                    mClassicSalads.Add(bandSalads);
                }

                return mClassicSalads;
            }
        }

        public IList<SaladComponentGroup> SaladComponentGroups
        {
            get
            {
                if (mGroups == null)
                {
                    mGroups = new List<SaladComponentGroup>();

                    SaladComponentGroup saladBase = new SaladComponentGroup() { Name = "پایه سالاد" };
                    saladBase.Components.Add(new SaladComponent() { Name = "کاهو پیچ" });
                    saladBase.Components.Add(new SaladComponent() { Name = "کاهو رسمی" });
                    saladBase.Components.Add(new SaladComponent() { Name = "اسفناج" });

                    SaladComponentGroup vegtables = new SaladComponentGroup() { Name = "سبزیجات" };
                    vegtables.Components.Add(new SaladComponent() { Name = "کلم سفید" });

                    SaladComponentGroup cheese = new SaladComponentGroup() { Name = "پنیر" };
                    cheese.Components.Add(new SaladComponent() { Name = "پنیر پارمسان" });

                    SaladComponentGroup dried = new SaladComponentGroup() { Name = "خشکبار" };
                    dried.Components.Add(new SaladComponent() { Name = "نان کروتون" });

                    SaladComponentGroup sauce = new SaladComponentGroup() { Name = "سس و ونیگار" };
                    sauce.Components.Add(new SaladComponent() { Name = "سس سزار" });

                    mGroups.Add(saladBase);
                    mGroups.Add(vegtables);
                    mGroups.Add(cheese);
                    mGroups.Add(dried);
                    mGroups.Add(sauce);
                }
                return mGroups;
            }
        }

        public IList<SavedSaladGroup> SavedSaladGroups
        {
            get
            {
                if (mSavedSalads == null)
                {
                    mSavedSalads = new List<SavedSaladGroup>();

                    SavedSaladGroup savedSalads = new SavedSaladGroup()
                    {
                        Name = "سالادهای انتخابی من"
                    };
                    savedSalads.Salads.Add(new SavedSalad()
                    {
                        Name = "سالاد پروتئینه 1",
                        Ingredients = "کاهو رسمی، سینه مرغ طعم دار شده، لوبیای پیمنتو، سویای خشک ، بروکلی، کلم سفید",
                        Callorie = 760,
                        Price = 17000,
                        Group = savedSalads
                    });
                    savedSalads.Salads.Add(new SavedSalad()
                    {
                        Name = "سالاد پروتئینه 2",
                        Ingredients = "کاهو رسمی، سینه مرغ طعم دار شده، لوبیای پیمنتو، سویای خشک ، بروکلی، کلم سفید",
                        Callorie = 760,
                        Price = 17000,
                        Group = savedSalads
                    });

                    SavedSaladGroup classicSalads = new SavedSaladGroup()
                    {
                        Name = "سالادهای کلاسیک"
                    };
                    classicSalads.Salads.Add(new SavedSalad()
                    {
                        Name = "چیکن باند سالاد",
                        Ingredients = "سینه مرغ، کرفس، بادام زمینی، سس مخصوص",
                        Callorie = 760,
                        Price = 17000,
                        Group = classicSalads
                    });

                    mSavedSalads.Add(savedSalads);
                    mSavedSalads.Add(classicSalads);
                }

                return mSavedSalads;
            }
        }

        public IList<DeliveryHourRange> DeliveryHours
        {
            get
            {
                if (mDeliveryHours == null)
                {
                    const string morningDelivery = "صرف ناهار";
                    const string dinnerDelivery = "صرف شام";

                    mDeliveryHours = new List<DeliveryHourRange>();

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 11,
                        To = 12,
                        Catagory = morningDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 12,
                        To = 13,
                        Catagory = morningDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 13,
                        To = 14,
                        Catagory = morningDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 14,
                        To = 15,
                        Catagory = morningDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 18,
                        To = 19,
                        Catagory = dinnerDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 19,
                        To = 20,
                        Catagory = dinnerDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 20,
                        To = 21,
                        Catagory = dinnerDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 21,
                        To = 22,
                        Catagory = dinnerDelivery
                    });

                    mDeliveryHours.Add(new DeliveryHourRange()
                    {
                        From = 22,
                        To = 23,
                        Catagory = dinnerDelivery
                    });
                }
            
                return mDeliveryHours;
            }
        }

        public IList<string> DeliveryAddresses
        {
            get
            {
                if (mDeliveryAddresses == null)
                {
                    mDeliveryAddresses = new List<string>();
                    mDeliveryAddresses.Add("تهران، ونک، پلاک ۱۷");
                    mDeliveryAddresses.Add("تهران، ونک، پلاک ۱۷");
                }

                return mDeliveryAddresses;
            }
        }

        public IList<OrderSchedule> GetOrderSchedules(int year, int month)
        {
            if (mOrderSchedules == null)
            {
                mOrderSchedules = new List<OrderSchedule>();
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 1,
                    LaunchCount = 1,
                    DinnerCount = 0
                });
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 2,
                    LaunchCount = 1,
                    DinnerCount = 0
                });
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 3,
                    LaunchCount = 0,
                    DinnerCount = 1
                });
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 4,
                    LaunchCount = 1,
                    DinnerCount = 0
                });
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 5,
                    LaunchCount = 1,
                    DinnerCount = 0
                });
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 6,
                    LaunchCount = 0,
                    DinnerCount = 1
                });
                mOrderSchedules.Add(new OrderSchedule
                {
                    Year = 1395,
                    Month = 9,
                    Day = 8,
                    LaunchCount = 0,
                    DinnerCount = 2
                });
            }

            return mOrderSchedules.Where(x => x.Year == year && x.Month == month).ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SaladioContext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}