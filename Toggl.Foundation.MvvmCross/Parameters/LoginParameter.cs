using System;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class LoginParameter
    {
        public enum LoginType
        {
            Login,
            SignUp
        }

        public static LoginParameter Login { get; } = new LoginParameter { Type = LoginType.Login };

        public static LoginParameter Signup { get; } = new LoginParameter { Type = LoginType.SignUp };

        public LoginType Type { get; set; }
    }
}
