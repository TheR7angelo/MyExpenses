using Microsoft.Maui.Controls.Shapes;

namespace MyExpenses.Smartphones.PackIcons;

public static class EPackIconsUtils
{
    public static string ToGeometryString(this EPackIcons icon)
    {
        var geometryString = icon switch
        {
            EPackIcons.Abacus => "M5 5H7V11H5V5M10 5H8V11H10V5M5 19H7V13H5V19M10 13H8V19H10V17H15V15H10V13M2 21H4V3H2V21M20 3V7H13V5H11V11H13V9H20V15H18V13H16V19H18V17H20V21H22V3H20Z",
            EPackIcons.ArrowDownThin => "M7.03 13.92H11.03V5L13.04 4.97V13.92H17.03L12.03 18.92Z",
            EPackIcons.ArrowRightThin => "M14 16.94V12.94H5.08L5.05 10.93H14V6.94L19 11.94Z",
            EPackIcons.Cellphone => "M17,19H7V5H17M17,1H7C5.89,1 5,1.89 5,3V21A2,2 0 0,0 7,23H17A2,2 0 0,0 19,21V3C19,1.89 18.1,1 17,1Z",
            EPackIcons.CheckboxBlankOutline => "M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3M19,5V19H5V5H19Z",
            EPackIcons.CheckboxOutline => "M19,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M19,5V19H5V5H19M10,17L6,13L7.41,11.58L10,14.17L16.59,7.58L18,9",
            EPackIcons.ClockFast => "M15,4A8,8 0 0,1 23,12A8,8 0 0,1 15,20A8,8 0 0,1 7,12A8,8 0 0,1 15,4M15,6A6,6 0 0,0 9,12A6,6 0 0,0 15,18A6,6 0 0,0 21,12A6,6 0 0,0 15,6M14,8H15.5V11.78L17.83,14.11L16.77,15.17L14,12.4V8M2,18A1,1 0 0,1 1,17A1,1 0 0,1 2,16H5.83C6.14,16.71 6.54,17.38 7,18H2M3,13A1,1 0 0,1 2,12A1,1 0 0,1 3,11H5.05L5,12L5.05,13H3M4,8A1,1 0 0,1 3,7A1,1 0 0,1 4,6H7C6.54,6.62 6.14,7.29 5.83,8H4Z",
            EPackIcons.CloseCircle => "M12,2C17.53,2 22,6.47 22,12C22,17.53 17.53,22 12,22C6.47,22 2,17.53 2,12C2,6.47 6.47,2 12,2M15.59,7L12,10.59L8.41,7L7,8.41L10.59,12L7,15.59L8.41,17L12,13.41L15.59,17L17,15.59L13.41,12L17,8.41L15.59,7Z",
            EPackIcons.Database => "M12,3C7.58,3 4,4.79 4,7C4,9.21 7.58,11 12,11C16.42,11 20,9.21 20,7C20,4.79 16.42,3 12,3M4,9V12C4,14.21 7.58,16 12,16C16.42,16 20,14.21 20,12V9C20,11.21 16.42,13 12,13C7.58,13 4,11.21 4,9M4,14V17C4,19.21 7.58,21 12,21C16.42,21 20,19.21 20,17V14C20,16.21 16.42,18 12,18C7.58,18 4,16.21 4,14Z",
            EPackIcons.DatabaseExport => "M12,3C7.58,3 4,4.79 4,7 4,9.21 7.58,11 12,11 12.5,11 13,10.97 13.5,10.92L13.5,9.5 16.39,9.5 15.39,8.5 18.9,5C17.5,3.8,14.94,3,12,3 M18.92,7.08L17.5,8.5 20,11 15,11 15,13 20,13 17.5,15.5 18.92,16.92 23.84,12 M4,9L4,12C4,14.21 7.58,16 12,16 13.17,16 14.26,15.85 15.25,15.63L16.38,14.5 13.5,14.5 13.5,12.92C13,12.97 12.5,13 12,13 7.58,13 4,11.21 4,9 M4,14L4,17C4,19.21 7.58,21 12,21 14.94,21 17.5,20.2 18.9,19L17,17.1C15.61,17.66 13.9,18 12,18 7.58,18 4,16.21 4,14z",
            EPackIcons.DatabaseImport => "M12,3C8.59,3,5.69,4.07,4.54,5.57L9.79,10.82C10.5,10.93 11.22,11 12,11 16.42,11 20,9.21 20,7 20,4.79 16.42,3 12,3 M3.92,7.08L2.5,8.5 5,11 0,11 0,13 5,13 2.5,15.5 3.92,16.92 8.84,12 M20,9C20,11.21 16.42,13 12,13 11.34,13 10.7,12.95 10.09,12.87L7.62,15.34C8.88,15.75 10.38,16 12,16 16.42,16 20,14.21 20,12 M20,14C20,16.21 16.42,18 12,18 9.72,18 7.67,17.5 6.21,16.75L4.53,18.43C5.68,19.93 8.59,21 12,21 16.42,21 20,19.21 20,17z",
            EPackIcons.Dropbox => "M150.94,17.51L0,115.32 105.06,199.85 256,105.66C256,105.66,150.94,17.51,150.94,17.51z M0,281.96L150.94,380.97999999999996 256,292.8299999999999 105.06,199.8499999999999C105.06,199.8499999999999,0,281.96,0,281.96z M256,292.83L362.26,380.98 512,283.17 408.15,199.85000000000002C408.15,199.85000000000002,255.99999999999997,292.83000000000004,255.99999999999997,292.83000000000004z M512,115.32L362.26,17.51 256,105.66000000000001 408.15,199.85000000000002 512,115.32000000000002z M257.21,312.15L150.95,400.29999999999995 106.26999999999998,370.10999999999996 106.26999999999998,403.91999999999996 257.21,494.48999999999995 408.15,403.91999999999996 408.15,370.10999999999996 362.26,400.29999999999995C362.26,400.29999999999995,257.2,312.15,257.2,312.15z",
            EPackIcons.Filter => "M14,12V19.88C14.04,20.18 13.94,20.5 13.71,20.71C13.32,21.1 12.69,21.1 12.3,20.71L10.29,18.7C10.06,18.47 9.96,18.16 10,17.87V12H9.97L4.21,4.62C3.87,4.19 3.95,3.56 4.38,3.22C4.57,3.08 4.78,3 5,3V3H19V3C19.22,3 19.43,3.08 19.62,3.22C20.05,3.56 20.13,4.19 19.79,4.62L14.03,12H14Z",
            EPackIcons.FilterCheck => "M12 12V19.88C12.04 20.18 11.94 20.5 11.71 20.71C11.32 21.1 10.69 21.1 10.3 20.71L8.29 18.7C8.06 18.47 7.96 18.16 8 17.87V12H7.97L2.21 4.62C1.87 4.19 1.95 3.56 2.38 3.22C2.57 3.08 2.78 3 3 3H17C17.22 3 17.43 3.08 17.62 3.22C18.05 3.56 18.13 4.19 17.79 4.62L12.03 12H12M17.75 21L15 18L16.16 16.84L17.75 18.43L21.34 14.84L22.5 16.25L17.75 21",
            EPackIcons.Folder => "M10,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V8C22,6.89 21.1,6 20,6H12L10,4Z",
            EPackIcons.History => "M13.5,8H12V13L16.28,15.54L17,14.33L13.5,12.25V8M13,3A9,9 0 0,0 4,12H1L4.96,16.03L9,12H6A7,7 0 0,1 13,5A7,7 0 0,1 20,12A7,7 0 0,1 13,19C11.07,19 9.32,18.21 8.06,16.94L6.64,18.36C8.27,20 10.5,21 13,21A9,9 0 0,0 22,12A9,9 0 0,0 13,3",
            EPackIcons.InvoicePlus => "M3 22V3H21V13.34C20.37 13.12 19.7 13 19 13C15.69 13 13 15.69 13 19C13 19.65 13.1 20.28 13.3 20.86L12 20L9 22L6 20L3 22M18 15V18H15V20H18V23H20V20H23V18H20V15H18Z",
            EPackIcons.Minus => "M19,13L5,13 5,11 19,11 19,13z",
            EPackIcons.MinusCheckboxOutline => "M19,19V5H5V19H19M19,3A2,2 0 0,1 21,5V19A2,2 0 0,1 19,21H5A2,2 0 0,1 3,19V5C3,3.89 3.9,3 5,3H19M17,11V13H7V11H17Z",
            EPackIcons.Plus => "M19,13L13,13 13,19 11,19 11,13 5,13 5,11 11,11 11,5 13,5 13,11 19,11 19,13z",
            EPackIcons.Refresh => "M17.65,6.35C16.2,4.9 14.21,4 12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20C15.73,20 18.84,17.45 19.73,14H17.65C16.83,16.33 14.61,18 12,18A6,6 0 0,1 6,12A6,6 0 0,1 12,6C13.66,6 15.14,6.69 16.22,7.78L13,11H20V4L17.65,6.35Z",
            EPackIcons.SwapHorizontal => "M21,9L17,5V8H10V10H17V13M7,11L3,15L7,19V16H14V14H7V11Z",
            EPackIcons.WeatherPartlyCloudy => "M12.74,5.47C15.1,6.5 16.35,9.03 15.92,11.46C17.19,12.56 18,14.19 18,16V16.17C18.31,16.06 18.65,16 19,16A3,3 0 0,1 22,19A3,3 0 0,1 19,22H6A4,4 0 0,1 2,18A4,4 0 0,1 6,14H6.27C5,12.45 4.6,10.24 5.5,8.26C6.72,5.5 9.97,4.24 12.74,5.47M11.93,7.3C10.16,6.5 8.09,7.31 7.31,9.07C6.85,10.09 6.93,11.22 7.41,12.13C8.5,10.83 10.16,10 12,10C12.7,10 13.38,10.12 14,10.34C13.94,9.06 13.18,7.86 11.93,7.3M13.55,3.64C13,3.4 12.45,3.23 11.88,3.12L14.37,1.82L15.27,4.71C14.76,4.29 14.19,3.93 13.55,3.64M6.09,4.44C5.6,4.79 5.17,5.19 4.8,5.63L4.91,2.82L7.87,3.5C7.25,3.71 6.65,4.03 6.09,4.44M18,9.71C17.91,9.12 17.78,8.55 17.59,8L19.97,9.5L17.92,11.73C18.03,11.08 18.05,10.4 18,9.71M3.04,11.3C3.11,11.9 3.24,12.47 3.43,13L1.06,11.5L3.1,9.28C3,9.93 2.97,10.61 3.04,11.3M19,18H16V16A4,4 0 0,0 12,12A4,4 0 0,0 8,16H6A2,2 0 0,0 4,18A2,2 0 0,0 6,20H19A1,1 0 0,0 20,19A1,1 0 0,0 19,18Z",
            EPackIcons.WeatherPouring => "M9,12C9.53,12.14 9.85,12.69 9.71,13.22L8.41,18.05C8.27,18.59 7.72,18.9 7.19,18.76C6.65,18.62 6.34,18.07 6.5,17.54L7.78,12.71C7.92,12.17 8.47,11.86 9,12M13,12C13.53,12.14 13.85,12.69 13.71,13.22L11.64,20.95C11.5,21.5 10.95,21.8 10.41,21.66C9.88,21.5 9.56,20.97 9.7,20.43L11.78,12.71C11.92,12.17 12.47,11.86 13,12M17,12C17.53,12.14 17.85,12.69 17.71,13.22L16.41,18.05C16.27,18.59 15.72,18.9 15.19,18.76C14.65,18.62 14.34,18.07 14.5,17.54L15.78,12.71C15.92,12.17 16.47,11.86 17,12M17,10V9A5,5 0 0,0 12,4C9.5,4 7.45,5.82 7.06,8.19C6.73,8.07 6.37,8 6,8A3,3 0 0,0 3,11C3,12.11 3.6,13.08 4.5,13.6V13.59C5,13.87 5.14,14.5 4.87,14.96C4.59,15.43 4,15.6 3.5,15.32V15.33C2,14.47 1,12.85 1,11A5,5 0 0,1 6,6C7,3.65 9.3,2 12,2C15.43,2 18.24,4.66 18.5,8.03L19,8A4,4 0 0,1 23,12C23,13.5 22.2,14.77 21,15.46V15.46C20.5,15.73 19.91,15.57 19.63,15.09C19.36,14.61 19.5,14 20,13.72V13.73C20.6,13.39 21,12.74 21,12A2,2 0 0,0 19,10H17Z",
            EPackIcons.WhiteBalanceSunny => "M3.55 19.09L4.96 20.5L6.76 18.71L5.34 17.29M12 6C8.69 6 6 8.69 6 12S8.69 18 12 18 18 15.31 18 12C18 8.68 15.31 6 12 6M20 13H23V11H20M17.24 18.71L19.04 20.5L20.45 19.09L18.66 17.29M20.45 5L19.04 3.6L17.24 5.39L18.66 6.81M13 1H11V4H13M6.76 5.39L4.96 3.6L3.55 5L5.34 6.81L6.76 5.39M1 13H4V11H1M13 20H11V23H13",
            _ => throw new ArgumentOutOfRangeException()
        };

        return geometryString;
    }

    public static Geometry? ToGeometry(this EPackIcons icon)
    {
        var geometryString = icon.ToGeometryString();

        // Add parsing logic here
        var converter = new PathGeometryConverter();
        return converter.ConvertFromInvariantString(geometryString) as Geometry;
    }
}