using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels.Reports;

namespace Toggl.Droid.ViewHolders.Reports
{
    public abstract class ReportElementViewHolder<T> : BaseRecyclerViewHolder<IReportElement>
        where T : IReportElement
    {
        public new T Item
        {
            get => (T)base.Item;
            set => base.Item = value;
        }

        protected ReportElementViewHolder(View itemView) : base(itemView)
        {
        }

        protected ReportElementViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }
    }
}
