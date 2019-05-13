using System.Reactive;
using System.Reactive.Disposables;
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
    public partial class SelectDurationFormatViewController : ReactiveViewController<SelectDurationFormatViewModel>, IDismissableViewController
    {
        private const int rowHeight = 48;

        CompositeDisposable disposeBag = new CompositeDisposable();

        public SelectDurationFormatViewController(SelectDurationFormatViewModel viewModel)
            : base(viewModel, nameof(SelectDurationFormatViewController))
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

            TitleLabel.Text = Resources.DurationFormat;

            DurationFormatsTableView.RowHeight = rowHeight;
            DurationFormatsTableView.RegisterNibForCellReuse(DurationFormatViewCell.Nib, DurationFormatViewCell.Identifier);

            var source = new CustomTableViewSource<SectionModel<Unit, SelectableDurationFormatViewModel>, Unit, SelectableDurationFormatViewModel>(
                DurationFormatViewCell.CellConfiguration(DurationFormatViewCell.Identifier),
                ViewModel.DurationFormats
            );

            DurationFormatsTableView.Source = source;

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectDurationFormat.Inputs)
                .DisposedBy(disposeBag);

            BackButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(disposeBag);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            disposeBag.Dispose();
        }
    }
}
