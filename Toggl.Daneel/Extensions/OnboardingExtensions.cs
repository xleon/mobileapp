using System;
using CoreGraphics;
using Toggl.PrimeRadiant.Onboarding;
using UIKit;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.PrimeRadiant.Settings;
using Toggl.PrimeRadiant.Extensions;

namespace Toggl.Daneel.Extensions
{
    public static class Onboarding
    {
        private static readonly nfloat cellRadius = 8;

        private static readonly CGSize shadowOffset = new CGSize(0, 2);

        private static readonly CGColor shadowColor = UIColor.Black.CGColor;

        private static readonly nfloat shadowRadius = 6;

        private const float shadowOpacity = 0.1f;

        public static void MockSuggestion(this UIView view)
        {
            view.Layer.CornerRadius = cellRadius;
            view.Layer.ShadowOffset = shadowOffset;
            view.Layer.ShadowColor = shadowColor;
            view.Layer.ShadowOpacity = shadowOpacity;
            view.Layer.ShadowRadius = shadowRadius;
        }

        public static IDisposable ManageVisibilityOf(this IOnboardingStep step, UIView view)
            => step.ShouldBeVisible.Subscribe(
                visible => UIApplication.SharedApplication.InvokeOnMainThread(
                    () => view.Hidden = !visible));

        public static void DismissByTapping(this IDismissable step, UIView view)
        {
            var tapGestureRecognizer = new UITapGestureRecognizer(() => step.Dismiss());
            view.AddGestureRecognizer(tapGestureRecognizer);
        }

        public static IDisposable ManageDismissableTooltip(this IOnboardingStep step, UIView tooltip, IOnboardingStorage storage)
        {
            var dismissableStep = step.ToDismissable(step.GetType().FullName, storage);
            dismissableStep.DismissByTapping(tooltip);
            return dismissableStep.ManageVisibilityOf(tooltip);
        }
    }
}
