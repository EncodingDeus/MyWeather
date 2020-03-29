using System;
using System.Collections.Generic;

namespace MyWeather
{
    [Serializable]
    public class WeatherForecastInfo
    {
        public MainInfo main; 
        public List<WeatherInfo> weather;
        public CloudsInfo clouds;
        public WindInfo wind;
        public string dt_txt;
    }
}