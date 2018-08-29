namespace Toggl.Foundation
{
    public sealed class PlatformInfo
    {
        public Platform Platform { get; set; }
    }

    public enum Platform
    {
        Daneel,
        Giskard
    }
}
