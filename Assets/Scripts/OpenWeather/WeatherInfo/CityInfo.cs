using System;

namespace MyWeather
{
    [Serializable]
    public class CityInfo
    {
        public int id;
        public string name;
        public string state;
        public string country;
        public CityCoordInfo coord;
    }
}