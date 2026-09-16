using PersianDatePicker.Blazor.Models;
using PersianDatePicker.Blazor.Text;

namespace PersianDatePicker.Blazor.Services;

/// <summary>
/// Pure logic for the day calendar: building the 42-cell grid, weekday names,
/// min/max disabling, the month/year header and range-state. Ported from the
/// Angular <c>DayCalendarService</c>.
/// </summary>
public sealed class DayCalendarService
{
    // Jalali day order, indexed by JDate.DayIndex (0 = Saturday .. 6 = Friday).
    // The English grid, by contrast, is indexed by .NET DayOfWeek (0 = Sunday .. 6 = Saturday).
    private static readonly DayOfWeek[] PersianOrder =
    {
        DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday,
        DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
    };

    public DayCalendarService(PickerConfig config)
    {
        Config = config;
    }

    public PickerConfig Config { get; }

    public DatePickerLocale Locale => Config.Locale;

    public DayOfWeek[] GenerateWeekdays()
    {
        var start = FirstDowIndex();
        var list = new List<DayOfWeek>(7);
        for (var i = 0; i < 7; i++)
        {
            list.Add(PersianOrder[(start + i) % 7]);
        }
        return list.ToArray();
    }

    public List<DayCell> GenerateMonthArray(JDate month, IReadOnlyList<JDate> selected)
    {
        var firstDayOfBoard = month.StartOfMonth();
        // Roll back until the board's first cell lands on the configured first day of week.
        var firstDow = FirstDowIndex();
        var guard = 0;
        while (firstDayOfBoard.DayIndex != firstDow && guard++ < 7)
        {
            firstDayOfBoard = firstDayOfBoard.AddDays(-1);
        }

        var current = firstDayOfBoard;
        var prevMonth = month.AddMonths(-1);
        var nextMonth = month.AddMonths(1);
        var today = JDate.Today();

        var cells = new List<DayCell>(42);
        for (var i = 0; i < 42; i++)
        {
            var cell = new DayCell(current)
            {
                Selected = selected.Any(s => s.IsSame(current, JGranularity.Day)),
                CurrentMonth = current.IsSame(month, JGranularity.Month),
                PrevMonth = current.IsSame(prevMonth, JGranularity.Month),
                NextMonth = current.IsSame(nextMonth, JGranularity.Month),
                CurrentDay = current.IsSame(today, JGranularity.Day),
                Disabled = IsDateDisabled(current)
            };
            cells.Add(cell);
            current = current.AddDays(1);
        }

        return cells;
    }

    private int FirstDowIndex()
    {
        return Config.FirstDayOfWeek switch
        {
            WeekDay.Saturday => 0,
            WeekDay.Sunday => 1,
            WeekDay.Monday => 2,
            WeekDay.Tuesday => 3,
            WeekDay.Wednesday => 4,
            WeekDay.Thursday => 5,
            _ => 0
        };
    }

    public bool IsDateDisabled(JDate date)
    {
        if (Config.IsDayDisabledCallback is { } cb) return cb(date);
        if (Config.Min is { } min && date.IsBefore(min, JGranularity.Day)) return true;
        if (Config.Max is { } max && date.IsAfter(max, JGranularity.Day)) return true;
        return false;
    }

    public string GetHeaderLabel(JDate month) =>
        Localizer.Format(month, Config.MonthFormat, Locale);

    public string GetDayAriaLabel(JDate date)
    {
        var format = Localizer.IsPersian(Locale) ? "dddd D MMMM YYYY" : "dddd, MMMM D, YYYY";
        return Localizer.Format(date, format, Locale);
    }

    public string GetDayBtnText(JDate date)
    {
        if (Config.DayBtnFormatter is { } f) return f(date);
        return Localizer.Format(date, Config.DayBtnFormat, Locale);
    }

    public bool ShouldShowLeft(JDate month) =>
        Config.Min is null || Config.Min is { } min && min.IsBefore(month, JGranularity.Month);

    public bool ShouldShowRight(JDate month) =>
        Config.Max is null || Config.Max is { } max && max.IsAfter(month, JGranularity.Month);

    public bool ShouldShowGoToCurrent() =>
        Config.ShowGoToCurrent
        && Config.Mode != DatePickerMode.Time
        && DateRangeUtils.IsDateInRange(JDate.Today(), Config.Min, Config.Max);

    /// <summary>Where <paramref name="date"/> sits inside the range (including the hover preview).</summary>
    public RangeState GetRangeState(JDate date, IReadOnlyList<JDate> selected, JDate? hovered, JGranularity granularity)
    {
        var from = selected.Count > 0 ? selected[0] : default;
        var hasFrom = selected.Count > 0;
        var to = selected.Count > 1 ? selected[1] : default;
        var hasTo = selected.Count > 1;

        if (!hasFrom)
        {
            return new RangeState(false, false, false, false);
        }

        var previewEnd = !hasTo && hovered is { } h && h.IsAfter(from, granularity) ? hovered : null;
        var previewStart = !hasTo && hovered is { } h2 && h2.IsBefore(from, granularity) ? hovered : null;
        var start = previewStart ?? from;
        var end = hasTo ? to : previewEnd ?? (previewStart != null ? from : null);
        var preview = !hasTo && (previewStart != null || previewEnd != null);

        if (end is { } e)
        {
            return new RangeState(date.IsSame(start, granularity),
                date.IsSame(e, granularity),
                date.IsBetween(start, e, granularity),
                preview);
        }

        return new RangeState(date.IsSame(start, granularity), false, false, preview);
    }
}

/// <summary>Position of a cell relative to the selected range.</summary>
public readonly record struct RangeState(bool IsStart, bool IsEnd, bool IsInRange, bool IsPreview);
