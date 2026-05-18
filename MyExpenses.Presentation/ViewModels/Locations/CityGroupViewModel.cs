using System.Collections.ObjectModel;

namespace MyExpenses.Presentation.ViewModels.Locations;

public class CityGroupViewModel
{
    public string? City { get; set; }
    public ObservableCollection<PlaceViewModel>? Places { get; set; }
}