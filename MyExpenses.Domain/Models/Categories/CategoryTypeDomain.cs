using Domain.Models.Systems;

namespace Domain.Models.Categories;

public class CategoryTypeDomain
{
    public const int MaxNameLength = 55;

    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime DateAdded { get; init; } = DateTime.Now;

    public required ColorDomain Color { get; set; }
}