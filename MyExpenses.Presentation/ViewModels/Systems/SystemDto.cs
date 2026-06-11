using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Systems;

[DirtyTracking]
public partial class AppSettingsViewModel
{
    [DirtyTrackedProperty]
    public SystemViewModel SystemSettings { get; set; } = new();

    [DirtyTrackedProperty]
    public InterfaceViewModel InterfaceSettings { get; set; } = new();
}

[DirtyTracking]
public partial class SystemViewModel
{
    [DirtyTrackedProperty]
    public int MaxDaysLog { get; set; }

    [DirtyTrackedProperty]
    public int MaxBackupDatabase { get; set; }

    [DirtyTrackedProperty]
    public DateTime? CallBackLaterTime { get; set; }
}

[DirtyTracking]
public partial class InterfaceViewModel
{
    [DirtyTrackedProperty]
    public string Language { get; set; } = "en-001";

    [DirtyTrackedProperty]
    public ThemeViewModel Theme { get; set; } = new();

    [DirtyTrackedProperty]
    public ClockViewModel Clock { get; set; } = new();
}

[DirtyTracking]
public partial class ThemeViewModel
{
    [DirtyTrackedProperty]
    public int BaseTheme { get; set; } = 2;

    [DirtyTrackedProperty]
    public string HexadecimalCodePrimaryColor { get; set; } = "#FF32CD30";

    [DirtyTrackedProperty]
    public string HexadecimalCodeSecondaryColor { get; set; } = "#FFFFA500";
}

[DirtyTracking]
public partial class ClockViewModel
{
    [DirtyTrackedProperty]
    public bool Is24Hours { get; set; }
}