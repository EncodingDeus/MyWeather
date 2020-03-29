using System;
using System.Collections.Generic;

namespace MyWeather
{
    [Serializable]
    public class WeatherForecastInfo
    {
        public int dt;
        public MainInfo main; 
        public List<WeatherInfo> weather;
        public WindInfo wind;
        public SystemInfo sys;
        public string dt_txt;
    }
}