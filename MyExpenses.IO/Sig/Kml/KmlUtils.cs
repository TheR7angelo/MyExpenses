using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Linq;
using NetTopologySuite.IO.Esri.Dbf.Fields;

namespace MyExpenses.IO.Sig.Kml;

public static class KmlUtils
{
    public static XNamespace KmlNamespace => XNamespace.Get("http://www.opengis.net/kml/2.2");

    /// <summary>
    /// Creates a KML (Keyhole Markup Language) attribute representation for an object based on its properties,
    /// including an ExtendedData element and associated SchemaData elements.
    /// </summary>
    /// <param name="obj">The object whose properties will be converted to KML attributes.</param>
    /// <param name="schemaId">The schema ID used to reference the schema in the KML SchemaData.</param>
    /// <returns>A KML XElement containing the ExtendedData with SchemaData and SimpleData elements derived from the object's properties.</returns>
    public static XElement CreateKmlAttribute(this object obj, string schemaId)
    {
        // The creation of multiple XElement instances is necessary to construct
        // the KML file's XML tree structure.
        // Each XElement represents a unique XML node or tag
        // and cannot be reused due to the hierarchical nature of the XML document.
        // These allocations are required to ensure that the serialized KML adheres to the specification and maintains data integrity.
        // This approach guarantees the correct representation of KML elements with their
        // attributes and values, and is essential for proper functioning.

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var extendedDataElement = new XElement(KmlNamespace + "ExtendedData");

        // ReSharper disable HeapView.ObjectAllocation.Evident
        var schemaDataElement = new XElement(KmlNamespace + "SchemaData",
            new XAttribute("schemaUrl", $"#{schemaId}"));
        // ReSharper restore HeapView.ObjectAllocation.Evident

        extendedDataElement.Add(schemaDataElement);

        var propertiesInfo = obj.GetType().GetProperties();
        foreach (var propertyInfo in propertiesInfo)
        {
            var columnName = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name;
            if (string.IsNullOrEmpty(columnName)) continue;

            var value = propertyInfo.GetValue(obj);
            if (value is bool b) value = b ? "1" : "0";

            // ReSharper disable HeapView.ObjectAllocation.Evident
            var xElement = new XElement(KmlNamespace + "SimpleData",
                new XAttribute("name", columnName), value);
            // ReSharper restore HeapView.ObjectAllocation.Evident
            schemaDataElement.Add(xElement);
        }

        return extendedDataElement;
    }

    public static XElement CreateKmlSchema(this Dictionary<string, DbfField> fields, string schemaId)
    {
        // ReSharper disable HeapView.ObjectAllocation.Evident
        var schemaElement = new XElement(KmlNamespace + "Schema",
            new XAttribute("name", schemaId),
            new XAttribute("id", schemaId)
            // ReSharper restore HeapView.ObjectAllocation.Evident
        );

        foreach(var field in fields)
        {
            var type = Utils.GetDbFieldTypeMap[field.Value.FieldType];

            // ReSharper disable HeapView.ObjectAllocation.Evident
            var fieldElement = new XElement(KmlNamespace + "SimpleField",
                new XAttribute("name", field.Value.Name),
                new XAttribute("type", type)
                // ReSharper restore HeapView.ObjectAllocation.Evident
            );
            schemaElement.Add(fieldElement);
        }

        return schemaElement;
    }
}