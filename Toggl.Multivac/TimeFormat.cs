using System;
using System.Collections.Generic;

namespace Toggl.Multivac
{
    public struct TimeFormat
    {
        private static readonly Dictionary<string, string> formatConversion = new Dictionary<string, string>
        {
            ["h:mm A"] = "hh:mm tt",
            ["H:mm"] = "H:mm"
        };

        /// <summary>
        /// Intended for displaying in the UI
        /// </summary>
        public string Localized { get; }

        /// <summary>
        /// Intended for  using in dateTime.ToString(useHere)
        /// </summary>
        public string Format { get; }

        private TimeFormat(string localized, string format)
        {
            Localized = localized;
            Format = format;
        }

        public static TimeFormat FromLocalizedTimeFormat(string timePattern)
        {
            if (formatConversion.ContainsKey(timePattern) == false)
                throw new ArgumentException($"Time pattern '{timePattern}' is not supported.");

            return new TimeFormat(timePattern, formatConversion[timePattern]);
        }
    }
}
