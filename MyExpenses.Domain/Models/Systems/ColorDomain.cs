namespace Domain.Models.Systems;

public class ColorDomain
{
    public const int MaxNameLength = 55;
    public const int MaxHexadecimalColorCodeLength = 9;

    public int Id { get; set; }

    public string Name { get; set; }

    public string HexadecimalColorCode { get; set; }

    public DateTime DateAdded { get; init; }
}