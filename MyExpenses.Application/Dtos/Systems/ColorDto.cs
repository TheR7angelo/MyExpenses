namespace MyExpenses.Application.Dtos.Systems;

public class ColorDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string HexadecimalColorCode { get; set; } = string.Empty;

    public DateTime DateAdded { get; init; }
}