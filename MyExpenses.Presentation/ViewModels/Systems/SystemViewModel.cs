using CommunityToolkit.Mvvm.ComponentModel;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Systems;

[DirtyTracking]
public partial class AppSettingsViewModel : ObservableObject
{
    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial SystemViewModel SystemSettings { get; set; } = new();

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial InterfaceViewModel InterfaceSettings { get; set; } = new();
}

[DirtyTracking]
public partial class SystemViewModel : ObservableObject
{
    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial int MaxDaysLog { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial int MaxBackupDatabase { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial DateTime? CallBackLaterTime { get; set; }
}

[DirtyTracking]
public partial class InterfaceViewModel : ObservableObject
{
    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string Language { get; set; } = "en-001";

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial ThemeViewModel Theme { get; set; } = new();

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial ClockViewModel Clock { get; set; } = new();
}

[DirtyTracking]
public partial class ThemeViewModel : ObservableObject
{
    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial int BaseTheme { get; set; } = 2;

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string HexadecimalCodePrimaryColor { get; set; } = "#FF32CD30";

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string HexadecimalCodeSecondaryColor { get; set; } = "#FFFFA500";
}

[DirtyTracking]
public partial class ClockViewModel : ObservableObject
{
    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool Is24Hours { get; set; }
}