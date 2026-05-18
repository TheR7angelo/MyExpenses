using System.Collections.ObjectModel;

namespace MyExpenses.Presentation.ViewModels.Locations;

public class CountryGroupViewModel
{
    public string? Country { get; set; }
    public ObservableCollection<CityGroupViewModel>? CityGroups { get; set; }
}