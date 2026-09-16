using PersianDatePicker.Blazor.Models;
using PersianDatePicker.Blazor.Text;

namespace PersianDatePicker.Blazor.Services;

/// <summary>Pure logic for the month calendar, ported from the Angular <c>MonthCalendarService</c>.</summary>
public sealed class MonthCalendarService
{
    private static readonly int YearStep = 21;

    public MonthCalendarService(PickerConfig config)
    {
        Config = config;
    }

    public PickerConfig Config { get; }
    public DatePickerLocale Locale => Config.Locale;

    public List<List<MonthCell>> GenerateYear(JDate year, IReadOnlyList<JDate> selected)
    {
        var index = year.StartOfYear();
        var today = JDate.Today();
        var rows = new List<List<MonthCell>>(4);
        for (var r = 0; r < 4; r++)
        {
            var row = new List<MonthCell>(3);
            for (var c = 0; c < 3; c++)
            {
                var cell = new MonthCell(index)
                {
                    Selected = selected.Any(s => index.IsSame(s, JGranularity.Month)),
                    CurrentMonth = index.IsSame(today, JGranularity.Month),
                    Disabled = IsMonthDisabled(index),
                    Text = Localizer.MonthLong(index, Locale)
                };
                row.Add(cell);
                index = index.AddMonths(1);
            }
            rows.Add(row);
        }
        return rows;
    }

    public bool IsMonthDisabled(JDate date)
    {
        if (Config.Min is { } min && date.IsBefore(min, JGranularity.Month)) return true;
        if (Config.Max is { } max && date.IsAfter(max, JGranularity.Month)) return true;
        return false;
    }

    public static int[] GenerateYearRange(JDate currentYear)
    {
        var startYear = (currentYear.Year / YearStep) * YearStep;
        var years = new int[YearStep];
        for (var i = 0; i < YearStep; i++)
        {
            years[i] = startYear + i;
        }
        return years;
    }

    public bool IsYearDisabled(int year, JDate currentView)
    {
        var date = currentView.StartOfMonth().WithYear(year);
        if (Config.Min is { } min && date.IsBefore(min, JGranularity.Year)) return true;
        if (Config.Max is { } max && date.IsAfter(max, JGranularity.Year)) return true;
        return false;
    }

    public bool ShouldShowLeft(JDate monthView) =>
        Config.Min is null || Config.Min is { } min && min.IsBefore(monthView, JGranularity.Year);

    public bool ShouldShowRight(JDate monthView) =>
        Config.Max is null || Config.Max is { } max && max.IsAfter(monthView, JGranularity.Year);

    public bool ShouldShowGoToCurrent() =>
        Config.ShowGoToCurrent && DateRangeUtils.IsDateInRange(JDate.Today(), Config.Min, Config.Max);

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

    public string GetMonthAriaLabel(JDate date)
    {
        var format = Localizer.IsPersian(Locale) ? "MMMM YYYY" : "MMMM YYYY";
        return Localizer.Format(date, format, Locale);
    }
}
