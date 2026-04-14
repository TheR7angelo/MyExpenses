namespace MyExpenses.Presentation.ViewModels.Systems;

public class RecursiveFrequencyViewModel
{
    public int Id { get; init; }

    // [NotMapped]
    // public ERecursiveFrequency ERecursiveFrequency
    //     => (ERecursiveFrequency)Id;

    public string Frequency { get; init; } = string.Empty;

    public string? Description { get; init; } = string.Empty;
}