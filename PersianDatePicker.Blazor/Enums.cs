namespace PersianDatePicker.Blazor;

/// <summary>The kind of calendar the picker presents.</summary>
public enum DatePickerMode
{
    Day,
    Month,
    Time,
    DayTime
}

/// <summary>How many values a single selection can hold.</summary>
public enum SelectionMode
{
    Single,
    Range
}

/// <summary>The day of week to use as the first column of the grid.</summary>
public enum WeekDay
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}

/// <summary>Supported locales. Any non-<see cref="Persian"/> value renders in English.</summary>
public enum DatePickerLocale
{
    Persian,
    English
}
