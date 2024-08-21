using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.IO;
using MyExpenses.Models.IO.Export;
using MyExpenses.Models.IO.Export.Sql.Tables;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Switch;

namespace MyExpenses.Core.Export;

public static class Common
{
    public static List<ExportRecord> GetExportRecords(this ExistingDatabase existingDatabase)
    {
        using var context = new DataBaseContext(existingDatabase.FilePath);

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
            var exportRecord = new ExportRecord { Name = name, Source = table, Records = records };
            exportRecords.Add(exportRecord);
        }

        return exportRecords;
    }
}