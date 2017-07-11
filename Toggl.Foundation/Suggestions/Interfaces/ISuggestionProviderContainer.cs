using System.Collections.Immutable;

namespace Toggl.Foundation.Suggestions
{
    public interface ISuggestionProviderContainer
    {
        ImmutableList<ISuggestionProvider> Providers { get; }
    }
}
