using System.Globalization;

namespace PersianDatePicker.Blazor;

/// <summary>
/// A single Jalali (Shamsi) date with a time component, backed by a .NET <see cref="DateTime"/>.
/// This is the "moment" of the picker: every calendar cell, selection and bound value is a
/// <see cref="JDate"/> (or a list of them). It is intentionally a tiny value type, not a full
/// date library — it only does what the calendar needs: Jalali arithmetic, comparison and
/// localization.
/// </summary>
public readonly struct JDate : IComparable<JDate>
{
    public static readonly PersianCalendar Persian = new();

    private readonly DateTime _dt;

    public JDate(DateTime dt)
    {
        _dt = dt;
    }

    /// <summary>The underlying UTC-independent .NET representation.</summary>
    public DateTime DateTime => _dt;

    public int Year => Persian.GetYear(_dt);
    public int Month => Persian.GetMonth(_dt);
    public int Day => Persian.GetDayOfMonth(_dt);
    /// <summary>Jalali day-of-week index, 0 (Saturday) through 6 (Friday) — see <see cref="DayIndex"/>.</summary>
    public int DayIndex => Persian.GetDayOfWeek(_dt).ToPersianIndex();

    /// <summary>The raw .NET day of week (0 = Sunday .. 6 = Saturday).</summary>
    public DayOfWeek NetDayOfWeek => _dt.DayOfWeek;

    public int Hour => _dt.Hour;
    public int Minute => _dt.Minute;
    public int Second => _dt.Second;

    public bool IsValid => true;

    public static JDate Now() => new(DateTime.Now);
    public static JDate Today() => new(DateTime.Today);

    public JDate Clone() => this;

    public JDate StartOfDay() => new(DateTime.Today);

    public JDate StartOfMonth()
    {
        var dt = Persian.ToDateTime(Year, Month, 1, 0, 0, 0, 0);
        return new JDate(dt);
    }

    public JDate StartOfYear()
    {
        var dt = Persian.ToDateTime(Year, 1, 1, 0, 0, 0, 0);
        return new JDate(dt);
    }

    public JDate AddDays(int days) => new(_dt.AddDays(days));
    public JDate AddMonths(int months) => new(_dt.AddMonths(months));
    public JDate AddYears(int years) => new(_dt.AddYears(years));

    public JDate AddTime(int hour, int minute, int second) =>
        new(DateTime.Today.AddHours(hour).AddMinutes(minute).AddSeconds(second));

    public JDate WithDay(int day)
    {
        var daysInMonth = Persian.GetDaysInMonth(Year, Month);
        day = Math.Clamp(day, 1, daysInMonth);
        return new(Persian.ToDateTime(Year, Month, day, 0, 0, 0, 0));
    }

    public JDate WithMonth(int month)
    {
        month = Math.Clamp(month, 1, 12);
        return new(Persian.ToDateTime(Year, month, 1, 0, 0, 0, 0));
    }

    public JDate WithYear(int year) => new(Persian.ToDateTime(year, 1, 1, 0, 0, 0, 0));

    public JDate WithTime(int hour, int minute, int second) =>
        new(new DateTime(_dt.Year, _dt.Month, _dt.Day,
            Math.Clamp(hour, 0, 23), Math.Clamp(minute, 0, 59), Math.Clamp(second, 0, 59)));

    public bool IsSame(JDate other, JGranularity unit = JGranularity.Day)
    {
        return unit switch
        {
            JGranularity.Year => Year == other.Year,
            JGranularity.Month => Year == other.Year && Month == other.Month,
            JGranularity.Day => Year == other.Year && Month == other.Month && Day == other.Day,
            JGranularity.Second => _dt == other._dt,
            _ => _dt == other._dt
        };
    }

    public bool IsBefore(JDate other, JGranularity unit = JGranularity.Day) =>
        CompareKeys(Key(unit), other.Key(unit)) < 0;

    public bool IsAfter(JDate other, JGranularity unit = JGranularity.Day) =>
        CompareKeys(Key(unit), other.Key(unit)) > 0;

    public bool IsBefore(JDate? other) => other is { } o && IsBefore(o);
    public bool IsAfter(JDate? other) => other is { } o && IsAfter(o);

    public bool IsSameOrAfter(JDate other, JGranularity unit = JGranularity.Day) =>
        CompareKeys(Key(unit), other.Key(unit)) >= 0;

    public bool IsSameOrBefore(JDate other, JGranularity unit = JGranularity.Day) =>
        CompareKeys(Key(unit), other.Key(unit)) <= 0;

    public bool IsBetween(JDate from, JDate to, JGranularity unit = JGranularity.Day)
    {
        var key = Key(unit);
        return CompareKeys(key, from.Key(unit)) >= 0 && CompareKeys(key, to.Key(unit)) <= 0;
    }

    private static int CompareKeys((int, int, int) a, (int, int, int) b)
    {
        var c = a.Item1.CompareTo(b.Item1);
        if (c != 0) return c;
        c = a.Item2.CompareTo(b.Item2);
        if (c != 0) return c;
        return a.Item3.CompareTo(b.Item3);
    }

    private (int, int, int) Key(JGranularity unit)
    {
        return unit switch
        {
            JGranularity.Year => (Year, 0, 0),
            JGranularity.Month => (Year, Month, 0),
            JGranularity.Day => (Year, Month, Day),
            _ => (Year, Month, Day * 86400 + Hour * 3600 + Minute * 60 + Second)
        };
    }

    /// <summary>ISO-8601 week number (Gregorian), matching moment's <c>isoWeek()</c>.</summary>
    public int IsoWeek
    {
        get
        {
            // The ISO week number is determined by the week's Thursday:
            // week = (Thursday's day-of-year - 1) / 7 + 1.
            var thursday = _dt.AddDays(3 - (int)_dt.DayOfWeek);
            return (thursday.DayOfYear - 1) / 7 + 1;
        }
    }

    public int CompareTo(JDate other) => _dt.CompareTo(other._dt);

    public static bool operator ==(JDate left, JDate right) => left._dt == right._dt;
    public static bool operator !=(JDate left, JDate right) => left._dt != right._dt;

    public static bool operator <(JDate left, JDate right) => left._dt < right._dt;
    public static bool operator >(JDate left, JDate right) => left._dt > right._dt;
    public static bool operator <=(JDate left, JDate right) => left._dt <= right._dt;
    public static bool operator >=(JDate left, JDate right) => left._dt >= right._dt;

    public bool Equals(JDate other) => _dt == other._dt;
    public override bool Equals(object? obj) => obj is JDate j && Equals(j);
    public override int GetHashCode() => _dt.GetHashCode();

    public override string ToString() => $"{Year:0000}/{Month:00}/{Day:00}";
}
