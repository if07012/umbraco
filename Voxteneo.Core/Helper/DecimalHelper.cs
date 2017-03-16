using System.Globalization;

namespace Voxteneo.Core.Helper
{
    public static class DecimalHelper
    {
        public static double? GetDoubleFromString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            double tmp;

            if (!double.TryParse(input, out tmp))
            {
                return null;
            }

            return tmp;
        }

        public static string ToDecimalToStringFormat(this decimal input)
        {
            return ToDecimalToStringFormat(input, "");
        }

        public static string ToDecimalToStringFormat(this decimal input, string format = "")
        {
            return input.ToString(CultureInfo.InvariantCulture).Replace(Constants.DotValue, Constants.CommaValue);
        }

        public static string ToDecimalToStringFormat(this decimal? input)
        {
            if (!input.HasValue) return string.Empty;
            return ToDecimalToStringFormat(input.Value, "");
        }

        public static string ToDecimalToStringFormat(this decimal? input, string format = "")
        {
            if (!input.HasValue) return string.Empty;
            return ToDecimalToStringFormat(input.Value, format);
        }
    }
}
