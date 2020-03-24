using System;

namespace MyWeather
{
    [Serializable]
    public class MainInfo
    {
        public MainInfo(float temp, float feels_like, int pressure, int humidity)
        {
            this.temp = temp;
            this.feels_like = feels_like;
            this.pressure = pressure;
            this.humidity = humidity;
        }

        public float temp;
        public float feels_like;
        public float temp_min;
        public float temp_max;
        public int pressure;
        public int humidity;
    }
}
