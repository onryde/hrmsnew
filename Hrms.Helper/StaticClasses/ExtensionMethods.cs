using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hrms.Helper.StaticClasses
{
    public static class ExtensionMethods
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }
        public static string GetSizeAsString(this long size)
        {
            var isKb = size < 500000;
            if (isKb)
            {
                var isByte = size < 1024;
                if (isByte)
                {
                    return string.Concat(size, " Bytes");
                }
                return string.Concat(Math.Round((decimal)size / 1024, 3), " KB");
            }
            return string.Concat(Math.Round((decimal)size / 1024 * 1024, 3), " MB");
        }
        public static string GetSizeAsString(this double size)
        {
            var isKb = size < 500000;
            if (isKb)
            {
                var isByte = size < 1024;
                if (isByte)
                {
                    return string.Concat(size, " Bytes");
                }
                return string.Concat(Math.Round((decimal)size / 1024, 3), " KB");
            }
            return string.Concat(Math.Round((decimal)size / 1024 * 1024, 3), " MB");
        }
        public static string GetSizeAsString(this double size, string type)
        {
            switch (type)
            {
                case "Byte":
                    return string.Concat(size, " Bytes");

                case "Kb":
                    return string.Concat(Math.Round((decimal)size / 1024, 3), " KB");

                case "Mb":
                    return string.Concat(Math.Round((decimal)size / 1024 * 1024, 3), " MB");

                default:
                    return string.Concat(size, " Bytes");
            }
        }
        public static decimal GetSizeOnlyAsString(this double size, string type)
        {
            var newSize = (decimal)0.0;
            switch (type)
            {
                case "Byte":
                    newSize = (decimal)size;
                    break;

                case "Kb":
                    newSize = Math.Round((decimal)size / 1024, 3);
                    break;

                case "Mb":
                    newSize = Math.Round((decimal)size / (1024 * 1024), 3);
                    break;

                default:
                    newSize = (decimal)size;
                    break;
            }

            return newSize;
        }
        public static string GetSizeAsString(this int size)
        {
            var isKb = size < 500000;
            if (isKb)
            {
                var isByte = size < 1024;
                if (isByte)
                {
                    return string.Concat(size, " Bytes");
                }
                return string.Concat(Math.Round((decimal)size / 1024, 3), " KB");
            }
            return string.Concat(Math.Round((decimal)size / 1024 * 1024, 3), " MB");
        }

        public static DateTime ConvertToTimezone(this DateTime date, string timezone = null)
        {
            if (string.IsNullOrWhiteSpace(timezone))
            {
                return date;
            }
            else
            {
                TimeZoneInfo localTimezone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                DateTime convertedTime = TimeZoneInfo.ConvertTimeFromUtc(date, localTimezone);

                return convertedTime;
            }
        }

        public static string ToCustomDateTimeFormat(this DateTime date, string timezone = null)
        {
            return date.ConvertToTimezone(timezone).ToString("dd MMM yyyy hh:mm tt");
        }

        public static string ToCustomTimeFormat(this DateTime date, string timezone = null)
        {
            return date.ConvertToTimezone(timezone).ToString("hh:mm tt");
        }

        public static string ToCustomDateFormat(this DateTime date, string timezone = null)
        {
            return date.ConvertToTimezone(timezone).ToString("dd MMM yyyy ");
        }

        public static string ToCustomDateTimeFormat(this DateTime? date, string timezone = null)
        {
            return date.HasValue ? date.Value.ConvertToTimezone(timezone).ToString("dd MMM yyyy hh:mm tt") : string.Empty;
        }

        public static string ToCustomTimeFormat(this DateTime? date, string timezone = null)
        {
            return date.HasValue ? date.Value.ConvertToTimezone(timezone).ToString("hh:mm tt") : string.Empty;
        }

        public static string ToCustomDateFormat(this DateTime? date, string timezone = null)
        {
            return date.HasValue ? date.Value.ConvertToTimezone(timezone).ToString("dd MMM yyyy ") : string.Empty;
        }

        public static bool IsBoolean(this Type type)
        {
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            return t == typeof(bool);
        }

        public static bool IsTrueEnum(this Type type)
        {
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            return t.IsEnum;
        }

        public static DateTime ConvertToTimezone(this DateTime date, int timeDiff = 0)
        {
            if (timeDiff == 0)
            {
                return date;
            }
            else
            {
                var convertedTime = date.AddMinutes(timeDiff);
                return convertedTime;
            }
        }
        public static string ToCustomDateTimeFormat(this DateTime date, int timeDiff = 0)
        {
            return date.ConvertToTimezone(-timeDiff).ToString("dd MMM yyyy hh:mm tt");
        }
        public static string ToCustomDateTimeFormat(this DateTime? date, int timeDiff = 0)
        {
            return date.HasValue ? date.Value.ConvertToTimezone(-timeDiff).ToString("dd MMM yyyy hh:mm tt") : string.Empty;
        }
        public static string ToCustomDateFormat(this DateTime date, int timeDiff)
        {
            return date.ConvertToTimezone(-timeDiff).ToString("dd MMM yyyy");
        }
        public static string ToCustomDateFormat(this DateTime? date, int timeDiff)
        {
            return date.HasValue ? date.Value.ConvertToTimezone(-timeDiff).ToString("dd MMM yyyy") : string.Empty;
        }

        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        public static DateTime Previous(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target >= start)
                target -= 7;
            return from.AddDays(target - start);
        }

        public static int GetAge(this DateTime date)
        {
            var age = DateTime.Today.Year - date.Year;
            if (date.Date > DateTime.Today.AddYears(-age)) age--;

            return age;
        }

        public static bool BirthdayImminent(this DateTime birthDate, DateTime referenceDate, int days)
        {
            DateTime birthdayThisYear = birthDate.AddYears(referenceDate.Year - birthDate.Year);

            if (birthdayThisYear < referenceDate)
                birthdayThisYear = birthdayThisYear.AddYears(1);

            bool birthdayImminent = (birthdayThisYear - referenceDate).TotalDays <= days;

            return birthdayImminent;
        }

        public static bool Birthdayday(this DateTime birthDate, DateTime referenceDate)
        {
            DateTime birthdayThisYear = birthDate.AddYears(referenceDate.Year - birthDate.Year);
            DateTime refdate = new DateTime(referenceDate.Year, referenceDate.Month, 01);
            int days = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);

            if (birthdayThisYear < refdate)
                birthdayThisYear = birthdayThisYear.AddYears(1);

            bool birthdayImminent = (birthdayThisYear - refdate).TotalDays < days;

            return birthdayImminent;
        }


        public static string GenerateSlug(this string phrase)
        {
            var s = phrase.ToLower();
            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
            s = Regex.Replace(s, @"\s+", " ").Trim();
            s = s.Substring(0, s.Length <= 45 ? s.Length : 45).Trim();
            s = Regex.Replace(s, @"\s", "-");
            return s.ToLower();
        }
        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(txt);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
