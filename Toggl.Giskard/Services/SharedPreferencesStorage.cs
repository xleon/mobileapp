using Android.Content;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Giskard.Services
{
    public sealed class SharedPreferencesStorage : IKeyValueStorage
    {
        private readonly ISharedPreferences sharedPreferences;

        public SharedPreferencesStorage(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public bool GetBool(string key)
            => sharedPreferences.GetBoolean(key, false);

        public string GetString(string key)
            => sharedPreferences.GetString(key, null);

        public void SetBool(string key, bool value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutBoolean(key, value);
            editor.Commit();
        }

        public void SetString(string key, string value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutString(key, value);
            editor.Commit();
        }
    }
}
