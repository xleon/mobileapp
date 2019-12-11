using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using CoreGraphics;
using Foundation;
using Toggl.Core.Sync;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Helper;
using Toggl.iOS.Presentation.Transition;
using Toggl.Core.UI.ViewModels.Settings.Rows;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class DebugCommandsViewController : ReactiveViewController<DebugCommandsViewModel>
    {
        private readonly float bottomInset = 24;

        public DebugCommandsViewController(DebugCommandsViewModel viewModel)
            : base(viewModel, nameof(DebugCommandsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ((ReactiveNavigationController)NavigationController).SetBackgroundColor(ColorAssets.TableBackground);
            NavigationItem.RightBarButtonItem = ReactiveNavigationController.CreateSystemItem(
                Resources.Done, UIBarButtonItemStyle.Done, ViewModel.Close);
            NavigationItem.BackBarButtonItem.Title = Resources.Back;

            var source = new SettingsTableViewSource(TableView);
            TableView.Source = source;
            TableView.TableFooterView = new UIView(frame: new CGRect(0, 0, 0, bottomInset));

            ViewModel.TableSections
                .Subscribe(TableView.Rx().ReloadSections(source))
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(handleTap)
                .DisposedBy(DisposeBag);

            Title = ViewModel.Title;
        }

        private void handleTap(ISettingRow row)
        {
            row.Action.Execute();
        }
    }
}

