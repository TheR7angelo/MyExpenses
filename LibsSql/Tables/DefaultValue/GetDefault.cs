namespace LibsSql.Tables.DefaultValue;

public static class GetDefault
{
    public static IEnumerable<WalletType> GetWalletTypes()
    {
        return new List<WalletType>
        {
            new() { Name = Translation.Cash },
            new() { Name = Translation.Saving },
            new() { Name = Translation.Main }
        };
    }
}