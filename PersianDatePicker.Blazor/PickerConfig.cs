namespace PersianDatePicker.Blazor;

/// <summary>
/// All configuration for the picker, mirroring the Angular <c>IDatePickerModalConfig</c>
/// (which is the union of the common, day, month and time-select config interfaces).
/// Component parameters ultimately flow through here; the services read from this one
/// object so the calendar logic stays decoupled from the UI.
/// </summary>
public class PickerConfig
{
    // ---- Common (IConfig) ----
    public bool CloseOnSelect { get; set; } = true;
    public int CloseOnSelectDelay { get; set; } = 100;
    public bool OpenOnFocus { get; set; } = true;
    public bool OpenOnClick { get; set; } = true;
    public int OnOpenDelay { get; set; }
    public bool DisableKeypress { get; set; }
    public bool HideOnOutsideClick { get; set; } = true;
    /// <summary>Popup anchored below the input instead of a centered full-viewport modal.</summary>
    public bool Dropdown { get; set; }
    /// <summary>
    /// Show the confirm/close action bar. When left null the picker decides: modes that
    /// build a value across several clicks (range, time, daytime) get the bar.
    /// </summary>
    public bool? ShowActionButtons { get; set; }
    public string RangeSeparator { get; set; } = " - ";

    // ---- Mode / locale ----
    public DatePickerMode Mode { get; set; } = DatePickerMode.Day;
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;
    public DatePickerLocale Locale { get; set; } = DatePickerLocale.Persian;

    // ---- Bounds (Jalali, parsed) ----
    public JDate? Min { get; set; }
    public JDate? Max { get; set; }
    public JDate? MinTime { get; set; }
    public JDate? MaxTime { get; set; }

    // ---- Day calendar ----
    public Func<JDate, bool>? IsDayDisabledCallback { get; set; }
    public string WeekDayFormat { get; set; } = "dd";
    public bool ShowNearMonthDays { get; set; } = true;
    public bool ShowWeekNumbers { get; set; }
    public WeekDay FirstDayOfWeek { get; set; } = WeekDay.Saturday;
    public string MonthFormat { get; set; } = "MMMM YYYY";
    public bool AllowMultiSelect { get; set; }
    public bool EnableMonthSelector { get; set; } = true;
    public string DayBtnFormat { get; set; } = "D";
    public Func<JDate, string>? DayBtnFormatter { get; set; }
    public bool ShowGoToCurrent { get; set; } = true;
    public bool UnSelectOnClick { get; set; }

    // ---- Month calendar ----
    public string YearFormat { get; set; } = "YYYY";
    public string MonthBtnFormat { get; set; } = "MMMM";
    public Func<JDate, string>? MonthBtnFormatter { get; set; }

    // ---- Time select ----
    public string Hours12Format { get; set; } = "hh";
    public string Hours24Format { get; set; } = "HH";
    public string MeridiemFormat { get; set; } = "A";
    public string MinutesFormat { get; set; } = "mm";
    public int MinutesInterval { get; set; } = 1;
    public string SecondsFormat { get; set; } = "ss";
    public int SecondsInterval { get; set; } = 1;
    public bool ShowSeconds { get; set; }
    public bool ShowTwentyFourHours { get; set; }
    public string TimeSeparator { get; set; } = ":";

    // ---- Resolved defaults ----
    /// <summary>Effective output/input token format for the current mode + locale.</summary>
    public string Format { get; set; } = "YYYY/MM/DD";

    public PickerConfig Clone() => (PickerConfig)MemberwiseClone();

    /// <summary>Applies the locale/format defaults that depend on the chosen mode.</summary>
    public void ApplyModeDefaults()
    {
        var isFa = Locale == DatePickerLocale.Persian;
        var dateFormat = isFa ? "YYYY/MM/DD" : "DD-MM-YYYY";
        var monthFormat = isFa ? "MMMM YYYY" : "MMM, YYYY";
        var timeFormat = "HH:mm:ss";

        Format = Mode switch
        {
            DatePickerMode.Day => dateFormat,
            DatePickerMode.Month => monthFormat,
            DatePickerMode.Time => timeFormat,
            DatePickerMode.DayTime => $"{dateFormat} {timeFormat}",
            _ => dateFormat
        };

        // Gregorian default adjustments (the Angular GREGORIAN_CONFIG_EXTENTION).
        if (!isFa)
        {
            if (FirstDayOfWeek == WeekDay.Saturday) FirstDayOfWeek = WeekDay.Sunday;
            if (DayBtnFormat == "D") DayBtnFormat = "DD";
            UnSelectOnClick = true;
        }
    }

    /// <summary>
    /// Fills in the "smart" defaults the Angular <c>DatePickerModalService.getConfig</c>
    /// computes: auto action-bar for multi-step modes, auto closeOnSelect, and the
    /// read-only input for ranges.
    /// </summary>
    public void ApplyDerivedDefaults()
    {
        if (ShowActionButtons is null)
        {
            ShowActionButtons = SelectionMode == SelectionMode.Range
                                 || Mode == DatePickerMode.Time
                                 || Mode == DatePickerMode.DayTime;
        }

        if (ShowActionButtons == true)
        {
            CloseOnSelect = false;
        }

        if (AllowMultiSelect && !ShowActionButtons.GetValueOrDefault())
        {
            // Keep multi-select open so the user can pick several values.
        }

        if (SelectionMode == SelectionMode.Range)
        {
            DisableKeypress = true;
        }
    }
}
