using System.Reactive;
using System.Threading.Tasks;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Views.Settings;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers
{
    public partial class SelectBeginningOfWeekViewController : ReactiveViewController<SelectBeginningOfWeekViewModel>, IDismissableViewController
    {
        public SelectBeginningOfWeekViewController(SelectBeginningOfWeekViewModel viewModel)
            : base(viewModel, nameof(SelectBeginningOfWeekViewController))
        {
        }

        public async Task<bool> Dismiss()
        {
            ViewModel.Close.Execute();
            return true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.FirstDayOfTheWeek;

            DaysTableView.RegisterNibForCellReuse(DayOfWeekViewCell.Nib, DayOfWeekViewCell.Identifier);

            var source = new CustomTableViewSource<SectionModel<Unit, SelectableBeginningOfWeekViewModel>, Unit, SelectableBeginningOfWeekViewModel>(
                DayOfWeekViewCell.CellConfiguration(DayOfWeekViewCell.Identifier),
                ViewModel.BeginningOfWeekCollection
            );

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectBeginningOfWeek.Inputs)
                .DisposedBy(DisposeBag);

            DaysTableView.Source = source;

            BackButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);
        }
    }
}

