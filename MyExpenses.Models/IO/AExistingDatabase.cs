using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Models.IO;

[ObservableObject]
public partial class AExistingDatabase(string filePath) : ExistingDatabase(filePath)
{
    public bool IsSelected
    {
        get;
        set => SetProperty(ref field, value);
    }
}