using System;
using System.Collections.Generic;

namespace MyWeather
{
    [Serializable]
    public class WeatherForecastResponse
    {
        public int cnt;
        public List<WeatherForecastInfo> list;
        public CityInfo city;
    }
}