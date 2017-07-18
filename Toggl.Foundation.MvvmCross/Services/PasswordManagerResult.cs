namespace Toggl.Foundation.MvvmCross.Services
{
    public sealed class PasswordManagerResult
    {
        public string Email { get; }

        public string Password { get; }

        public PasswordManagerResult(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
