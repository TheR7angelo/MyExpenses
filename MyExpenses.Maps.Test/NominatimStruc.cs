namespace MyExpenses.Maps.Test;

public partial class Nominatim
{
    public struct NominatimStruc
    {
        public long place_id = 0;
        public string? licence = null;
        public string? osm_type = null;
        public long osm_id = 0;
        public float lat = 0;
        public float lon = 0;
        public string? display_name = null;
        public Address address = new ();
        public IEnumerable<float> boundingbox = Array.Empty<float>();

        public NominatimStruc()
        {
        }
    }
    
    public struct Address
    {
        public string? house_number = null;
        public string? road = null;
        public string? suburb = null;
        public string? city = null;
        public string? village = null;
        public string? municipality = null;
        public string? county = null;
        public string? state = null;
        public string? region = null;
        public long postcode = 0;
        public string? country = null;
        public string? country_code = null;

        public Address()
        {
        }
    }
}