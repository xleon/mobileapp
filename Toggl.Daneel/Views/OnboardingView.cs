using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Plugins.Color;
using MvvmCross.Plugins.Visibility;
using Toggl.Daneel.Extensions;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Daneel.Views
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    public sealed partial class OnboardingView : MvxViewController<OnboardingViewModel>
    {
        public OnboardingView() 
            : base("OnboardingView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PageControl.Pages = ViewModel.NumberOfPages;
            FirstPageLabel.Text = Resources.OnboardingTrackPageCopy;
            SecondPageLabel.Text = Resources.OnboardingLogPageCopy;
            ThirdPageLabel.Text = Resources.OnboardingSummaryPageCopy;

            var visibilityConverter = new MvxVisibilityValueConverter();
            var invertedVisibilityConverter = new MvxInvertedVisibilityValueConverter();
            var colorConverter = new MvxNativeColorValueConverter();
            var bindingSet = this.CreateBindingSet<OnboardingView, OnboardingViewModel>();

            //Commands
            bindingSet.Bind(Skip).To(vm => vm.SkipCommand);
            bindingSet.Bind(Next).To(vm => vm.NextCommand);
            bindingSet.Bind(Login).To(vm => vm.LoginCommand);
            bindingSet.Bind(SignUp).To(vm => vm.SignUpCommand);
            bindingSet.Bind(Previous).To(vm => vm.PreviousCommand);

            //Color
            bindingSet.Bind(View)
                      .For(v => v.BindAnimatedBackground())
                      .To(vm => vm.BackgroundColor)
                      .WithConversion(colorConverter);
            
            bindingSet.Bind(PhoneFrame)
                      .For(v => v.BindAnimatedBackground())
                      .To(vm => vm.BorderColor)
                      .WithConversion(colorConverter);

            //Visibility
            bindingSet.Bind(PhoneContents)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsLastPage)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Bind(LastPageItems)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsLastPage)
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(Next)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsLastPage)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Bind(ScrollView)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsLastPage)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Bind(Skip)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsLastPage)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Bind(Previous)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsFirstPage)
                      .WithConversion(invertedVisibilityConverter);

            //Current Page
            bindingSet.Bind(ScrollView)
                      .For(v => v.BindCurrentPage())
                      .To(vm => vm.CurrentPage);

            bindingSet.Bind(PageControl)
                      .For(v => v.CurrentPage)
                      .To(vm => vm.CurrentPage);

            bindingSet.Apply();
        }
    }
}
