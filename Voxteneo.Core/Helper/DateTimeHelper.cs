using System;

namespace Voxteneo.Core.Helper
{
    public class DateTimeHelper
    {
        public static string ConvertDateToString(DateTime dateTime)
        {
            return dateTime.ToString(Constants.DefaultFormatDateString);
        }
        public static string ConvertDateToString(DateTime? dateTime)
        {
            return !dateTime.HasValue ? "" : ConvertDateToString(dateTime.Value);
        }
        public static string ConvertTimeToString(TimeSpan timespan)
        {
            return DateTime.Today.Add(timespan).ToString(Constants.DefaultFormatTimeString);
        }
        public static string ConvertTimeToString(TimeSpan? timespan)
        {
            return !timespan.HasValue ? "" : ConvertTimeToString(timespan.Value);
        }
    }
}
