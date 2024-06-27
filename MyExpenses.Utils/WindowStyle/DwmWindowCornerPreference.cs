namespace MyExpenses.Utils.WindowStyle;

// The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
// what value of the enum to set.
// Copied from dwmapi.h
public enum DwmWindowCornerPreference
{
    DwmWcpDefault      = 0,
    DwmWcpDoNotRound   = 1,
    DwmWcpRound        = 2,
    DwmWcpSmallRound   = 3
}