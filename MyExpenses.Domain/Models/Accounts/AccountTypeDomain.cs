namespace Domain.Models.Accounts;

public class AccountTypeDomain
{
    public const int MaxNameLength = 100;

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime DateAdded { get; set; }
}