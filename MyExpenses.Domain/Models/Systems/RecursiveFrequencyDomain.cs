namespace Domain.Models.Systems;

public class RecursiveFrequencyDomain
{
    public const int MaxFrequencyLength = 55;
    public const int MaxDescriptionLength = 100;

    public int Id { get; set; }

    public string Frequency { get; set; }

    public string Description { get; set; }
}