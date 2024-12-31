using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;

namespace MyExpenses.IO.Sig;

public static class Utils
{
    public static Dictionary<DbfType, string> GetDbFieldTypeMap { get; } = new()
    {
        { DbfType.Numeric, "long" },
        { DbfType.Float, "double" },
        { DbfType.Character, "string" },
        { DbfType.Date, "DateTime" },
        { DbfType.Logical, "bool" }
    };

    private static Dictionary<Type, Func<string, int?, int?, DbfField>> FieldCreators { get; } = new()
    {
        [typeof(int)] = (name, maxLength, _) =>
            maxLength == null ? new DbfNumericInt32Field(name) : new DbfNumericInt32Field(name, maxLength.Value),
        [typeof(long)] = (name, maxLength, _) =>
            maxLength == null ? new DbfNumericInt64Field(name) : new DbfNumericInt64Field(name, maxLength.Value),
        [typeof(double)] = (name, maxLength, precision) =>
            new DbfFloatField(name, maxLength ?? 19, precision ?? 0),
        [typeof(string)] = (name, maxLength, _) =>
            new DbfCharacterField(name, maxLength ?? 255), // Default 255 characters max
        [typeof(DateTime)] = (name, _, _) => new DbfDateField(name),
        [typeof(bool)] = (name, _, _) => new DbfLogicalField(name)
    };

    public static Dictionary<string, DbfField> GetFields(this ISig feature, bool limitColumnNameSize = true)
    {
        if (feature is null) throw new ArgumentNullException(nameof(feature), @"Feature cannot be null");

        var fields = new Dictionary<string, DbfField>();

        foreach (var property in feature.GetType().GetProperties())
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null || string.IsNullOrWhiteSpace(columnAttribute.Name)) continue;

            var name = columnAttribute.Name;
            if (limitColumnNameSize && name.Length > 10) name = name[..10];

            var maxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.Length;
            var precision = property.GetCustomAttribute<PrecisionAttribute>()?.Precision;

            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (!FieldCreators.TryGetValue(propertyType, out var fieldCreator))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(propertyType),
                    @$"Type not supported for the property '{property.Name}' : {propertyType.Name}"
                );
            }

            fields[name] = fieldCreator(name, maxLength, precision);
        }

        return fields;
    }
}