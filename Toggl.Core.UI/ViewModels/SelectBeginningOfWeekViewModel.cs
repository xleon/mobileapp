using System;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectBeginningOfWeekViewModel : ViewModel<BeginningOfWeek, BeginningOfWeek>
    {
        private BeginningOfWeek defaultResult;

        public SelectableBeginningOfWeekViewModel[] BeginningOfWeekCollection { get; }

        public InputAction<SelectableBeginningOfWeekViewModel> SelectBeginningOfWeek { get; }

        public SelectBeginningOfWeekViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            SelectBeginningOfWeek = rxActionFactory.FromAction<SelectableBeginningOfWeekViewModel>(selectFormat);

            BeginningOfWeekCollection = Enum.GetValues(typeof(BeginningOfWeek))
                .Cast<BeginningOfWeek>()
                .Select(beginningOfWeek => new SelectableBeginningOfWeekViewModel(beginningOfWeek, false))
                .ToArray();
        }

        public override Task Initialize(BeginningOfWeek defaultValue)
        {
            defaultResult = defaultValue;
            updateSelectedFormat(defaultValue);

            return base.Initialize(defaultValue);
        }

        public override void CloseWithDefaultResult()
        {
            Close(defaultResult);
        }

        private void selectFormat(SelectableBeginningOfWeekViewModel viewModel)
        {
            Close(viewModel.BeginningOfWeek);
        }

        private void updateSelectedFormat(BeginningOfWeek selected)
            => BeginningOfWeekCollection.ForEach(viewModel
                => viewModel.Selected = viewModel.BeginningOfWeek == selected);
    }
}
