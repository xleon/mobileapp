using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectDateFormatViewModel : ViewModel<DateFormat, DateFormat>
    {
        private readonly INavigationService navigationService;
        private readonly DateFormat[] availableDateFormats =
        {
            DateFormat.FromLocalizedDateFormat("MM/DD/YYYY"),
            DateFormat.FromLocalizedDateFormat("DD-MM-YYYY"),
            DateFormat.FromLocalizedDateFormat("MM-DD-YYYY"),
            DateFormat.FromLocalizedDateFormat("YYYY-MM-DD"),
            DateFormat.FromLocalizedDateFormat("DD/MM/YYYY"),
            DateFormat.FromLocalizedDateFormat("DD.MM.YYYY")
        };

        private DateFormat defaultResult;

        public ImmutableList<SelectableDateFormatViewModel> DateTimeFormats { get; }

        public UIAction Close { get; }

        public InputAction<SelectableDateFormatViewModel> SelectDateFormat { get; }

        public SelectDateFormatViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;

            Close = rxActionFactory.FromAsync(close);
            SelectDateFormat = rxActionFactory.FromAsync<SelectableDateFormatViewModel>(selectFormat);

            DateTimeFormats = availableDateFormats
                .Select(dateFormat => new SelectableDateFormatViewModel(dateFormat, false))
                .ToImmutableList();
        }

        public override Task Initialize(DateFormat parameter)
        {
            defaultResult = parameter;
            updateSelectedFormat(parameter);

            return base.Initialize(parameter);
        }

        private Task close() => Finish(defaultResult);

        private Task selectFormat(SelectableDateFormatViewModel dateFormatViewModel)
            => Finish(dateFormatViewModel.DateFormat);

        private void updateSelectedFormat(DateFormat selected)
            => DateTimeFormats.ForEach(dateFormat
                => dateFormat.Selected = dateFormat.DateFormat == selected);
    }
}
