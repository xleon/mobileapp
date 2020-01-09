using System;
using Android.App;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Math = System.Math;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportSummaryViewHolder : ReportElementViewHolder<ReportSummaryElement>
    {
        private static readonly ForegroundColorSpan totalTimeTextColorSpan = new ForegroundColorSpan(Application.Context.SafeGetColor(Resource.Color.totalTimeText));
        private static readonly ForegroundColorSpan disabledReportFeatureColorSpan = new ForegroundColorSpan(Application.Context.SafeGetColor(Resource.Color.disabledReportFeature));
        private static readonly ForegroundColorSpan billablePercentageTextColorSpan = new ForegroundColorSpan(Application.Context.SafeGetColor(Resource.Color.billablePercentageText));
        private static readonly TypefaceSpan sansSerifTypefaceSpan = new TypefaceSpan("sans-serif");
        private static readonly TypefaceSpan sansSerifLightTypefaceSpan = new TypefaceSpan("sans-serif-light");
        private static readonly string emDash = "—";
        private static readonly int secondsPartLength = 3;

        private TextView reportsSummaryTotal;
        private TextView reportsSummaryTotalLabel;
        private TextView reportsSummaryBillable;
        private TextView reportsSummaryBillableLabel;
        private readonly ISpannable emptyStateSpannable = new SpannableString(emDash);

        public ReportSummaryViewHolder(View itemView) : base(itemView)
        {
            emptyStateSpannable.SetSpan(disabledReportFeatureColorSpan, 0, emptyStateSpannable.Length(),
                SpanTypes.ExclusiveExclusive);
        }

        public ReportSummaryViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            reportsSummaryTotal = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryTotal);
            reportsSummaryTotalLabel = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryTotalLabel);
            reportsSummaryBillable = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryBillable);
            reportsSummaryBillableLabel = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryBillableLabel);

            reportsSummaryTotalLabel.Text = Resources.Total;
            reportsSummaryBillableLabel.Text = Resources.Billable;
        }

        protected override void UpdateView()
        {
            reportsSummaryTotal.TextFormatted = convertReportTimespanToDurationString();
            reportsSummaryBillable.TextFormatted = convertBillablePercentageToSpannable();
        }

        private ISpannable convertReportTimespanToDurationString()
        {
            if (!Item.TotalTime.HasValue || Item.IsLoading)
            {
                return emptyStateSpannable;
            }
            
            var timeString = Item.TotalTime.Value.ToFormattedString(Item.DurationFormat);
            var emphasizedPartLength = Item.DurationFormat == DurationFormat.Improved
                ? timeString.Length - secondsPartLength
                : timeString.Length;

            return selectTimespanEnabledStateSpan(timeString, emphasizedPartLength);
        }

        private ISpannable selectTimespanEnabledStateSpan(string timeString, int emphasizedPartLength)
        {
            var spannable = new SpannableString(timeString);

            spannable.SetSpan(sansSerifTypefaceSpan, 0, Math.Min(emphasizedPartLength, spannable.Length()), SpanTypes.InclusiveInclusive);
            if (emphasizedPartLength < timeString.Length)
            {
                spannable.SetSpan(sansSerifLightTypefaceSpan, emphasizedPartLength, timeString.Length,
                    SpanTypes.InclusiveInclusive);
            }

            spannable.SetSpan(totalTimeTextColorSpan, 0, spannable.Length(), SpanTypes.InclusiveInclusive);

            return spannable;
        }

        private ISpannable convertBillablePercentageToSpannable()
        {
            if (!Item.BillablePercentage.HasValue || Item.IsLoading)
            {
                return emptyStateSpannable;
            }
            
            return selectBillablePercentageEnabledStateSpan($"{Item.BillablePercentage.Value:0.00}%");
        }

        private ISpannable selectBillablePercentageEnabledStateSpan(string percentage)
        {
            var spannable = new SpannableString(percentage);
            spannable.SetSpan(billablePercentageTextColorSpan, 0, spannable.Length(), SpanTypes.ExclusiveExclusive);

            return spannable;
        }
    }
}