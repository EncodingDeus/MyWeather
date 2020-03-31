using System;

namespace MyWeather
{
    [Serializable]
    public class WindInfo
    {
        public WindInfo(float speed, int deg)
        {
            this.speed = speed;
            this.deg = deg;
        }

        public float speed; // Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour.
        public int deg; // Wind direction, degrees (meteorological)
    }
}