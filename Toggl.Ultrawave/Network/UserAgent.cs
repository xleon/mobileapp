using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    public sealed class UserAgent
    {
        private readonly string userAgentString;

        public UserAgent(string agentName, string version)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(version, nameof(version));
            Ensure.Argument.IsNotNullOrWhiteSpaceString(agentName, nameof(agentName));

            userAgentString = $"{agentName}/{version}";
        }

        public override string ToString() => userAgentString;
    }
}
