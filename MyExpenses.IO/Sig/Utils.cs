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
    public static string GetDbFieldType(DbfType dbfType)
    {
        return dbfType switch
        {
            DbfType.Numeric => "long",
            DbfType.Float => "double",
            DbfType.Character => "string",
            DbfType.Date => "DateTime",
            DbfType.Logical => "int",
            _ => throw new ArgumentOutOfRangeException(nameof(dbfType), dbfType, @"Unsupported DbfType")
        };
    }

    private static DbfField CreateFieldUsingSwitch(Type propertyType, string name, int? maxLength = null, int? precision = null)
    {
        // ReSharper disable HeapView.ObjectAllocation.Evident
        // The warning is disabled because the object allocations here are intentional and necessary.
        // Each case in the switch expression explicitly creates an instance of a `DbfField` or its derived types,
        // which inherently involves heap allocation since these objects need to exist independently.
        // This allocation is acceptable as these objects are lightweight and their lifespan aligns with the usage patterns.
        return propertyType switch
        {
            _ when propertyType == typeof(int) =>
                maxLength is null
                    ? new DbfNumericInt32Field(name)
                    : new DbfNumericInt32Field(name, maxLength.Value),

            _ when propertyType == typeof(long) =>
                maxLength is null
                    ? new DbfNumericInt64Field(name)
                    : new DbfNumericInt64Field(name, maxLength.Value),

            _ when propertyType == typeof(double) =>
                new DbfFloatField(name, maxLength ?? 19, precision ?? 0),

            _ when propertyType == typeof(string) =>
                new DbfCharacterField(name, maxLength ?? 255),

            _ when propertyType == typeof(DateTime) => new DbfDateField(name),
            _ when propertyType == typeof(bool) => new DbfLogicalField(name),

            _ => throw new ArgumentOutOfRangeException(nameof(propertyType), @$"Type not supported : {propertyType.Name}")
        };
        // ReSharper restore HeapView.ObjectAllocation.Evident
    }

    public static Dictionary<string, DbfField> GetFields(this Type type)
    {
        if (!typeof(ISig).IsAssignableFrom(type)) throw new ArgumentException(@"Type must implement ISig", nameof(type));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This warning is disabled because the allocation of a Dictionary<string, DbfField> is
        // both intentional and necessary to store the mapping between field names and the corresponding DbfField objects.
        // Dictionaries inherently require heap allocation since they are data structures designed for dynamic key-value storage.
        // In this context, this allocation has minimal impact on performance and is fundamental to the method's purpose.
        var fields = new Dictionary<string, DbfField>();

        foreach (var property in type.GetProperties())
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null || string.IsNullOrWhiteSpace(columnAttribute.Name)) continue;

            var name = columnAttribute.Name;
            if (name.Length > 10) name = name[..10];

            var maxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.Length;
            var precision = property.GetCustomAttribute<PrecisionAttribute>()?.Precision;

            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            fields[name] = CreateFieldUsingSwitch(propertyType, name, maxLength, precision);
        }

        return fields;
    }
}