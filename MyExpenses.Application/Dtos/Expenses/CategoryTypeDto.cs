using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Dtos.Expenses;

public class CategoryTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public required ColorDto Color { get; set; }

    public DateTime? DateAdded { get; set; } = DateTime.Now;
}