using Mapsui;
using Mapsui.Layers;

namespace MyExpenses.Models.Mapsui.PointFeatures;

/// <summary>
/// Represents a temporary point feature used for handling temporary states on a map.
/// This class extends the functionality of the <see cref="PointFeature"/> class by including a temporary state property.
/// </summary>
public class TemporaryPointFeature : PointFeature
{
    /// <summary>
    /// Gets or sets a value indicating whether a point feature is temporary.
    /// When set to true, the feature is considered a temporary object, often used for transient operations or states.
    /// </summary>
    public bool IsTemp { get; set; }

    public TemporaryPointFeature(PointFeature pointFeature) : base(pointFeature)
    {
    }

    public TemporaryPointFeature(MPoint point) : base(point)
    {
    }
    //
    // public PointFeatureTemp(double x, double y) : base(x, y)
    // {
    // }
    //
    // public PointFeatureTemp((double x, double y) point) : base(point)
    // {
    // }
}