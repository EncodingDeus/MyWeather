using System;

namespace MyWeather
{
    [Serializable]
    public class WeatherResponse
    {
        public CityCoordInfo coord; // City geo location, longitude and latitude
        public string[] weather; // more info Weather condition codes // Now don't working

        public string Base; // Internal parameter // should be "base". Now don't working

        public MainInfo main;
        public int visibility; // idk what is this
        public WindInfo wind;
        public CloudsInfo clouds;
        public int dt; // Time of data calculation, unix, UTC
        public SystemInfo sys;
        public int timezone; // Shift in seconds from UTC
        public int id; // City ID
        public string name; // City name
        public int cod; // internal parameter
    }
}
