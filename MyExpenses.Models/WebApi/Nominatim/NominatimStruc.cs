namespace MyExpenses.Maps.Test;

namespace MyExpenses.Models.WebApi.Nominatim;

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