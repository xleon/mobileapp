using System;

namespace Toggl.Shared
{
    public struct Color : IEquatable<Color>
    {
        public byte Alpha { get; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }

        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Color(uint argb)
        {
            Alpha = (byte)((argb >> 24) & 255);
            Red = (byte)((argb >> 16) & 255);
            Green = (byte)((argb >> 8) & 255);
            Blue = (byte)(argb & 255);
        }

        /// <summary>
        /// Creates a Color from a hexadecimal string. Valid formats: aarrggbb, #aarrggbb, rrggbb, #rrggbb
        /// </summary>
        public Color(string hex) : this(hexStringToInt(hex))
        {
        }

        private static uint hexStringToInt(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return 0;

            hex = hex.TrimStart('#');

            var hexLength = hex.Length;

            if (hexLength == 6)
                return 0xFF000000 + Convert.ToUInt32(hex, 16);

            if (hexLength == 8)
                return Convert.ToUInt32(hex, 16);

            throw new ArgumentException("Invalid hex string was provided. Valid formats: aarrggbb, #aarrggbb, rrggbb, #rrggbb");
        }

        public override string ToString()
        {
            return $"{{a={Alpha}, r={Red}, g={Green}, b={Blue}}}";
        }

        public override int GetHashCode()
            => HashCode.From(Alpha, Red, Green, Blue);

        public static bool operator ==(Color color, Color otherColor)
            => color.Red == otherColor.Red
            && color.Green == otherColor.Green
            && color.Blue == otherColor.Blue
            && color.Alpha == otherColor.Alpha;

        public static bool operator !=(Color color, Color otherColor)
            => !(color == otherColor);

        public override bool Equals(object obj)
        {
            if (obj is Color color)
                return this == color;

            return false;
        }

        public bool Equals(Color other)
            => this == other;
    }

    public static class ColorExtensions
    {
        public static string ToHexString(this Color color)
            => $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
    }
}
