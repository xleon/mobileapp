using Toggl.Foundation.Models;
using Toggl.Multivac.Models;

namespace Toggl.Foundation
{
    public interface IIntentDonationService
    {
        void StopTimeEntry(ITimeEntry te);
    }
}