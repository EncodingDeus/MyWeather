using System;

namespace MyWeather
{
    [Serializable]
    public class WeatherInfo
    {
        public WeatherInfo(string main, string description, string icon)
        {
            this.main = main;
            this.description = description;
            this.icon = icon;
        }

        public int id;
        public string main;
        public string description;
        public string icon;
    }
}