using System;

namespace MyWeather
{
    [Serializable]
    public class WeatherInfo
    {
        public int id;
        public string main;
        public string description;
        public string icon;
    }
}