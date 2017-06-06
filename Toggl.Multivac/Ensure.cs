using System;

namespace Toggl.Multivac
{
    public static class Ensure
    {
        public static void ArgumentIsNotNull<T>(T value, string argumentName)
        {
            #pragma warning disable RECS0017 // Possible compare of value type with 'null'
            if (value != null) return;
            #pragma warning restore RECS0017 // Possible compare of value type with 'null'

            throw new ArgumentNullException(argumentName);
        }

        public static void ArgumentIsNotNullOrWhiteSpace(string value, string argumentName)
        {
            ArgumentIsNotNull(value, argumentName);

            if (!string.IsNullOrWhiteSpace(value)) return;

            throw new ArgumentException("String cannot be empty.", argumentName);
        }

        public static void UriIsAbsolute(Uri uri, string argumentName)
        {
            ArgumentIsNotNull(uri, argumentName);

            if (uri.IsAbsoluteUri) return;

            throw new ArgumentException("Uri must be absolute.", argumentName);
        }
    }
}
