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

namespace Saladio.Contexts
{
    public class SaladioContext : IDisposable
    {
        private IList<SaladComponentGroup> mGroups = new List<SaladComponentGroup>();

        public SaladioContext()
        {
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

        public IList<SaladComponentGroup> SaladComponentGroups
        {
            get { return mGroups; }
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