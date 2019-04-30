using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDurationFormatViewModel : ViewModel<DurationFormat, DurationFormat>
    {
        private readonly INavigationService navigationService;

        private DurationFormat defaultResult;

        public IImmutableList<SelectableDurationFormatViewModel> DurationFormats { get; }

        public UIAction Close { get; }

        public InputAction<SelectableDurationFormatViewModel> SelectDurationFormat { get; }

        public SelectDurationFormatViewModel(
            INavigationService navigationService,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;

            Close = rxActionFactory.FromAsync(close);
            SelectDurationFormat = rxActionFactory.FromAsync<SelectableDurationFormatViewModel>(selectFormat);

            DurationFormats = Enum.GetValues(typeof(DurationFormat))
                            .Cast<DurationFormat>()
                            .Select(durationFormat => new SelectableDurationFormatViewModel(durationFormat, false))
                            .ToImmutableList();
        }

        public override Task Initialize(DurationFormat defaultDuration)
        {
            defaultResult = defaultDuration;
            updateSelectedFormat(defaultDuration);

            return base.Initialize(defaultDuration);
        }

        private Task close() => Finish(defaultResult);

        private Task selectFormat(SelectableDurationFormatViewModel viewModel)
            => Finish(viewModel.DurationFormat);

        private void updateSelectedFormat(DurationFormat selected)
            => DurationFormats.ForEach(viewModel
                => viewModel.Selected = viewModel.DurationFormat == selected);
    }
}
