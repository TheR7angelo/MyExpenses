using CommandLine;
using MyExpenses.Models.Systems;

namespace MyExpenses.Utils.Systems;

public static class CommandLineUtility
{
    public static SSystems GetArguments(this string[] args)
    {
        var parser = new Parser(settings => settings.CaseInsensitiveEnumValues = true);

        var result = new SSystems();

        parser.ParseArguments<Options>(args)
            .WithParsed(o =>
                {
                    result.EnvironmentType = o.Environment;
                    result.DebugLevel = o.DebugLevel;
                }
            );

        return result;
    }
}