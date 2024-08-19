using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyExpenses.IO.Csv;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.IO.Export;
using MyExpenses.Models.IO.Export.Sql.Tables;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.IO.Test.Sig.Csv;

public class CsvWriter
{
    [Fact]
    public void Test()
    {
        using var context = new DataBaseContext();

        var tables = context.Model.GetEntityTypes()
            .SelectMany(s => s.GetTableMappings())
            .Select(s => s.TypeBase)
            .Select(s => s.ClrType)
            .ToList();

        var methodInfo = typeof(DataBaseContext)
            .GetMethods()
            .First(m => m is { Name: "Set", IsGenericMethod: true } && m.GetParameters().Length == 0);

        var mapper = Mapping.Mapper;

        var typeSwitch = new TypeSwitch()
            .Case<TAccount>(s => mapper.Map<ExportTAccount>(s))
            .Case<TAccountType>(s => mapper.Map<ExportTAccountType>(s))
            .Case<TBankTransfer>(s => mapper.Map<ExportTBankTransfer>(s))
            .Case<TCategoryType>(s => mapper.Map<ExportTCategoryType>(s))
            .Case<TColor>(s => mapper.Map<ExportTColor>(s))
            .Case<TCurrency>(s => mapper.Map<ExportTCurrency>(s))
            .Case<TGeometryColumn>(s => mapper.Map<ExportTGeometryColumn>(s))
            .Case<THistory>(s => mapper.Map<ExportTHistory>(s))
            .Case<TModePayment>(s => mapper.Map<ExportTModePayment>(s))
            .Case<TPlace>(s => mapper.Map<ExportTPlace>(s))
            .Case<TRecursiveExpense>(s => mapper.Map<ExportTRecursiveExpense>(s))
            .Case<TRecursiveFrequency>(s => mapper.Map<ExportTRecursiveFrequency>(s))
            .Case<TSpatialRefSy>(s => mapper.Map<ExportTSpatialRefSy>(s))
            .Case<TSupportedLanguage>(s => mapper.Map<ExportTSupportedLanguage>(s))
            .Case<TVersion>(s => mapper.Map<ExportTVersion>(s));

        var exportRecords = new List<ExportRecord>();
        foreach (var table in tables)
        {
            var genericSetMethod = methodInfo.MakeGenericMethod(table);
            dynamic dynamicRecords = genericSetMethod.Invoke(context, null)!;

            var records = new List<object?>();
            foreach (var dynamicRecord in dynamicRecords)
            {
                var tmp = typeSwitch.Switch(dynamicRecord);
                records.Add(tmp);
            }

            var name = table.GetCustomAttribute<TableAttribute>()!.Name;
            exportRecords.Add(new ExportRecord { Name = name, Records = records });
        }

        foreach (var exportRecord in exportRecords)
        {
            var records = exportRecord.Records;
            records.WriteCsv(exportRecord.Name);
        }
    }
}

public class TypeSwitch
{
    private readonly Dictionary<Type, Func<object, object>> _matches = new();

    public TypeSwitch Case<T>(Func<T, object> action)
    {
        _matches.Add(typeof(T), (x) => action((T)x));
        return this;
    }

    public object Switch(object x)
    {
        return _matches[x.GetType()](x);
    }
}