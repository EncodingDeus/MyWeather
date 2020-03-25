using System;

namespace MyWeather
{
    [Serializable]
    public class CityCoordInfo
    {
        public CityCoordInfo(float lon, float lat)
        {
            this.lon = lon;
            this.lat = lat;
        }

        public float lon; // longitude
        public float lat; // latitude
    }
}
