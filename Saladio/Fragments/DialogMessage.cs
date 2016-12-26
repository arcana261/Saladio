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
    public class DialogMessage : DialogFragment
    {
        private string mMessage;
        private TextView mTxtMessage;
        private Button mBtnOk;

        public DialogMessage(string message)
        {
            mMessage = message;
        }

        public DialogMessage()
        {
            mMessage = "";
        }

        public event EventHandler<DialogDismissEventArgs> DialogDismissEvent;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.DialogMessage, container);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString("message", mMessage);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);

            DialogDismissEvent?.Invoke(this, new DialogDismissEventArgs());
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            if (savedInstanceState != null)
            {
                mMessage = savedInstanceState.GetString("message");
            }

            mTxtMessage = view.FindViewById<TextView>(Resource.Id.txtMessage);
            mBtnOk = view.FindViewById<Button>(Resource.Id.btnOk);

            mTxtMessage.Text = mMessage;
            mBtnOk.Click += BtnOk_Click;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}