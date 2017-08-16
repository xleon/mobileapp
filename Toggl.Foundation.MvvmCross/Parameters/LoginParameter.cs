using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    [Preserve(AllMembers = true)]
    public sealed class LoginParameter
    {
        public enum LoginType
        {
            Login,
            SignUp
        }

        public static LoginParameter Login { get; } = new LoginParameter { Type = LoginType.Login };

        public static LoginParameter SignUp { get; } = new LoginParameter { Type = LoginType.SignUp };

        public LoginType Type { get; set; }
    }
}
