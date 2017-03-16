using System;

namespace Voxteneo.Core.Extensions
{
    public static class DateTimeFormatExtension
    {
        public static string TimeAsString(this TimeSpan timeSpan)
        {

            return DateTime.Today.Add(timeSpan).ToString(Constants.DefaultFormatTimeString);
        }
        public static string DateAsString(this DateTime date)
        {

            return date.ToString(Constants.DefaultFormatDateString);
        }
    }
}
