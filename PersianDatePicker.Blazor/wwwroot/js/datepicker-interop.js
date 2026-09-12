export function initDatePicker(element, dotNetHelper, options) {
    // فرض میکنیم کتابخانه شما با دستور زیر مقداردهی (Initialize) میشود
    const picker = new PersianDatePicker(element, {
        ...options,
        onSelect: function(selectedDate) {
            // ارسال تاریخ انتخاب شده از JS به متد C#
            dotNetHelper.invokeMethodAsync('OnDateSelected', selectedDate);
        }
    });
}