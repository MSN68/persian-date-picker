// یه آبجکت گلوبال برای نگهداری اینستنس‌های تقویم 
// تا موقع از بین رفتن کامپوننت بلیزور بتونیم از رم پاکشون کنیم (جلوگیری از مموری لیک)
const instances = {};

export function init(element, dotNetHelper, options) {
    if (!element) return;

    // کانفیگ‌های دریافتی از سی‌شارپ رو با کانفیگ‌های پیش‌فرض ترکیب میکنیم
    const config = {
        format: options.format || 'YYYY/MM/DD',
        minDate: options.minDate,
        maxDate: options.maxDate,
        
        // این متد وقتی کاربر روی یه روز کلیک میکنه فراخوانی میشه
        onSelect: function(dateText) {
            // مقدار انتخاب شده رو میفرستیم سمت متد OnDateChanged تو سی‌شارپ
            dotNetHelper.invokeMethodAsync('OnDateChanged', dateText);
        }
    };

    try {
        // ایجاد نمونه از تقویم شما روی اینپوت مورد نظر
        // ممکنه اسم کلاس کتابخونه شما فرق داشته باشه، اینجا عوضش کن
        const picker = new PersianDatePicker(element, config); 
        
        // ذخیره آیدی یکتا برای هر اینپوت
        const id = element.id || Math.random().toString(36).substr(2, 9);
        element.dataset.dpId = id;
        instances[id] = picker;

        // اگه مقداری از قبل تو سی‌شارپ ست شده بود، به تقویم پاس میدیم
        if(options.initialValue && typeof picker.setDate === 'function'){
            picker.setDate(options.initialValue);
        }
        
    } catch (e) {
        console.error("مشکل در اجرای PersianDatePicker:", e);
    }
}

export function destroy(element) {
    if (!element || !element.dataset.dpId) return;
    
    const id = element.dataset.dpId;
    const picker = instances[id];
    
    if (picker) {
        // اگه کتابخونه اصلیت متدی برای پاکسازی (مثل destroy یا remove) داره اینجا صداش بزن
        if (typeof picker.destroy === 'function') {
            picker.destroy();
        }
        delete instances[id];
    }
}