namespace Domain.Models.Systems;

public class ColorDomain
{
    public const int MaxNameLength = 55;
    public const int MaxHexadecimalColorCodeLength = 9;

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string HexadecimalColorCode { get; set; } = string.Empty;

    public DateTime DateAdded { get; init; }
}