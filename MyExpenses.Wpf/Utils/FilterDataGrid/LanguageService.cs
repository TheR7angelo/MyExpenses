using System.Globalization;
using FilterDataGrid;

namespace MyExpenses.Wpf.Utils.FilterDataGrid;

public static class LanguageService
{
    public static Local ToLocal(this CultureInfo cultureInfo)
    {
        return cultureInfo.Name.ToLower() switch
        {
            "zh-tw" => Local.TraditionalChinese,
            "zh-cn" => Local.SimplifiedChinese,
            "nl-nl" => Local.Dutch,
            "en-us" => Local.English,
            "fr-fr" => Local.French,
            "de-de" => Local.German,
            "hu-hu" => Local.Hungarian,
            "it-it" => Local.Italian,
            "ja-jp" => Local.Japanese,
            "pl-pl" => Local.Polish,
            "ru-ru" => Local.Russian,
            "es-es" => Local.Spanish,
            "tr-tr" => Local.Turkish,
            _ => Local.English
        };
    }
}