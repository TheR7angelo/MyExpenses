namespace MyExpenses.Application.Dtos.Expenses;

public class ModePaymentDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool CanBeDeleted { get; init; }

    public DateTime DateAdded { get; init; } = DateTime.Now;
}