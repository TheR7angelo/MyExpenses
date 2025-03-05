using CommandLine;
using MyExpenses.Models.Systems;

namespace MyExpenses.Utils.Systems;

public static class CommandLineUtility
{
    public static SSystems GetArguments(this string[] args)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The parser is from the CommandLineParser library and needs to be instantiated here
        // because each instance can have its own specific configuration.
        var parser = new Parser(settings => settings.CaseInsensitiveEnumValues = true);

        // ReSharper disable once HeapView.ClosureAllocation
        var result = new SSystems();

        parser.ParseArguments<Options>(args)
            // ReSharper disable once HeapView.DelegateAllocation
            .WithParsed(o =>
                {
                    result.LogEventLevel = o.DebugLevel;
                    result.LogEfCore = o.LogEfCore;
                    result.WriteToFileEfCore = o.WriteToFileEfCore;
                }
            );

        return result;
    }
}