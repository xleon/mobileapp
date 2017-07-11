using System.Collections.ObjectModel;

namespace Toggl.Foundation.Suggestions
{
    public interface ISuggestionProviderContainer
    {
        ReadOnlyCollection<ISuggestionProvider> Providers { get; }
    }
}
