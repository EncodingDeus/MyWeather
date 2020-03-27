using System;
using System.Collections.Generic;

namespace MyWeather
{
    [Serializable]
    public class WeatherForecastInfo
    {
        public int dt;
        public MainInfo main; // добавить доп. поля, проверить на совместимость с WeatherResponse
        public List<WeatherInfo> weather;
        public WindInfo wind;
        public SystemInfo sys; // add field pod (check what is it)
        public string dt_txt;
    }
}