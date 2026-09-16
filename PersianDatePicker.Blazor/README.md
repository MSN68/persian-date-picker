# Persian DatePicker for Blazor 📅

A self-contained, fully-managed Persian (Jalali) DatePicker component for Blazor (WebAssembly & Server) — **no external JS dependency required**. A faithful port of [PersianDatePicker](https://github.com/hosseinvatankhah0/persian-date-picker) (Angular) written entirely in C#/Razor.

## Features

* **4 selection modes** — Day, Month, Time, Day+Time (`Mode="DayTime"`)
* **Range selection** — `SelectionMode="Range"` with live preview band and range guidance
* **Modal, dropdown & inline modes** — centered modal, input-anchored dropdown, or inline calendar
* **Two-way binding** — `@bind-Value` (single date, range, or time)
* **12/24-hour time** — with meridiem toggle, configurable intervals, min/max time bounds
* **Locale support** — Persian (`fa`, digits + labels) and English (`en`)
* **Min/Max bounds** — Jalali date strings auto-parsed; error messages shown in the active locale
* **Custom disabled dates** — `IsDayDisabledCallback` (e.g. disable Fridays)
* **Keyboard navigation** — arrow/page keys, Home/End, Escape, Tab focus trap inside modal
* **Reactive Forms & ngModel-style** — implements `ControlValueAccessor` equivalent (two-way binding)
* **NuGet-publishable** — `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>` (or `dotnet pack`)

## Installation

```bash
dotnet add package HosseinVatankhah.PersianDatePicker.Blazor
```

## Usage

```razor
@using PersianDatePicker.Blazor

<!-- Single day, Persian locale -->
<PersianDatePicker @bind-Value="myDate" />

<!-- Day range -->
<PersianDatePicker @bind-Value="range" SelectionMode="SelectionMode.Range" />

<!-- Month picker -->
<PersianDatePicker @bind-Value="month" Mode="DatePickerMode.Month" />

<!-- Time picker, 24-hour -->
<PersianDatePicker @bind-Value="time" Mode="DatePickerMode.Time" ShowTwentyFourHours="true" />

<!-- Day + time combined -->
<PersianDatePicker @bind-Value="dayTime" Mode="DatePickerMode.DayTime" />

<!-- With bounds & custom callback -->
<PersianDatePicker @bind-Value="bounded"
                    MinDate="1403/01/01"
                    MaxDate="1404/12/29"
                    IsDayDisabledCallback="d => d.NetDayOfWeek == DayOfWeek.Friday" />

<!-- Dropdown mode (anchored below input) -->
<PersianDatePicker @bind-Value="dropdown" Dropdown="true" OpenOnFocus="false" />

<!-- Inline calendar (no input) -->
<PersianDatePicker @bind-Value="inline" Inline="true" />
```

## Sample

See `PersianDatePicker.Sample` — a Blazor WebAssembly project referencing this library with demos of every mode above at `/pickers`.

## Notes

* The picker uses .NET `PersianCalendar` for all Jalali math (no `jalali-moment` / JS interop).  
* Free-text input is parsed and validated live; min/max errors surface as the user types, with messages localized to the active locale.
* Range values are emitted as `from {rangeSeparator} to` (default ` - `). Multi-select is unsupported in range mode.

License: MIT
