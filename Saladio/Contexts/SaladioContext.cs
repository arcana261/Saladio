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
using IO.Swagger.Api;
using Saladio.Utility;
using Saladio.Activities;
using IO.Swagger.Model;
using Saladio.Config;

namespace Saladio.Contexts
{
    public class SaladioContext : IDisposable
    {
        private SharedActivity mOwner;
        private static IList<SaladComponentGroup> mGroups;
        private static IList<ClassicSaladCatagory> mClassicSaladCatagories;
        private static IDictionary<int, IList<ClassicSalad>> mClassicSalads;
        private static IDictionary<int, SaladComponent> mSaladComponentById;
        private static IList<SavedSalad> mSavedSalads;
        private static IList<DeliverySchedule> mDeliveryHours;
        private static IList<string> mDeliveryAddresses;
        private static IList<OrderSchedule> mOrderSchedules;

        public SaladioContext()
        {
        }

        public SaladioContext(SharedActivity owner)
        {
            mOwner = owner;
        }

        public SharedActivity Owner
        {
            get
            {
                return mOwner;
            }
        }

        public IList<ClassicSalad> GetClassicSaladsByCatagory(ClassicSaladCatagory catagory)
        {
            return GetClassicSaladsByCatagory(catagory.Id.Value);
        }

        public IList<ClassicSalad> GetClassicSaladsByCatagory(int catagoryId)
        {
            if (mClassicSalads == null)
            {
                mClassicSalads = new Dictionary<int, IList<ClassicSalad>>();
            }

            if (!mClassicSalads.ContainsKey(catagoryId))
            {
                try
                {
                    using (mOwner.OpenLoadingFromThread())
                    {
                        ClassicSaladsApi api = new ClassicSaladsApi(SharedConfig.AuthorizedApiConfig);
                        IList<ClassicSalad> salads = new List<ClassicSalad>();

                        PagedApiHelper.FetchAll((start, length) =>
                        {
                            var res = api.GetClassicSaladsByCatagory(catagoryId, length, start);

                            foreach (var item in res.Data)
                            {
                                salads.Add(item);
                            }

                            return res.RecordsFiltered.Value;
                        });

                        mClassicSalads[catagoryId] = salads;
                    }
                }
                catch(Exception e)
                {
                    mOwner.ShowMessageDialogForExceptionFromThread(e);
                    return new List<ClassicSalad>();
                }
            }

            return mClassicSalads[catagoryId];
        }

        public IList<ClassicSaladCatagory> ClassicSaladCatagories
        {
            get
            {
                if (mClassicSaladCatagories == null)
                {
                    try
                    {
                        using (mOwner.OpenLoadingFromThread())
                        {
                            ClassicSaladCatagoriesApi catagoryApi = new ClassicSaladCatagoriesApi(SharedConfig.AuthorizedApiConfig);
                            mClassicSaladCatagories = new List<ClassicSaladCatagory>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = catagoryApi.GetClassicSaladCatagories(length, start);

                                foreach (var item in res.Data)
                                {
                                    mClassicSaladCatagories.Add(item);
                                }

                                return res.RecordsFiltered.Value;
                            });
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<ClassicSaladCatagory>();
                    }
                }

                return mClassicSaladCatagories;
            }
        }

        public SaladComponent GetSaladComponentById(int saladComponentId)
        {
            if (mSaladComponentById == null)
            {
                mSaladComponentById = new Dictionary<int, SaladComponent>();
            }

            if (!mSaladComponentById.ContainsKey(saladComponentId))
            {
                try
                {
                    SaladComponentsApi api = new SaladComponentsApi(SharedConfig.AuthorizedApiConfig);
                    var res = api.GetSaladComponentById(saladComponentId);

                    mSaladComponentById[saladComponentId] = res;
                    return res;
                }
                catch(Exception e)
                {
                    mOwner.ShowMessageDialogForExceptionFromThread(e);
                    return null;
                }
            }

            return mSaladComponentById[saladComponentId];
        }

        public IList<SaladComponentGroup> SaladComponentGroups
        {
            get
            {
                if (mGroups == null)
                {
                    try
                    {
                        using (var handle = mOwner.OpenLoadingFromThread())
                        {
                            SaladComponetGroupsApi api = new SaladComponetGroupsApi(SharedConfig.AuthorizedApiConfig);
                            mGroups = new List<SaladComponentGroup>();
                            mSaladComponentById = new Dictionary<int, SaladComponent>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = api.GetSaladComponentGroup(length, start);
                                foreach (var item in res.Data)
                                {
                                    mGroups.Add(item);

                                    foreach (var component in item.Items)
                                    {
                                        mSaladComponentById[component.Id.Value] = component;
                                    }
                                }

                                return res.RecordsFiltered.Value;
                            });
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<SaladComponentGroup>();
                    }
                }

                return mGroups;
            }
        }

        public IList<SavedSalad> SavedSalads
        {
            get
            {
                if (mSavedSalads == null)
                {
                    try
                    {
                        using (mOwner.OpenLoadingFromThread())
                        {
                            SavedSaladsApi api = new SavedSaladsApi(SharedConfig.AuthorizedApiConfig);
                            mSavedSalads = new List<SavedSalad>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = api.GetSavedSaladsByMe(length, start);

                                foreach (var item in res.Data)
                                {
                                    mSavedSalads.Add(item);
                                }

                                return res.RecordsFiltered.Value;
                            });
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<SavedSalad>();
                    }
                }

                return mSavedSalads;
            }
        }

        public IList<DeliverySchedule> DeliveryHours
        {
            get
            {
                if (mDeliveryHours == null)
                {
                    try
                    {
                        using (var handle = mOwner.OpenLoadingFromThread())
                        {
                            DeliverySchedulesApi api = new DeliverySchedulesApi(SharedConfig.AuthorizedApiConfig);
                            mDeliveryHours = new List<DeliverySchedule>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = api.GetDeliverySchedules(length, null, start);
                                foreach (var item in res.Data)
                                {
                                    mDeliveryHours.Add(item);
                                }

                                return res.RecordsFiltered.Value;
                            });
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<DeliverySchedule>();
                    }
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
                    try
                    {
                        using (var handle = mOwner.OpenLoadingFromThread())
                        {
                            UsersApi api = new UsersApi(SharedConfig.AuthorizedApiConfig);
                            mDeliveryAddresses = api.GetCurrentUser().Addresses;
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<string>();
                    }
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