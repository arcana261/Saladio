using Android.App;
using Android.Widget;
using Android.OS;
using Saladio.Fragments;

namespace Saladio.Activities
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/ic_pie_salad_64")]
    public class StartActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Start);

            using (var transaction = FragmentManager.BeginTransaction())
            {
                var welcome = new FragmentWelcome();
                var wizard = new FragmentWizard(welcome);

                transaction.Replace(Resource.Id.layoutWizardContainer, wizard);
                transaction.Commit();
            }
        }
    }
}

