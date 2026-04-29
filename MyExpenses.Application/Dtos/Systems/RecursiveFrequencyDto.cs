namespace MyExpenses.Application.Dtos.Systems;

public class RecursiveFrequencyDto
{
    public int Id { get; set; }

    public required string Frequency { get; set; }

    public required string Description { get; set; }
}