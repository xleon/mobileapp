using StoreKit;
using Toggl.Core.Services;

namespace Toggl.Daneel.Services
{
    public sealed class RatingServiceIos : IRatingService
    {
        public void AskForRating()
        {
            SKStoreReviewController.RequestReview();
        }
    }
}
