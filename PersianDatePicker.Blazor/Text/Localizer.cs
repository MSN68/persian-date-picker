using System.Globalization;
using System.Text;

namespace PersianDatePicker.Blazor.Text;

/// <summary>
/// Localization: Jalali month/weekday names, Persian digits and moment-style format
/// tokens. The <see cref="DatePickerLocale"/> selects the whole set of labels, so a
/// single call site (<c>Localizer.Format</c>) renders the same <see cref="JDate"/>
/// correctly in either Persian or English.
/// </summary>
public static class Localizer
{
    // Jalali month names, indexed 1..12.
    private static readonly string[] FaMonthLong =
    {
        "", "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

    private static readonly string[] EnMonthLong =
    {
        "", "Farvardin", "Ordibehesht", "Khordad", "Tir", "Mordad", "Shahrivar",
        "Mehr", "Aban", "Azar", "Dey", "Bahman", "Esfand"
    };

    private static readonly string[] EnMonthShort =
    {
        "", "Far", "Ord", "Kho", "Tir", "Mor", "Sha", "Meh", "Aba", "Az", "Dey", "Bah", "Esf"
    };

    // Weekday names. The Jalali week starts on Saturday, so the Persian arrays are
    // indexed by JDate.DayIndex (0 = Saturday .. 6 = Friday). The English arrays are
    // indexed by .NET DayOfWeek (0 = Sunday .. 6 = Saturday).
    private static readonly string[] FaWeekdayLong =
        { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
    private static readonly string[] FaWeekdayShort =
        { "ش", "ی", "د", "س", "چ", "پ", "ج" };

    private static readonly string[] EnWeekdayLong =
        { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    private static readonly string[] EnWeekdayShort =
        { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

    public static bool IsPersian(DatePickerLocale locale) => locale == DatePickerLocale.Persian;

    public static string MonthLong(JDate d, DatePickerLocale locale) =>
        IsPersian(locale) ? FaMonthLong[d.Month] : EnMonthLong[d.Month];

    public static string MonthShort(JDate d, DatePickerLocale locale) =>
        IsPersian(locale) ? FaMonthLong[d.Month] : EnMonthShort[d.Month];

    public static string WeekdayLong(JDate d, DatePickerLocale locale) =>
        WeekdayLong(d.NetDayOfWeek, locale);

    public static string WeekdayShort(JDate d, DatePickerLocale locale) =>
        WeekdayShort(d.NetDayOfWeek, locale);

    /// <summary>Weekday name for a raw .NET day of week (0 = Sunday .. 6 = Saturday).</summary>
    public static string WeekdayLong(DayOfWeek dow, DatePickerLocale locale)
    {
        var persianIndex = dow.ToPersianIndex();
        return IsPersian(locale) ? FaWeekdayLong[persianIndex] : EnWeekdayLong[(int)dow];
    }

    /// <summary>Short weekday name for a raw .NET day of week (0 = Sunday .. 6 = Saturday).</summary>
    public static string WeekdayShort(DayOfWeek dow, DatePickerLocale locale)
    {
        var persianIndex = dow.ToPersianIndex();
        return IsPersian(locale) ? FaWeekdayShort[persianIndex] : EnWeekdayShort[(int)dow];
    }

    public static string Meridiem(JDate d, DatePickerLocale locale)
    {
        var pm = d.Hour >= 12;
        return IsPersian(locale) ? (pm ? "پ.ذ" : "ب.ذ") : (pm ? "PM" : "AM");
    }

    /// <summary>Converts ASCII digits in <paramref name="value"/> to Persian digits.</summary>
    public static string ToPersianDigits(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            sb.Append(c >= '0' && c <= '9' ? (char)('۰' + (c - '0')) : c);
        }
        return sb.ToString();
    }

    /// <summary>Converts Persian digits in <paramref name="value"/> to ASCII digits.</summary>
    public static string PersianToAscii(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c >= '۰' && c <= '۹')
            {
                sb.Append((char)('0' + (c - '۰')));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Formats a <see cref="JDate"/> with a moment-style token string. Supported tokens:
    /// <c>YYYY/YY</c> year, <c>MMMM/MMM/MM/M</c> month, <c>DD/D</c> day, <c>dddd/ddd</c> weekday,
    /// <c>HH/H</c> 24-hour, <c>hh/h</c> 12-hour, <c>mm/m</c> minute, <c>ss/s</c> second,
    /// <c>A/a</c> meridiem. A leading <c>j</c> on a token (e.g. <c>jYYYY</c>) is accepted and
    /// ignored, to stay compatible with jalali-moment format strings.
    /// </summary>
    public static string Format(JDate d, string format, DatePickerLocale locale)
    {
        if (string.IsNullOrEmpty(format)) return "";
        var sb = new StringBuilder(format.Length + 16);
        var i = 0;
        while (i < format.Length)
        {
            var c = format[i];
            if (c == '\'')
            {
                // Moment escapes literal text in single quotes.
                i++;
                while (i < format.Length && format[i] != '\'')
                {
                    sb.Append(format[i]);
                    i++;
                }
                if (i < format.Length) i++; // closing quote
                continue;
            }

            if (c == 'j' && i + 1 < format.Length)
            {
                i++; // jalali prefix, fall through to the token
                c = format[i];
            }

            // Read a run of the same letter.
            var start = i;
            while (i < format.Length && format[i] == c) i++;
            var token = format.AsSpan(start, i - start).ToString();

            switch (c)
            {
                case 'Y':
                    sb.Append(token.Length == 2
                        ? (d.Year % 100).ToString("00", CultureInfo.InvariantCulture)
                        : d.Year.ToString(CultureInfo.InvariantCulture));
                    break;
                case 'M':
                    sb.Append(FormatMonth(d, locale, token.Length));
                    break;
                case 'D':
                    sb.Append(token.Length == 2 ? d.Day.ToString("00", CultureInfo.InvariantCulture) : d.Day.ToString(CultureInfo.InvariantCulture));
                    break;
                case 'd':
                    sb.Append(token.Length >= 4 ? WeekdayLong(d, locale) : WeekdayShort(d, locale));
                    break;
                case 'H':
                    sb.Append(token.Length == 2 ? d.Hour.ToString("00", CultureInfo.InvariantCulture) : d.Hour.ToString(CultureInfo.InvariantCulture));
                    break;
                case 'h':
                    var h12 = d.Hour % 12; if (h12 == 0) h12 = 12;
                    sb.Append(token.Length == 2 ? h12.ToString("00", CultureInfo.InvariantCulture) : h12.ToString(CultureInfo.InvariantCulture));
                    break;
                case 'm':
                    sb.Append(token.Length == 2 ? d.Minute.ToString("00", CultureInfo.InvariantCulture) : d.Minute.ToString(CultureInfo.InvariantCulture));
                    break;
                case 's':
                    sb.Append(token.Length == 2 ? d.Second.ToString("00", CultureInfo.InvariantCulture) : d.Second.ToString(CultureInfo.InvariantCulture));
                    break;
                case 'A':
                    sb.Append(Meridiem(d, locale));
                    break;
                case 'a':
                    sb.Append(Meridiem(d, locale).ToLowerInvariant());
                    break;
                default:
                    // Anything else (separators, stray letters) is literal.
                    sb.Append(token);
                    break;
            }
        }

        var result = sb.ToString();
        return IsPersian(locale) ? ToPersianDigits(result) : result;
    }

    private static string FormatMonth(JDate d, DatePickerLocale locale, int tokenLength)
    {
        return tokenLength switch
        {
            4 => MonthLong(d, locale),
            3 => MonthShort(d, locale),
            2 => d.Month.ToString("00", CultureInfo.InvariantCulture),
            _ => d.Month.ToString(CultureInfo.InvariantCulture)
        };
    }

}
