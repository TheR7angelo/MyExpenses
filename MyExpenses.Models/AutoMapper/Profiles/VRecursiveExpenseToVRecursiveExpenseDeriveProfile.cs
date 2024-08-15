using AutoMapper;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class VRecursiveExpenseToVRecursiveExpenseDeriveProfile : Profile
{
    public VRecursiveExpenseToVRecursiveExpenseDeriveProfile()
    {
        CreateMap<VRecursiveExpense, VRecursiveExpenseDerive>();
    }
}