using System.Reactive;
using Toggl.Core.UI.ViewModels;
using UIKit;
using Toggl.iOS.Extensions;
using System.Threading.Tasks;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core;
using Toggl.Shared.Extensions;
using Toggl.Core.UI.Collections;
using Toggl.iOS.Presentation.Attributes;
using Toggl.iOS.Views.CountrySelection;
using Toggl.iOS.ViewSources.Generic.TableView;

namespace Toggl.iOS.ViewControllers
{
    [ModalCardPresentation]
    public sealed partial class SelectCountryViewController : KeyboardAwareViewController<SelectCountryViewModel>, IDismissableViewController
    {
        private const int rowHeight = 48;

        public SelectCountryViewController() : base(nameof(SelectCountryViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.CountryOfResidence;
            SearchTextField.Placeholder = Resources.Search;

            CountriesTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            CountriesTableView.RegisterNibForCellReuse(CountryViewCell.Nib, CountryViewCell.Identifier);
            CountriesTableView.RowHeight = rowHeight;

            var source = new CustomTableViewSource<SectionModel<string, SelectableCountryViewModel>, string, SelectableCountryViewModel>(
                CountryViewCell.CellConfiguration(CountryViewCell.Identifier));

            CountriesTableView.Source = source;

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectCountry.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Countries
                .Subscribe(CountriesTableView.Rx().ReloadItems(source))
                .DisposedBy(DisposeBag);

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            SearchTextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);



            SearchTextField.BecomeFirstResponder();
        }

        public async Task<bool> Dismiss()
        {
            ViewModel.Close.Execute(Unit.Default);
            return true;
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.AnimateSetConstant(e.FrameEnd.Height, View);
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.AnimateSetConstant(0, View);
        }
    }
}

