namespace Toggl.Shared
{
    public struct PushNotificationsToken
    {
        private readonly string token;

        public PushNotificationsToken(string token)
        {
            Ensure.Argument.IsNotNullOrEmpty(token, nameof(token));

            this.token = token;
        }

        public static explicit operator string(PushNotificationsToken pushNotificationsToken)
            => pushNotificationsToken.token;
    }
}
