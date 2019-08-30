using Android.Support.Constraints;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Activities
{
    public partial class ShareTimeEntryActivity
    {
        private ImageView closeButton;
        private View acceptButton;

        private Group projectGroup;
        private Group tagsGroup;
        private Group stopTimeGroup;
        private Group billableGroup;

        private TextView descriptionTextView;
        private TextView projectTextView;
        private TextView tagsTextView;
        private TextView startTimeTextView;
        private TextView startDateTextView;
        private TextView stopTimeTextView;
        private TextView stopDateTextView;
        private TextView billableTextView;

        private TextView missingProjectLabel;
        private TextView missingTagsLabel;

        private Switch projectSwitch;
        private Switch tagsSwitch;

        protected override void InitializeViews()
        {
            closeButton = FindViewById<ImageView>(Resource.Id.CloseButton);
            acceptButton = FindViewById(Resource.Id.AcceptButton);

            projectGroup = FindViewById<Group>(Resource.Id.ProjectGroup);
            tagsGroup = FindViewById<Group>(Resource.Id.TagsGroup);
            stopTimeGroup = FindViewById<Group>(Resource.Id.StopTimeGroup);
            billableGroup = FindViewById<Group>(Resource.Id.BillableGroup);

            projectSwitch = FindViewById<Switch>(Resource.Id.SwitchProject);
            tagsSwitch = FindViewById<Switch>(Resource.Id.SwitchTags);

            descriptionTextView = FindViewById<TextView>(Resource.Id.TextDescription);
            projectTextView = FindViewById<TextView>(Resource.Id.TextProject);
            tagsTextView = FindViewById<TextView>(Resource.Id.TextTags);
            startTimeTextView = FindViewById<TextView>(Resource.Id.TextStartTime);
            startDateTextView = FindViewById<TextView>(Resource.Id.TextStartDate);
            stopTimeTextView = FindViewById<TextView>(Resource.Id.TextStopTime);
            stopDateTextView = FindViewById<TextView>(Resource.Id.TextStopDate);
            billableTextView = FindViewById<TextView>(Resource.Id.TextBillable);

            missingProjectLabel = FindViewById<TextView>(Resource.Id.LabelMissingProject);
            missingTagsLabel = FindViewById<TextView>(Resource.Id.LabelMissingTags);
        }
    }
}
