namespace Domain.Models.Dependencies;

public class DeletionDependency
{
    public required int Count { get; set; }
    public required EntityType Category { get; set; }
}