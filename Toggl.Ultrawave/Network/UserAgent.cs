using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    public sealed class UserAgent
    {
        private readonly string version;
        private readonly string agentName;

        public UserAgent(string agentName, string version)
        {
            Ensure.ArgumentIsNotNullOrWhiteSpace(version, nameof(version));
            Ensure.ArgumentIsNotNullOrWhiteSpace(agentName, nameof(agentName));

            this.version = version;
            this.agentName = agentName;
        }

        public override string ToString() => $"{agentName}/{version}";
    }
}
