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
using Android.Support.V4.App;
using Saladio.Fragments;
using IO.Swagger.Client;
using Saladio.Contexts;
using System.Threading.Tasks;
using System.Threading;

namespace Saladio.Activities
{
    public class SharedActivity : Activity
    {
        private DialogLoader mDialogLoader;
        private volatile int mLoaderCount = 0;
        private SaladioContext mDataContext = null;
        private object monitor = new object();
        private volatile int mLoaderOperationId = 0;

        public SaladioContext DataContext
        {
            get
            {
                if (mDataContext == null)
                {
                    mDataContext = new SaladioContext(this);
                }

                return mDataContext;
            }
        }

        private class LoaderDialogHandler : IDisposable
        {
            private Action mCloser;

            public LoaderDialogHandler(Action closer)
            {
                mCloser = closer;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        mCloser();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~LoaderDialogHandler() {
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

        public void ShowToast(int resource)
        {
            Toast.MakeText(this, Resources.GetString(resource), ToastLength.Short).Show();
        }

        public void ShowMessageDialog(int resource)
        {
            using (Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction())
            {
                DialogMessage dialog = new DialogMessage(Resources.GetString(resource));
                dialog.Show(transaction, "dialogMessage");
            }
        }

        public void ShowMessageDialogForExceptionFromThread(Exception err)
        {
            RunOnUiThread(() =>
            {
                ShowMessageDialogForException(err);
            });
        }

        public void ShowMessageDialogForException(Exception err)
        {
            int resource = Resource.String.ToastErrorUnknown;

            if (err is ApiException)
            {
                ApiException apiErr = (ApiException)err;

                switch (apiErr.ErrorCode)
                {
                    case 400:
                        resource = Resource.String.ToastApiErrorBadRequest;
                        break;
                    case 500:
                        resource = Resource.String.ToastApiErrorInternalServerError;
                        break;
                    case 409:
                        resource = Resource.String.ToastApiErrorConflict;
                        break;
                    case 401:
                    case 403:
                        resource = Resource.String.ToastApiErrorForbidden;
                        break;
                    case 404:
                        resource = Resource.String.ToastApiErrorNotFound;
                        break;
                    default:
                        resource = Resource.String.ToastApiError;
                        break;
                }
            }

            ShowMessageDialog(resource);
        }

        public IDisposable OpenLoading()
        {
            ShowLoading();
            return new LoaderDialogHandler(() =>
            {
                CloseLoading();
            });
        }

        public IDisposable OpenLoadingFromThread()
        {
            RunOnUiThread(() =>
            {
                ShowLoading();
            });
            return new LoaderDialogHandler(() =>
            {
                RunOnUiThread(() =>
                {
                    CloseLoading();
                });
            });
        }

        private void ShowLoading()
        {
            lock (monitor)
            {
                if (mDialogLoader == null)
                {
                    using (Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction())
                    {
                        mDialogLoader = new DialogLoader();
                        mDialogLoader.Show(transaction, "dialogLoader");
                    }
                }


                mLoaderCount++;
                mLoaderOperationId++;
            }
        }

        private void CloseLoading()
        {
            lock (monitor)
            {
                if (mDialogLoader != null)
                {
                    mLoaderCount--;
                    int id = mLoaderOperationId;

                    if (mLoaderCount < 1)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(500);
                            if (!IsDestroyed && !IsFinishing)
                            {
                                RunOnUiThread(() =>
                                {
                                    lock (monitor)
                                    {
                                        if (id == mLoaderOperationId)
                                        {
                                            mLoaderOperationId++;

                                            if (mDialogLoader != null)
                                            {
                                                mDialogLoader.Dismiss();
                                                mDialogLoader = null;
                                            }
                                        }
                                    }
                                });
                            }
                        });
                    }
                }
            }
        }
    }
}