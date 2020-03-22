using System;

namespace MyWeather
{
    [Serializable]
    public class MainInfo
    {
        public float temp;
        public float feels_like;
        public float temp_min;
        public float temp_max;
        public int pressure;
        public int humidity;
    }
}
