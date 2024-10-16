using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Models.Sig;

namespace MyExpenses.IO.Sig.Kml;

public static class KmlUtils
{
    public static XNamespace KmlNamespace => XNamespace.Get("http://www.opengis.net/kml/2.2");

    public static XElement CreateKmlAttribute(this object obj, string schemaId)
    {
        var extendedDataElement = new XElement(KmlNamespace + "ExtendedData");

        var schemaDataElement = new XElement(KmlNamespace + "SchemaData",
            new XAttribute("schemaUrl", $"#{schemaId}"));

        extendedDataElement.Add(schemaDataElement);

        var propertiesInfo = obj.GetType().GetProperties();
        foreach (var propertyInfo in propertiesInfo)
        {
            if (propertyInfo.GetValueByProperty<ColumnAttribute>() is not string columnName) continue;
            var value = propertyInfo.GetValue(obj);

            var xElement = new XElement(KmlNamespace + "SimpleData",
                new XAttribute("name", columnName), value);
            schemaDataElement.Add(xElement);
        }

        return extendedDataElement;
    }

    public static XElement CreateKmlSchema(this Dictionary<string, DbField> fields, string schemaId)
    {
        var schemaElement = new XElement(KmlNamespace + "Schema",
            new XAttribute("name", schemaId),
            new XAttribute("id", schemaId)
        );

        foreach(var field in fields)
        {
            var fieldElement = new XElement(KmlNamespace + "SimpleField",
                new XAttribute("name", field.Value.Name),
                new XAttribute("type", GetKmlType(field.Value.Type))
            );
            schemaElement.Add(fieldElement);
        }

        return schemaElement;
    }

    private static string GetKmlType(Type type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Int32 => "int",
            TypeCode.Int64 => "long",
            _ => type.Name.ToLower()
        };
    }

    public static Dictionary<string, DbField> GetISigFields(this Type type)
    {
        var propertiesInfo = type.GetProperties();
        var fields = new Dictionary<string, DbField>();

        foreach (var propertyInfo in propertiesInfo)
        {
            if (propertyInfo.GetValueByProperty<ColumnAttribute>() is not string columnName) continue;
            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            var dbField = new DbField
            {
                Name = columnName,
                Type = underlyingType
            };

            fields[columnName] = dbField;
        }

        return fields;
    }

    public static object? GetValueByProperty<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
    {
        var attribute = property.GetCustomAttribute<TAttribute>();

        return attribute switch
        {
            ColumnAttribute columnAttribute => columnAttribute.Name,
            MaxLengthAttribute maxLengthAttribute => maxLengthAttribute.Length,
            PrecisionAttribute precisionAttribute => precisionAttribute.Precision,
            _ => null
        };
    }
}