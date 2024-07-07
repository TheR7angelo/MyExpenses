using NetTopologySuite.Geometries;

namespace MyExpenses.Models.IO.Sig.Interfaces;

public interface ISig
{
    public Geometry? Geometry { get; set; }
}