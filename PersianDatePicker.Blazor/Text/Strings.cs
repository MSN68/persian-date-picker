namespace PersianDatePicker.Blazor.Text;

/// <summary>Localized UI strings (button labels, dialog titles, range guidance).</summary>
public static class Strings
{
    public static bool IsFa(DatePickerLocale locale) => locale == DatePickerLocale.Persian;

    public static string Prev(DatePickerLocale l) => IsFa(l) ? "قبلی" : "Previous";
    public static string Next(DatePickerLocale l) => IsFa(l) ? "بعدی" : "Next";
    public static string Today(DatePickerLocale l) => IsFa(l) ? "امروز" : "Today";
    public static string GoToTodayTitle(DatePickerLocale l) => IsFa(l) ? "برو به امروز" : "Go to today";
    public static string ChooseMonthYear(DatePickerLocale l) => IsFa(l) ? "انتخاب ماه و سال" : "Choose month and year";

    public static string OpenCalendar(DatePickerLocale l) => IsFa(l) ? "باز کردن تقویم" : "Open calendar";
    public static string CloseCalendar(DatePickerLocale l) => IsFa(l) ? "بستن تقویم" : "Close calendar";
    public static string Cancel(DatePickerLocale l) => IsFa(l) ? "انصراف" : "Cancel";
    public static string Confirm(DatePickerLocale l) => IsFa(l) ? "تایید" : "Confirm";

    public static string NotSelected(DatePickerLocale l) => IsFa(l) ? "انتخاب نشده" : "Not selected";
    public static string StartDate(DatePickerLocale l) => IsFa(l) ? "شروع بازه" : "Start date";
    public static string EndDate(DatePickerLocale l) => IsFa(l) ? "پایان بازه" : "End date";

    public static string RangeStartHint(DatePickerLocale l) => IsFa(l) ? "ابتدا تاریخ شروع را انتخاب کنید" : "Choose a start date";
    public static string RangeEndHint(DatePickerLocale l) => IsFa(l) ? "حالا تاریخ پایان را انتخاب کنید" : "Now choose an end date";
    public static string RangeReadyHint(DatePickerLocale l) => IsFa(l) ? "بازه آماده است؛ برای ثبت، تایید کنید" : "Your range is ready to confirm";

    public static string DayRangeHint(DatePickerLocale l) => IsFa(l) ? "تاریخ پایان بازه را انتخاب کنید" : "Pick the end date";
    public static string MonthRangeHint(DatePickerLocale l) => IsFa(l) ? "ماه پایان بازه را انتخاب کنید" : "Pick the end month";

    public static string DialogLabel(DatePickerLocale l, DatePickerMode mode)
    {
        if (IsFa(l))
        {
            return mode switch
            {
                DatePickerMode.Day => "انتخاب تاریخ",
                DatePickerMode.Month => "انتخاب ماه",
                DatePickerMode.Time => "انتخاب ساعت",
                DatePickerMode.DayTime => "انتخاب تاریخ و ساعت",
                _ => "انتخاب تاریخ"
            };
        }
        return mode switch
        {
            DatePickerMode.Day => "Choose date",
            DatePickerMode.Month => "Choose month",
            DatePickerMode.Time => "Choose time",
            DatePickerMode.DayTime => "Choose date and time",
            _ => "Choose date"
        };
    }

    public static string MinError(DatePickerLocale l) => IsFa(l) ? "تاریخ انتخابی نمی‌تواند از {0} کوچکتر باشد." : "Selected date cannot be earlier than {0}.";
    public static string MaxError(DatePickerLocale l) => IsFa(l) ? "تاریخ انتخابی نمی‌تواند از {0} بزرگتر باشد." : "Selected date cannot be later than {0}.";
}
