using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant
{
    public interface ISinceParameterRepository
    {
        ISinceParameters Get();

        void Set(ISinceParameters parameters);
    }
}
