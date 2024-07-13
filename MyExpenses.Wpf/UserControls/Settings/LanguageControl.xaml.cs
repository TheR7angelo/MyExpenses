using System.Globalization;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class LanguageControl
{
    public List<CultureInfo> CultureInfos { get; } = [];

    public LanguageControl()
    {
        var localFilePathDataBaseModel = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(localFilePathDataBaseModel);
        foreach (var supportedLanguage in context.TSupportedLanguages)
        {
            var cultureInfo = new CultureInfo(supportedLanguage.Code);
            CultureInfos.Add(cultureInfo);
        }

        var z = new CultureInfo("pt-PT");
        CultureInfos.Add(z);

        InitializeComponent();
    }
}