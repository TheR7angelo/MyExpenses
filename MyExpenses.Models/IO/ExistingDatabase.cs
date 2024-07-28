namespace MyExpenses.Models.IO;

public class ExistingDatabase
{
    public string FilePath { get; private set; }
    public string FileName { get; private set; }
    public string FileNameWithoutExtension { get; private set; }
    public FileInfo FileInfo { get; private set; }

    public ExistingDatabase(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
        FileInfo = new FileInfo(filePath);
    }

}