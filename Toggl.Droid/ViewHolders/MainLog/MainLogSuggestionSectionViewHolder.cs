using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders.MainLog
{
    public class MainLogSuggestionSectionViewHolder : BaseRecyclerViewHolder<MainLogItemViewModel>
    {
        private TextView mainLogHeaderTitle;

        public MainLogSuggestionSectionViewHolder(View itemView) : base(itemView)
        {
        }

        public MainLogSuggestionSectionViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            mainLogHeaderTitle = ItemView.FindViewById<TextView>(Resource.Id.MainLogSuggestionsHeaderTitle);
        }

        protected override void UpdateView()
        {
            mainLogHeaderTitle.Text = ((SuggestionsHeaderViewModel) Item).Title;
        }
    }
}
