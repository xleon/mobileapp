namespace Toggl.Multivac
{
    public struct DateFormat
    {
        /// <summary>
        /// Intended for displaying in the UI
        /// </summary>
        public string Localized { get; }

        /// <summary>
        /// Intended for  using in dateTime.ToString(useHere)
        /// </summary>
        public string Long { get; }

        /// <summary>
        /// Same as DateFormat.Long, but without the year portion
        /// </summary>
        public string Short { get; }

        private DateFormat(
            string longDateFormat,
            string shortDateFormat,
            string localizedDateFormat)
        {
            Ensure.Argument.IsNotNull(longDateFormat, nameof(longDateFormat));
            Ensure.Argument.IsNotNull(shortDateFormat, nameof(shortDateFormat));
            Ensure.Argument.IsNotNull(localizedDateFormat, nameof(localizedDateFormat));

            Long = longDateFormat;
            Short = shortDateFormat;
            Localized = localizedDateFormat;
        }

        public static DateFormat FromLocalizedDateFormat(string localizedDateFormat)
        {
            var longDateFormat = localizedDateFormat
                .Replace('Y', 'y')
                .Replace('D', 'd');

            var shortDateFormat = longDateFormat
                .Replace("y", "")
                .Trim('.', '-', '/');

            return new DateFormat(
                longDateFormat,
                shortDateFormat,
                localizedDateFormat);
        }
    }
}
