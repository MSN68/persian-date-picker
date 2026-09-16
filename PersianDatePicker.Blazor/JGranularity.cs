namespace PersianDatePicker.Blazor;

/// <summary>Comparison granularity for <see cref="JDate"/>.</summary>
public enum JGranularity
{
    Second,
    Day,
    Month,
    Year
}

internal static class DayOfWeekExtensions
{
    /// <summary>Maps .NET <see cref="DayOfWeek"/> (0=Sunday..6=Saturday) to the Jalali
    /// day index used by this library (0=Saturday..6=Friday).</summary>
    public static int ToPersianIndex(this DayOfWeek dow) => (dow.DayIndex() + 1) % 7;

    private static int DayIndex(this DayOfWeek dow) => (int)dow;
}
