using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDurationFormatViewModel : ViewModel<DurationFormat, DurationFormat>
    {
        private DurationFormat defaultResult;

        public IImmutableList<SelectableDurationFormatViewModel> DurationFormats { get; }
        public InputAction<SelectableDurationFormatViewModel> SelectDurationFormat { get; }

        public SelectDurationFormatViewModel(
            INavigationService navigationService,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            SelectDurationFormat = rxActionFactory.FromAction<SelectableDurationFormatViewModel>(selectFormat);

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

        public override void CloseWithDefaultResult()
        {
            Close(defaultResult);
        }

        private void selectFormat(SelectableDurationFormatViewModel viewModel)
        {
            Close(viewModel.DurationFormat);
        }

        private void updateSelectedFormat(DurationFormat selected)
            => DurationFormats.ForEach(viewModel
                => viewModel.Selected = viewModel.DurationFormat == selected);
    }
}
