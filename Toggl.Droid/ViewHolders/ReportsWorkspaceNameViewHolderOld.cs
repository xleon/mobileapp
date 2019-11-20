using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace Toggl.Droid.ViewHolders
{
    public class ReportsWorkspaceNameViewHolderOld : BaseRecyclerViewHolder<string>
    {
        private TextView workspaceName;

        public ReportsWorkspaceNameViewHolderOld(View itemView) : base(itemView)
        {
        }

        public ReportsWorkspaceNameViewHolderOld(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            workspaceName = (TextView)ItemView;
        }

        protected override void UpdateView()
        {
            workspaceName.Text = Item;
        }
    }
}
