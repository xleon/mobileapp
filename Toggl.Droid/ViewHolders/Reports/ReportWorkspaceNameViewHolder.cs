using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.Reports;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportWorkspaceNameViewHolder : ReportElementViewHolder<ReportWorkspaceNameElement>
    {
        private TextView workspaceNameLabel;

        public ReportWorkspaceNameViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportWorkspaceNameViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            workspaceNameLabel = (TextView)ItemView;
        }

        protected override void UpdateView()
        {
            workspaceNameLabel.Text = Item.Name;
        }
    }
}
