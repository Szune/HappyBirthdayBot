using System;

namespace BirthdayBot
{
    public static class DateTimeExtensions
    {
        public static DateTime ToTimeZone(this DateTime date, TimeZoneInfo timeZone)
        {
            return date + timeZone.BaseUtcOffset;
        }
    }
}