using Toggl.Foundation.MvvmCross.Converters;

// Use this file to register overrides of BoolToConstantValueConverter.
// They are all single line classes, but an override is needed to avoid using the Fluent syntax.
namespace Toggl.Giskard.Converters
{
    public sealed class LogDescriptionTopOffsetValueConverter : BoolToConstantValueConverter<int>
    {
        public LogDescriptionTopOffsetValueConverter() : base(16, 27) { }
    }
}
