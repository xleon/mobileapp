using Foundation;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class SelectableUserCalendarViewCell : BaseTableViewCell<SelectableUserCalendarViewModel>
    {
        public static readonly string Identifier = nameof(SelectableUserCalendarViewCell);
        public static readonly NSString Key = new NSString(nameof(SelectableUserCalendarViewCell));
        public static readonly UINib Nib;

        public Action Callback;
        public CompositeDisposable DisposeBag = new CompositeDisposable();

        static SelectableUserCalendarViewCell()
        {
            Nib = UINib.FromName(nameof(SelectableUserCalendarViewCell), NSBundle.MainBundle);
        }

        protected SelectableUserCalendarViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            FadeView.FadeRight = true;
            ContentView.InsertSeparator();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
        }

        public void ToggleSwitch()
        {
            IsSelectedSwitch.SetState(!IsSelectedSwitch.On, animated: true);
        }

        protected override void UpdateView()
        {
            CalendarNameLabel.Text = Item.Name;
            IsSelectedSwitch.SetState(Item.Selected, animated: false);
            IsSelectedSwitch.Rx().Changed()
                            .Delay(TimeSpan.FromSeconds(0.5)) // This is so the switch animation has time to finish before refresh
                            .Subscribe(_ => {
                                Callback();
                            })
                            .DisposedBy(DisposeBag);
        }
    }
}
