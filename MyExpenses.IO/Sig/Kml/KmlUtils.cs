using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Linq;
using NetTopologySuite.IO.Esri.Dbf.Fields;

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
            var columnName = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name;
            if (string.IsNullOrEmpty(columnName)) continue;

            var value = propertyInfo.GetValue(obj);
            if (value is bool b) value = b ? "1" : "0";

            var xElement = new XElement(KmlNamespace + "SimpleData",
                new XAttribute("name", columnName), value);
            schemaDataElement.Add(xElement);
        }

        return extendedDataElement;
    }

    public static XElement CreateKmlSchema(this Dictionary<string, DbfField> fields, string schemaId)
    {
        var schemaElement = new XElement(KmlNamespace + "Schema",
            new XAttribute("name", schemaId),
            new XAttribute("id", schemaId)
        );

        foreach(var field in fields)
        {
            var type = Utils.GetDbFieldTypeMap[field.Value.FieldType];

            var fieldElement = new XElement(KmlNamespace + "SimpleField",
                new XAttribute("name", field.Value.Name),
                new XAttribute("type", type)
            );
            schemaElement.Add(fieldElement);
        }

        return schemaElement;
    }
}