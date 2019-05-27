using System;
using System.Reactive;
using System.Reactive.Linq;
using Foundation;
using Toggl.Core.Models;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Views.Settings;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings.Siri
{
    public partial class SiriWorkflowsViewController : ReactiveViewController<SiriWorkflowsViewModel>
    {
        public SiriWorkflowsViewController() : base(nameof(SiriWorkflowsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.Siri_Workflows;

            HeaderLabel.Text = Resources.Siri_Workflows_Description;
            HeaderView.RemoveFromSuperview();
            HeaderView.BackgroundColor = Colors.Siri.HeaderBackground.ToNativeColor();
            TableView.TableHeaderView = HeaderView;
            HeaderView.TranslatesAutoresizingMaskIntoConstraints = false;
            HeaderView.WidthAnchor.ConstraintEqualTo(TableView.WidthAnchor).Active = true;

            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.BackgroundColor = Colors.Siri.HeaderBackground.ToNativeColor();
            TableView.TableFooterView = new UIView();
            TableView.ContentInset = new UIEdgeInsets(0, 0, 20, 0);

            TableView.RegisterNibForCellReuse(SiriWorkflowCell.Nib, SiriWorkflowCell.Identifier);
            TableView.RowHeight = UITableView.AutomaticDimension;

            var source = new CustomTableViewSource<SectionModel<Unit, SiriWorkflow>, Unit, SiriWorkflow>(
                SiriWorkflowCell.CellConfiguration(SiriWorkflowCell.Identifier)
            );

            ViewModel.Workflows
                .Subscribe(TableView.Rx().ReloadItems(source))
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(workflowSelected)
                .DisposedBy(DisposeBag);

            TableView.Source = source;
        }

        private void workflowSelected(SiriWorkflow workflow)
        {
            var path = ViewModel.PathForWorkflow(workflow);
            var escapedPath =
                ((NSString) path).CreateStringByAddingPercentEncoding(NSUrlUtilities_NSCharacterSet
                    .UrlQueryAllowedCharacterSet);
            var url = new NSUrl(escapedPath);

            if (!UIApplication.SharedApplication.CanOpenUrl(url))
            {

                var alert = UIAlertController.Create(Resources.CantOpenWorkflowTitle, Resources.CantOpenWorkflowDescription, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(Resources.Cancel, UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create(Resources.Download, UIAlertActionStyle.Default, (obj) => {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/app/shortcuts/id915249334"));
                }));
                PresentViewController(alert, true, null);
                return;
            }

            UIApplication.SharedApplication.OpenUrl(url);
        }
    }
}

