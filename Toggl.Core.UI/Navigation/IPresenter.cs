using System;
using System.Linq;
using System.Threading.Tasks;

namespace Toggl.Core.UI.Navigation
{
    public interface IPresenter
    {
        bool CanPresent<TInput>(NavigationInfo<TInput> navigationInfo);

        Task<TOutput> Present<TInput, TOutput>(NavigationInfo<TInput> navigationInfo);
    }
    
    public sealed class CompositePresenter : IPresenter
    {
        private readonly IPresenter[] presenters;

        public CompositePresenter(params IPresenter[] presenters)
        {
            this.presenters = presenters;
        }

        public bool CanPresent<TInput>(NavigationInfo<TInput> navigationInfo)
            => presenters.Any(p => p.CanPresent(navigationInfo));

        public Task<TOutput> Present<TInput, TOutput>(NavigationInfo<TInput> navigationInfo)
        {
            var presenter = presenters.FirstOrDefault(p => p.CanPresent(navigationInfo));
            if (presenter == null)
                throw new InvalidOperationException($"Failed to find a presenter that could present ViewModel with type {navigationInfo.ViewModelType.Name}");

            return presenter.Present<TInput, TOutput>(navigationInfo);
        }
    }
    
}
