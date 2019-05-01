using System.Reactive.Disposables;
using System.Threading.Tasks;
using MvvmCross.Platforms.Ios.Views;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.iOS.Presentation.Attributes;
using Toggl.iOS.Views.Settings;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers
{
    [ModalCardPresentation]
    public sealed partial class SelectDateFormatViewController
        : ReactiveViewController<SelectDateFormatViewModel>,
          IDismissableViewController
    {
        private const int rowHeight = 48;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public SelectDateFormatViewController() 
            : base(nameof(SelectDateFormatViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.DateFormat;

            DateFormatsTableView.RegisterNibForCellReuse(DateFormatViewCell.Nib, DateFormatViewCell.Identifier);
            DateFormatsTableView.RowHeight = rowHeight;

            var source = new CustomTableViewSource<SectionModel<string, SelectableDateFormatViewModel>, string, SelectableDateFormatViewModel>(
                DateFormatViewCell.CellConfiguration(DateFormatViewCell.Identifier),
                ViewModel.DateTimeFormats
            );

            DateFormatsTableView.Source = source;

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectDateFormat.Inputs)
                .DisposedBy(disposeBag);

            BackButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(disposeBag);
        }

        public async Task<bool> Dismiss()
        {
            ViewModel.Close.Execute();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            disposeBag.Dispose();
        }
    }
}

