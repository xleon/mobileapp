using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Droid.Extensions;
using static Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel;

namespace Toggl.Droid.ViewHolders
{
    public class DateRangePickerShortcutViewHolder : BaseRecyclerViewHolder<Shortcut>
    {
        private Color normalShortcutColor;
        private Color selectedShortcutColor;

        private TextView shortcutText;
        private GradientDrawable backgroundDrawable;

        public DateRangePickerShortcutViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public DateRangePickerShortcutViewHolder(View itemView, Context context) : base(itemView)
        {
            normalShortcutColor = context.SafeGetColor(Resource.Color.shortcutBackground);
            selectedShortcutColor = context.SafeGetColor(Resource.Color.selectionBackground);
        }

        protected override void InitializeViews()
        {
            shortcutText = (TextView)ItemView;
            backgroundDrawable = (GradientDrawable)shortcutText.Background;
        }

        protected override void UpdateView()
        {
            shortcutText.Text = Item.Text;

            var color = Item.IsSelected
                ? selectedShortcutColor
                : normalShortcutColor;

            backgroundDrawable.SetColor(color);
            backgroundDrawable.InvalidateSelf();
        }
    }
}
