using System;

namespace BirthdayBot.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToTimeZone(this DateTime date, TimeZoneInfo timeZone)
        {
            return date + timeZone.BaseUtcOffset;
        }
    }
}