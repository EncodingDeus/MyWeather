using System;

namespace MyWeather
{
    [Serializable]
    public class MainInfo
    {
        public MainInfo(float temp, float feels_like, float temp_min, float temp_max, int pressure, int humidity, int sea_level, int grnd_level)
        {
            this.temp = temp;
            this.feels_like = feels_like;
            this.temp_min = temp_min;
            this.temp_max = temp_max;
            this.pressure = pressure;
            this.humidity = humidity;
            this.sea_level = sea_level;
            this.grnd_level = grnd_level;
        }

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
        public int sea_level;
        public int grnd_level;
        public int pressure;
        public int humidity;
    }
}
