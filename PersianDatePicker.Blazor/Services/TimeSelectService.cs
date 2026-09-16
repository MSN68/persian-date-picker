using System.Globalization;
using PersianDatePicker.Blazor.Text;

namespace PersianDatePicker.Blazor.Services;

/// <summary>
/// Time-panel logic ported from the Angular <c>TimeSelectService</c>: the displayed
/// hour/minute/second/meridiem parts and the min/max boundary checks that enable or
/// disable each stepper button.
/// </summary>
public sealed class TimeSelectService
{
    private static readonly int FirstPmHour = 12;

    public TimeSelectService(PickerConfig config)
    {
        Config = config;
    }

    public PickerConfig Config { get; }

    public int Hours(JDate t) =>
        Config.ShowTwentyFourHours ? t.Hour : (t.Hour % 12 == 0 ? 12 : t.Hour % 12);

    public int Minutes(JDate t) => t.Minute;
    public int Seconds(JDate t) => t.Second;
    public string Meridiem(JDate t) => Localizer.Meridiem(t, Config.Locale);

    public string HoursText(JDate t) => Pad(Hours(t));
    public string MinutesText(JDate t) => Pad(t.Minute);
    public string SecondsText(JDate t) => Pad(t.Second);

    private static string Pad(int v) => v.ToString("00", CultureInfo.InvariantCulture);

    /// <summary>Steps by one unit. The date is preserved; only the time of day moves.</summary>
    public JDate Step(JDate time, bool increase, string unit)
    {
        var seconds = unit switch
        {
            "hour" => (increase ? 1 : -1) * 3600,
            "minute" => (increase ? 1 : -1) * Config.MinutesInterval * 60,
            "second" => (increase ? 1 : -1) * Config.SecondsInterval,
            _ => 0
        };
        return ShiftTime(time, seconds);
    }

    /// <summary>Adds 12 hours for the meridiem toggle (date preserved).</summary>
    public JDate ToggleMeridiem(JDate time) =>
        ShiftTime(time, time.Hour < FirstPmHour ? 12 * 3600 : -12 * 3600);

    private static JDate ShiftTime(JDate t, int addSeconds)
    {
        var tod = t.Hour * 3600 + t.Minute * 60 + t.Second + addSeconds;
        tod = ((tod % 86400) + 86400) % 86400;
        return t.WithTime(tod / 3600, (tod % 3600) / 60, tod % 60);
    }

    public bool ShouldShowDecrease(JDate time, string unit)
    {
        if (AllBoundsNull()) return true;
        var newTime = Step(time, false, unit);
        return WithinTimeBounds(newTime) && WithinDateBounds(time);
    }

    public bool ShouldShowIncrease(JDate time, string unit)
    {
        if (AllBoundsNull()) return true;
        var newTime = Step(time, true, unit);
        return WithinTimeBounds(newTime) && WithinDateBounds(time);
    }

    public bool ShouldShowToggleMeridiem(JDate time)
    {
        if (AllBoundsNull()) return true;
        var newTime = ToggleMeridiem(time);
        return WithinTimeBounds(newTime) && WithinDateBounds(newTime);
    }

    private bool AllBoundsNull() =>
        Config.Min is null && Config.Max is null && Config.MinTime is null && Config.MaxTime is null;

    private bool WithinTimeBounds(JDate t)
    {
        var onlyTime = DateRangeUtils.OnlyTime(t);
        if (Config.MinTime is { } minT && !onlyTime.IsSameOrAfter(DateRangeUtils.OnlyTime(minT))) return false;
        if (Config.MaxTime is { } maxT && !onlyTime.IsSameOrBefore(DateRangeUtils.OnlyTime(maxT))) return false;
        return true;
    }

    private bool WithinDateBounds(JDate t)
    {
        if (Config.Min is { } min && !t.IsSameOrAfter(min, JGranularity.Second)) return false;
        if (Config.Max is { } max && !t.IsSameOrBefore(max, JGranularity.Second)) return false;
        return true;
    }
}
