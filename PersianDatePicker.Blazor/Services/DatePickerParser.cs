using System.Globalization;
using System.Text.RegularExpressions;
using PersianDatePicker.Blazor.Text;

namespace PersianDatePicker.Blazor.Services;

/// <summary>
/// Parses a display string (as the user typed it or as a bound value) back into a
/// <see cref="JDate"/>. It auto-detects the layout so the same call handles the
/// picker's own emitted formats in both locales, accepts Persian or ASCII digits, and
/// understands Jalali month names.
/// </summary>
public static class DatePickerParser
{
    private static readonly Regex Numbers = new(@"\d+", RegexOptions.Compiled);
    private static readonly Regex Time = new(@"(\d{1,2}):(\d{2})(?::(\d{2}))?", RegexOptions.Compiled);

    private static readonly string[] FaMonth =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };
    private static readonly string[] EnMonthLong =
    {
        "Farvardin", "Ordibehesht", "Khordad", "Tir", "Mordad", "Shahrivar",
        "Mehr", "Aban", "Azar", "Dey", "Bahman", "Esfand"
    };
    private static readonly string[] EnMonthShort =
    {
        "Far", "Ord", "Kho", "Tir", "Mor", "Sha", "Meh", "Aba", "Az", "Dey", "Bah", "Esf"
    };

    public static bool TryParse(string? input, out JDate result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var digits = Localizer.PersianToAscii(input.Trim());

        int? month = null;
        int? year = null;
        int? day = null;

        var monthNameIndex = FindMonthName(digits);
        var hasMonthName = monthNameIndex >= 0;
        if (hasMonthName)
        {
            month = monthNameIndex;
            // Remove the month name so only the year number remains for the date part.
            digits = Regex.Replace(digits, FaMonth[monthNameIndex - 1] ?? "", " ");
        }

        // Extract the time component (HH:mm[:ss]) if present.
        var timeMatch = Time.Match(digits);
        int hour = 0, minute = 0, second = 0;
        var hasTime = timeMatch.Success;
        if (hasTime)
        {
            hour = int.Parse(timeMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            minute = int.Parse(timeMatch.Groups[2].Value, CultureInfo.InvariantCulture);
            second = timeMatch.Groups[3].Success ? int.Parse(timeMatch.Groups[3].Value, CultureInfo.InvariantCulture) : 0;
            digits = digits.Replace(timeMatch.Value, " ");
        }

        var ints = Numbers.Matches(digits).Select(m => int.Parse(m.Value, CultureInfo.InvariantCulture)).ToList();

        if (hasMonthName)
        {
            // A month name carries the month; the remaining numbers are the day and year.
            var yearNum = ints.Find(v => v.ToString().Length >= 3);
            year = yearNum >= 0 ? yearNum : (ints.Count > 0 ? ints.Last() : 1);
            var dayNum = ints.Find(v => v.ToString().Length < 3);
            day = dayNum >= 0 ? dayNum : 1;
        }
        else if (ints.Count >= 3)
        {
            AssignYMD(ints.Take(3).ToList(), out year, out month, out day);
        }
        else if (ints.Count == 2)
        {
            // Two numbers: a date like 1405/04 (Y-M) or 04-1405 (M-Y).
            if (ints[0].ToString().Length >= 3)
            {
                year = ints[0]; month = ints[1]; day = 1;
            }
            else
            {
                month = ints[0]; year = ints[1]; day = 1;
            }
        }
        else if (ints.Count == 1)
        {
            year = ints[0]; month = 1; day = 1;
        }
        else
        {
            return false;
        }

        if (month is null || year is null || day is null)
        {
            // No usable date; a lone time value is still meaningful for time mode.
            if (hasTime)
            {
                result = JDate.Today().WithTime(hour, minute, second);
                return true;
            }
            return false;
        }

        try
        {
            result = new JDate(JDate.Persian.ToDateTime(year.Value, month.Value, day.Value, hour, minute, second, 0));
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        return true;
    }

    private static void AssignYMD(List<int> ints, out int? year, out int? month, out int? day)
    {
        // The year is the 3-4 digit value. Position of the year determines the layout.
        var yearIndex = ints.FindIndex(v => v.ToString().Length >= 3);
        if (yearIndex == 0)
        {
            // Year first: Y - M - D
            year = ints[0]; month = ints[1]; day = ints[2];
        }
        else if (yearIndex == 2)
        {
            // Year last: D - M - Y
            day = ints[0]; month = ints[1]; year = ints[2];
        }
        else
        {
            // Year in the middle: M - Y - D
            month = ints[0]; year = ints[1]; day = ints[2];
        }
    }

    private static int? FindYear(List<int> ints)
    {
        var y = ints.Find(v => v.ToString().Length >= 3);
        return ints.Count == 0 ? null : (y >= 0 ? y : ints.Last());
    }

    /// <summary>Returns the 1-based Jalali month index if a Jalali month name is present, else -1.</summary>
    private static int FindMonthName(string text)
    {
        var lower = text.ToLowerInvariant();
        for (var i = 0; i < FaMonth.Length; i++)
        {
            if (lower.Contains(FaMonth[i].ToLowerInvariant())) return i + 1;
        }
        for (var i = 0; i < EnMonthLong.Length; i++)
        {
            if (lower.Contains(EnMonthLong[i].ToLowerInvariant())) return i + 1;
        }
        for (var i = 0; i < EnMonthShort.Length; i++)
        {
            // Short names risk false positives, so require a word boundary.
            var match = Regex.Match(lower, $@"\b{Regex.Escape(EnMonthShort[i].ToLowerInvariant())}\b");
            if (match.Success) return i + 1;
        }
        return -1;
    }
}
