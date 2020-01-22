using System;
using System.Reactive;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Toggl.Droid.ViewHolders.Settings
{
    public class ToggleRowViewWithDescriptionView : SettingsRowView<ToggleRowWithDescription>
    {
        public static ToggleRowViewWithDescriptionView Create(Context context) 
            => new ToggleRowViewWithDescriptionView(LayoutInflater.From(context).Inflate(Resource.Layout.SettingsToggleRowWithDescription, null, false));
        
        private readonly TextView title;
        private readonly TextView description;
        private readonly Switch switchView;

        public ToggleRowViewWithDescriptionView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private ToggleRowViewWithDescriptionView(View itemView) : base(itemView)
        {
            title = ItemView.FindViewById<TextView>(Resource.Id.Title);
            description = ItemView.FindViewById<TextView>(Resource.Id.Description);
            switchView = ItemView.FindViewById<Switch>(Resource.Id.Switch);
            ItemView.Click += OnItemViewClick;
        }

        protected override void OnRowDataChanged()
        {
            title.Text = RowData.Title;
            description.Text = RowData.Description;
            if (RowData.Value != switchView.Checked)
            {
                switchView.Checked = RowData.Value;
            }
        }
        
        protected virtual void OnItemViewClick(object sender, EventArgs args)
        {
            RowData?.Action.Inputs.OnNext(Unit.Default);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || ItemView == null) return;
            ItemView.Click -= OnItemViewClick;
        }
    }
}
