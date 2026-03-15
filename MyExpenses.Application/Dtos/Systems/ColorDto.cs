namespace MyExpenses.Application.Dtos.Systems;

public class ColorDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string HexadecimalColorCode { get; set; }

    public DateTime DateAdded { get; init; }
}