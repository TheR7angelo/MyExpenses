using PropertyChanged;

namespace MyExpenses.Models.IO;

[AddINotifyPropertyChangedInterface]
public class AExistingDatabase(string filePath) : ExistingDatabase(filePath)
{
    public bool IsSelected { get; set; }
}