using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    public sealed class UserAgent
    {
        private readonly string version;
        private readonly string agentName;

        public UserAgent(string agentName, string version)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(version, nameof(version));
            Ensure.Argument.IsNotNullOrWhiteSpaceString(agentName, nameof(agentName));

            this.version = version;
            this.agentName = agentName;
        }

        public override string ToString() => $"{agentName}/{version}";
    }
}
