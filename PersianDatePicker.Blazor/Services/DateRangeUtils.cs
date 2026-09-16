namespace PersianDatePicker.Blazor.Services;

/// <summary>
/// Shared date helpers ported from the Angular <c>UtilsService</c>: range containment,
/// time-only extraction, granularity mapping and selection updates.
/// </summary>
public static class DateRangeUtils
{
    public static bool IsDateInRange(JDate date, JDate? from, JDate? to)
    {
        if (from is null && to is null) return true;
        if (from is null && to is { } t) return date.IsBefore(t, JGranularity.Day);
        if (from is { } f && to is null) return date.IsAfter(f, JGranularity.Day);
        return from is { } f2 && to is { } t2 && date.IsBetween(f2, t2, JGranularity.Day);
    }

    /// <summary>Strips the date part, keeping only HH:mm:ss (midnight-based).</summary>
    public static JDate OnlyTime(JDate m) => JDate.Today().AddTime(m.Hour, m.Minute, m.Second);

    public static JGranularity GranularityFromMode(DatePickerMode mode) => mode switch
    {
        DatePickerMode.Time => JGranularity.Second,
        DatePickerMode.DayTime => JGranularity.Second,
        DatePickerMode.Month => JGranularity.Month,
        _ => JGranularity.Day
    };

    /// <summary>Single / multi-select toggle of a date, ported from <c>updateSelected</c>.</summary>
    public static List<JDate> UpdateSelected(bool isMultiple, List<JDate> currentlySelected, JDate date, JGranularity granularity)
    {
        var selected = currentlySelected.Any(d => d.IsSame(date, granularity));
        if (isMultiple)
        {
            if (!selected)
            {
                currentlySelected.Add(date);
            }
            else
            {
                currentlySelected.RemoveAll(d => d.IsSame(date, granularity));
            }
            return currentlySelected;
        }

        return selected ? new List<JDate>() : new List<JDate> { date };
    }

    /// <summary>
    /// Range selection: first click opens the range, second click closes it (swapping the
    /// ends when the user picks backwards), a third click starts a fresh range. Ported from
    /// <c>updateSelectedRange</c>.
    /// </summary>
    public static List<JDate> UpdateSelectedRange(List<JDate> currentlySelected, JDate date, JGranularity granularity)
    {
        var from = currentlySelected.Count > 0 ? currentlySelected[0] : default;
        var hasFrom = currentlySelected.Count > 0;
        var to = currentlySelected.Count > 1 ? currentlySelected[1] : default;
        var hasTo = currentlySelected.Count > 1;

        if (!hasFrom || hasTo)
        {
            return new List<JDate> { date };
        }

        if (date.IsBefore(from, granularity))
        {
            return new List<JDate> { date, from };
        }

        return new List<JDate> { from, date };
    }

    public static JDate GetDefaultDisplayDate(JDate current, IReadOnlyList<JDate> selected, bool allowMultiSelect, JDate? min)
    {
        JDate m;
        if (selected.Count > 0)
        {
            m = selected[allowMultiSelect ? selected.Count - 1 : 0];
        }
        else
        {
            m = current;
        }
        if (min is { } minDate && minDate.IsAfter(JDate.Today(), JGranularity.Day) && selected.Count == 0)
        {
            m = minDate;
        }
        return m;
    }
}
