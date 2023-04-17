namespace LibsSql.Tables.DefaultValue;

public static class GetDefault
{
    public static IEnumerable<object> GetDefaults()
    {
        var lst = new List<object>();
        
        lst.AddRange(GetWalletTypes());
        lst.AddRange(GetPaymentModes());

        return lst;
    }

    private static IEnumerable<WalletType> GetWalletTypes()
    {
        return new List<WalletType>
        {
            new() { Name = Translation.Cash },
            new() { Name = Translation.Saving },
            new() { Name = Translation.Main }
        };
    }

    private static IEnumerable<PaymentMode> GetPaymentModes()
    {
        return new List<PaymentMode>
        {
            new() { Name = Translation.Cash },
            new() { Name = Translation.Transfer },
            new() { Name = Translation.Card }
        };
    }
}