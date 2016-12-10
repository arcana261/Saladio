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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.DialogCalendar, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            using (FragmentTransaction transaction = this.ChildFragmentManager.BeginTransaction())
            {
                transaction.Replace(Resource.Id.frameCalendarContainer, new FragmentCalendar());
                transaction.Commit();
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}