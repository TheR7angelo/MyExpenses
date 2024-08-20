using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyExpenses.IO.Csv;
using MyExpenses.IO.Excel;
using MyExpenses.IO.Sig.GeoJson;
using MyExpenses.IO.Sig.Kml;
using MyExpenses.IO.Sig.Shp;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.IO.Export;
using MyExpenses.Models.IO.Export.Sql.Tables;
using MyExpenses.Models.IO.Sig.Interfaces;
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
            var exportRecord = new ExportRecord { Name = name, Source = table, Records = records };
            exportRecords.Add(exportRecord);
        }

        exportRecords.ToExcelWorksheet("test");
        foreach (var exportRecord in exportRecords)
        {
            var name = exportRecord.Name;
            var records = exportRecord.Records;

            var isGeom = exportRecord.Source.GetInterfaces().Contains(typeof(ISig));
            if (isGeom)
            {
                var recordGeoms = records.Where(x => x is ISig)
                    .Cast<ISig>()
                    .Where(s => s.Geometry is not null)
                    .ToList();

                var geomType = recordGeoms.First().Geometry!.GetType().Name;

                var projection = ShapeReader.GeographicCoordinateSystems.First(s => s.WellKnownId is 4326);

                recordGeoms.ToKmlFile($"{name}.kml", geomType);
                recordGeoms.ToKmlFile($"{name}.kmz", geomType);
                recordGeoms.ToShapeFile(name, projection.Prj);
                recordGeoms.ToGeoJson(name);
            }
            else
            {
                records.WriteCsv(name);
            }
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