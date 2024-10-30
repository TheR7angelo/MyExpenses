using BruTile.Predefined;

namespace MyExpenses.Utils.Maps;

public static class MapsuiMapExtensions
{
    public static IEnumerable<KnownTileSource> GetAllKnowTileSource()
    {
        var blackList = new List<KnownTileSource>
        {
            KnownTileSource.OpenCycleMap, KnownTileSource.OpenCycleMapTransport,
            KnownTileSource.StamenToner, KnownTileSource.StamenTonerLite, KnownTileSource.StamenWatercolor, KnownTileSource.StamenTerrain,
            KnownTileSource.EsriWorldReferenceOverlay, KnownTileSource.EsriWorldBoundariesAndPlaces,
            KnownTileSource.HereHybrid, KnownTileSource.HereTerrain
        };
        var knownTileSources = Enum.GetValues<KnownTileSource>().Where(s => !blackList.Contains(s)).ToList();
        return knownTileSources;
    }
}