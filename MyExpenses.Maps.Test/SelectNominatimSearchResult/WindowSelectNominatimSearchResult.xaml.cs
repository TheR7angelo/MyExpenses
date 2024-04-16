using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Maps.Test.SelectNominatimSearchResult;

public partial class WindowSelectNominatimSearchResult
{
    public ObservableCollection<TPlace> Places { get; } = [];

    public WindowSelectNominatimSearchResult()
    {
        InitializeComponent();
    }

    public void AddRange(IEnumerable<TPlace> places)
    {
        foreach (var place in places)
        {
            Places.Add(place);
        }
    }
}