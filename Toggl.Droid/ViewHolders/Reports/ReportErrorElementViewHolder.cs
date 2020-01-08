using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportErrorElementViewHolder : ReportElementViewHolder<ReportErrorElement>
    {
        private TextView title;
        private TextView message;

        public ReportErrorElementViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportErrorElementViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            title = ItemView.FindViewById<TextView>(Resource.Id.Title);
            message = ItemView.FindViewById<TextView>(Resource.Id.Message);

            title.Text = "Ooooops!";
        }

        protected override void UpdateView()
        {
            message.Text = Item.Message;
        }
    }
}
