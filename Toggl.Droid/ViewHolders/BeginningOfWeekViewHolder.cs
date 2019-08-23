using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Globalization;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public sealed class BeginningOfWeekViewHolder : BaseRecyclerViewHolder<SelectableBeginningOfWeekViewModel>
    {
        private TextView beginningOfWeekTextView;
        private RadioButton selectedButton;

        public static BeginningOfWeekViewHolder Create(View itemView)
            => new BeginningOfWeekViewHolder(itemView);

        public BeginningOfWeekViewHolder(View itemView)
            : base(itemView)
        {
        }

        public BeginningOfWeekViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            beginningOfWeekTextView = ItemView.FindViewById<TextView>(Resource.Id.BeginningOfWeekText);
            selectedButton = ItemView.FindViewById<RadioButton>(Resource.Id.BeginningOfWeekRadioButton);
        }

        protected override void UpdateView()
        {
            beginningOfWeekTextView.Text = Item.BeginningOfWeek.ToLocalizedString();
            selectedButton.Checked = Item.Selected;
        }
    }
}
