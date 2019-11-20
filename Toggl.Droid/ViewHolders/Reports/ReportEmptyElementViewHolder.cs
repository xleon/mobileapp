using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.Reports;

namespace Toggl.Droid.ViewHolders.Reports
{
    [Obsolete("Remove when all report elements are implemented.")]
    public class ReportEmptyElementViewHolder : ReportElementViewHolder<IReportElement>
    {
        private TextView label;

        public ReportEmptyElementViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportEmptyElementViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            label = (TextView)ItemView;
        }

        protected override void UpdateView()
        {
            if (Item is ReportElementBase element && element.IsLoading)
            {
                label.Text = $"{Item.GetType().Name} (LOADING)";
                return;
            }

            label.Text = Item.GetType().Name;
        }
    }
}
