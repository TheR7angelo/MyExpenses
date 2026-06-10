using System.Text.Json.Serialization;

namespace MyExpenses.Application.Dtos.Systems;

public class AppSettingsDto
{
    [JsonPropertyName("system")]
    public SystemDto SystemSettings { get; set; } = new();

    [JsonPropertyName("interface")]
    public InterfaceDto InterfaceSettings { get; set; } = new();
}

public class SystemDto
{
    [JsonPropertyName("max_days_log")]
    public int MaxDaysLog { get; set; }

    [JsonPropertyName("max_backup_database")]
    public int MaxBackupDatabase { get; set; }

    [JsonPropertyName("call_back_later_time")]
    public DateTime? CallBackLaterTime { get; set; }
}

public class InterfaceDto
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = "en-001";

    [JsonPropertyName("theme")]
    public ThemeDto Theme { get; set; } = new();

    [JsonPropertyName("clock")]
    public ClockDto Clock { get; set; } = new();
}

public class ThemeDto
{
    [JsonPropertyName("base_theme")]
    public int BaseTheme { get; set; } = 2;

    [JsonPropertyName("hexadecimal_code_primary_color")]
    public string HexadecimalCodePrimaryColor { get; set; } = "#FF32CD30";

    [JsonPropertyName("hexadecimal_code_secondary_color")]
    public string HexadecimalCodeSecondaryColor { get; set; } = "#FFFFA500";
}

public class ClockDto
{
    [JsonPropertyName("is_24_hours")]
    public bool Is24Hours { get; set; }
}