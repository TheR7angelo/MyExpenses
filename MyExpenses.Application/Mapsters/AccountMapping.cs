using Mapster;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.ViewModels.Accounts;

namespace MyExpenses.Application.Mapsters;

public class AccountMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TotalByAccountDto, TotalByAccountViewModel>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Total, src => src.Total)
            .Map(dest => dest.TotalPointed, src => src.TotalPointed)
            .Map(dest => dest.TotalNotPointed, src => src.TotalNotPointed)
            .Map(dest => dest.Symbol, src => src.Symbol)
            .TwoWays()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
    }
}