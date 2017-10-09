using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Color;
using MvvmCross.Plugins.Color.iOS;
using MvvmCross.Plugins.Visibility;
using Toggl.Daneel.Combiners;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntriesLogViewCell : MvxTableViewCell
    {
        private const float noProjectDistance = 24;
        private const float hasProjectDistance = 14;

        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogViewCell));
        public static readonly UINib Nib;

        public IMvxAsyncCommand<TimeEntryViewModel> ContinueTimeEntryCommand { get; set; }

        static TimeEntriesLogViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogViewCell), NSBundle.MainBundle);
        }

        protected TimeEntriesLogViewCell(IntPtr handle) 
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            FadeView.Layer.AddSublayer(new CAGradientLayer
            {
                Colors = new[] { UIColor.White.ColorWithAlpha(0.0f).CGColor, UIColor.White.CGColor },
                Locations = new[] { new NSNumber(0.0f), new NSNumber(0.2f) },
                StartPoint = new CGPoint(0.0, 0.5),
                EndPoint = new CGPoint(1.0, 0.5),
                Frame = FadeView.Bounds
            });

            TimeLabel.Font = TimeLabel.Font.GetMonospacedDigitFont();
            ContinueButton.TouchUpInside += onContinueButtonTap;

            this.DelayBind(() =>
            {
                var colorConverter = new MvxRGBValueConverter();
                var visibilityConverter = new MvxVisibilityValueConverter();
                var timeSpanConverter = new TimeSpanToDurationValueConverter();
                var invertedVisibilityConverter = new MvxInvertedVisibilityValueConverter();
                var projectTaskClientCombiner = new ProjectTaskClientValueCombiner(
                    ProjectTaskClientLabel.Font.CapHeight,
                    Color.TimeEntriesLog.ClientColor.ToNativeColor(),
                    true
                );
                var descriptionTopDistanceValueConverter = 
                    new BoolToConstantValueConverter<nfloat>(hasProjectDistance, noProjectDistance);
                
                var bindingSet = this.CreateBindingSet<TimeEntriesLogViewCell, TimeEntryViewModel>();

                //Text
                bindingSet.Bind(DescriptionLabel).To(vm => vm.Description);
                bindingSet.Bind(ProjectTaskClientLabel)
                          .For(v => v.AttributedText)
                          .ByCombining(projectTaskClientCombiner, 
                              v => v.ProjectName,
                              v => v.TaskName,
                              v => v.ClientName,
                              v => v.ProjectColor);

                bindingSet.Bind(TimeLabel)
                          .To(vm => vm.Duration)
                          .WithConversion(timeSpanConverter);

                //Visibility
                bindingSet.Bind(DescriptionTopDistanceConstraint)
                          .For(v => v.Constant)
                          .To(vm => vm.HasProject)
                          .WithConversion(descriptionTopDistanceValueConverter);

                bindingSet.Bind(ProjectTaskClientLabel)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Bind(AddDescriptionLabel)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasDescription)
                          .WithConversion(invertedVisibilityConverter);

                bindingSet.Bind(AddDescriptionTopDistanceConstraint)
                          .For(v => v.Constant)
                          .To(vm => vm.HasProject)
                          .WithConversion(descriptionTopDistanceValueConverter);

                bindingSet.Bind(SyncErrorImageView)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.CanSync)
                          .WithConversion(invertedVisibilityConverter);

                bindingSet.Bind(UnsyncedImageView)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.NeedsSync)
                          .WithConversion(visibilityConverter);

                bindingSet.Bind(ContinueButton)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.CanSync)
                          .WithConversion(visibilityConverter);

                bindingSet.Bind(ContinueImageView)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.CanSync)
                          .WithConversion(visibilityConverter);
                
                bindingSet.Apply();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            ContinueButton.TouchUpInside -= onContinueButtonTap;
        }

        private async void onContinueButtonTap(object sender, EventArgs e)
            => await ContinueTimeEntryCommand.ExecuteAsync((TimeEntryViewModel)DataContext);
    }
}
