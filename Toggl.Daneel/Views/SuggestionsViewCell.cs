using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Plugins.Color;
using MvvmCross.Plugins.Visibility;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.Suggestions;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class SuggestionsViewCell : MvxTableViewCell
    {
        private const float NoProjectDistance = 24;
        private const float HasProjectDistance = 14;

        public static readonly NSString Key = new NSString(nameof(SuggestionsViewCell));
        public static readonly UINib Nib;

        static SuggestionsViewCell()
        {
            Nib = UINib.FromName(nameof(SuggestionsViewCell), NSBundle.MainBundle);
        }

        protected SuggestionsViewCell(IntPtr handle) 
            : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ContentView.BackgroundColor = UIColor.White;

            this.DelayBind(() =>
            {
                var colorConverter = new MvxRGBValueConverter();
                var visibilityConverter = new MvxVisibilityValueConverter();
                var timeSpanConverter = new TimeSpanToDurationValueConverter();
                var descriptionTopDistanceValueConverter = new BoolToConstantValueConverter<nfloat>(HasProjectDistance, NoProjectDistance);

                var bindingSet = this.CreateBindingSet<SuggestionsViewCell, Suggestion>();

                //Text
                bindingSet.Bind(ProjectLabel).To(vm => vm.ProjectName);
                bindingSet.Bind(DescriptionLabel).To(vm => vm.Description);

                //Color
                bindingSet.Bind(ProjectLabel)
                          .For(v => v.TextColor)
                          .To(vm => vm.ProjectColor)
                          .WithConversion(colorConverter);

                bindingSet.Bind(ProjectDotView)
                          .For(v => v.BackgroundColor)
                          .To(vm => vm.ProjectColor)
                          .WithConversion(colorConverter);

                //Visibility
                bindingSet.Bind(DescriptionTopDistanceConstraint)
                          .For(v => v.Constant)
                          .To(vm => vm.HasProject)
                          .WithConversion(descriptionTopDistanceValueConverter);

                bindingSet.Bind(ProjectLabel)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Bind(ProjectDotView)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Bind(TaskLabel)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Apply();
            });
        }
    }
}
