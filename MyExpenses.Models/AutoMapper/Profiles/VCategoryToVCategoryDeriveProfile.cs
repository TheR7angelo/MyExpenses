using AutoMapper;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class VCategoryToVCategoryDeriveProfile : Profile
{
    public VCategoryToVCategoryDeriveProfile()
    {
        CreateMap<VCategory, VCategoryDerive>();
    }
}