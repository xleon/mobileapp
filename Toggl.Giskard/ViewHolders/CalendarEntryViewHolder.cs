using System;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Foundation.Calendar;
using Toggl.Giskard.Extensions;

namespace Toggl.Giskard.ViewHolders
{
    public class CalendarEntryViewHolder : BaseRecyclerViewHolder<CalendarItem>
    {
        private bool isInEditMode;
        private float defaultElevation;
        private TextView label;

        public CalendarEntryViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CalendarEntryViewHolder(View itemView) : base(itemView)
        {
        }

        protected override void InitializeViews()
        {
            label = ItemView.FindViewById<TextView>(Resource.Id.EntryLabel);
            defaultElevation = ItemView.Elevation;
        }

        protected override void UpdateView()
        {
            updateBackground();
            updateTextColor();
            label.Text = Item.Description;
        }

        public void SetIsInEditMode(bool editModeEnabled)
        {
            isInEditMode = editModeEnabled;
            ItemView.Elevation = isInEditMode
                ? defaultElevation + 4.DpToPixels(ItemView.Context)
                : defaultElevation;
        }

        private void updateBackground()
        {
            var color = Color.ParseColor(Item.Color);
            if (Item.Source == CalendarItemSource.Calendar)
                color.A = (byte) (color.A * 0.25);

            ItemView.Background.SetTint(color);
        }

        private void updateTextColor()
        {
            var color = Item.Source == CalendarItemSource.Calendar ? Color.ParseColor(Item.Color) : Color.White;
            label.SetTextColor(color);
        }
    }
}
