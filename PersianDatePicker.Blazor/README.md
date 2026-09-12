# Persian DatePicker for Blazor 📅

A lightweight, highly customizable, and easy-to-use Persian (Jalali) DatePicker component for Blazor (WebAssembly & Server), built on top of the powerful Vanilla JS library.

## Features
* 🚀 Support for both Blazor WebAssembly and Blazor Server.
* 🔄 Seamless two-way data binding (`@bind-Value`).
* 🎨 Fully customizable CSS.
* ⚡ Lightweight and fast.
* 📅 Supports Min/Max dates, custom formats, and initial values.

## Installation

Install the package via NuGet Package Manager Console:
```bash
Install-Package HosseinVatankhah.PersianDatePicker.Blazor

Or via .NET CLI:

dotnet add package HosseinVatankhah.PersianDatePicker.Blazor

Setup
After installing the package, you need to add the required CSS and JS files to your project.

1. Add Static Assets
Add the following lines to your wwwroot/index.html (Blazor WebAssembly) or Components/App.razor (Blazor Server), inside the <head> and <body> tags:

In <head>:

<link href="_content/HosseinVatankhah.PersianDatePicker.Blazor/css/persian-datepicker.min.css" rel="stylesheet" />

At the end of <body> (before Blazor script):

<script src="_content/HosseinVatankhah.PersianDatePicker.Blazor/js/persian-datepicker.min.js"></script>
(Note: adjust the paths above if your file names are different in the wwwroot folder).

2. Add _Imports.razor
Add the following using statement to your _Imports.razor file:

@using PersianDatePicker.Blazor

Usage
You can now use the component anywhere in your Blazor pages.

Basic Example:

<PersianDatePicker @bind-Value="mySelectedDate" Placeholder="تاریخ را انتخاب کنید..."/>

<p>Selected Date: @mySelectedDate</p>

@code {
    private string mySelectedDate;
}

Advanced Example (Min/Max and Formatting):

<PersianDatePicker @bind-Value="eventDate" CssClass="my-custom-input-class" Format="YYYY/MM/DD" MaxDate="1405/12/29" MinDate="1402/01/01"/>

Contributing
Contributions are always welcome! Please feel free to submit a Pull Request.

License
This project is licensed under the MIT License.






