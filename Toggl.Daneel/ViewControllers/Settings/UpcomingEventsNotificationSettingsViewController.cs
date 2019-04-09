using System.Reactive;
using CoreGraphics;
using Foundation;
using Toggl.Daneel.Cells.Settings;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Daneel.ViewSources.Generic.TableView;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewControllers.Settings
{
    using CalendarSectionModel = SectionModel<Unit, SelectableCalendarNotificationsOptionViewModel>;

    [ModalCardPresentation]
    public sealed partial class UpcomingEventsNotificationSettingsViewController : ReactiveViewController<UpcomingEventsNotificationSettingsViewModel>
    {

        private const int rowHeight = 44;

        public UpcomingEventsNotificationSettingsViewController() : base(nameof(UpcomingEventsNotificationSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TitleLabel.Text = Resources.UpcomingEvent;

            TableView.ScrollEnabled = false;
            TableView.TableFooterView = new UIView(CGRect.Empty);
            TableView.RegisterNibForCellReuse(UpcomingEventsOptionCell.Nib, UpcomingEventsOptionCell.Identifier);
            TableView.RowHeight = rowHeight;

            var source = new CustomTableViewSource<CalendarSectionModel, Unit, SelectableCalendarNotificationsOptionViewModel>(
                UpcomingEventsOptionCell.CellConfiguration(UpcomingEventsOptionCell.Identifier),
                ViewModel.AvailableOptions
            );

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectOption.Inputs)
                .DisposedBy(DisposeBag);

            TableView.Source = source;

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);
        }
    }
}
