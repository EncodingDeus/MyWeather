using System;

namespace MyWeather
{
    [Serializable]
    public class WindInfo
    {
        public float speed; // Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour.
        public int deg; // Wind direction, degrees (meteorological)
    }
}