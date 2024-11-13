using AutoMapper;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Derivatives.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class TAccountToTAccountDeriveProfile : Profile
{
    public TAccountToTAccountDeriveProfile()
    {
        CreateMap<TAccount, TAccountDerive>();
    }
}