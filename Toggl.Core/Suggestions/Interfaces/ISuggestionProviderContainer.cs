using System.Collections.ObjectModel;

namespace Toggl.Core.Suggestions
{
    public interface ISuggestionProviderContainer
    {
        ReadOnlyCollection<ISuggestionProvider> Providers { get; }
    }
}
