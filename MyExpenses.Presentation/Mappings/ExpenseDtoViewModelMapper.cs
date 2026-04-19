using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper(UseDeepCloning = true)]
public partial class ExpenseDtoViewModelMapper(ISystemDtoViewModelMapper mapper) : IExpenseDtoViewModelMapper
{
    public partial CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);

    [MapperIgnoreSource(nameof(CategoryTypeViewModel.HasErrors))]
    public partial CategoryTypeDto MapToDto(CategoryTypeViewModel src);

    public partial CategoryTypeViewModel Clone(CategoryTypeViewModel categoryTypeViewModel);

    private ColorDto MapToDto(ColorViewModel source)
    {
        var target = mapper.MapToDto(source);
        return target;
    }
}