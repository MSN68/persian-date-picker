namespace PersianDatePicker.Blazor.Models;

/// <summary>A single day cell in the day calendar grid.</summary>
public sealed class DayCell
{
    public DayCell(JDate date)
    {
        Date = date;
    }

    public JDate Date { get; }
    public bool Selected { get; set; }
    public bool CurrentMonth { get; set; }
    public bool PrevMonth { get; set; }
    public bool NextMonth { get; set; }
    public bool CurrentDay { get; set; }
    public bool Disabled { get; set; }

    // Range state (only meaningful in range mode).
    public bool RangeStart { get; set; }
    public bool RangeEnd { get; set; }
    public bool InRange { get; set; }
    public bool RangePreview { get; set; }
}

/// <summary>A single month cell in the month calendar grid.</summary>
public sealed class MonthCell
{
    public MonthCell(JDate date)
    {
        Date = date;
    }

    public JDate Date { get; }
    public bool Selected { get; set; }
    public bool CurrentMonth { get; set; }
    public bool Disabled { get; set; }
    public string Text { get; set; } = "";

    public bool RangeStart { get; set; }
    public bool RangeEnd { get; set; }
    public bool InRange { get; set; }
    public bool RangePreview { get; set; }
}
