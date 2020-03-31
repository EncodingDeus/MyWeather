using System;
using System.Collections.Generic;

namespace MyWeather
{
    [Serializable]
    public class WeatherForecastInfo
    {
        public WeatherForecastInfo(float temp, float feel_like, float temp_min, float temp_max, int pressure, int humidity, 
            int sea_level, int grnd_level, string weather_main, string weather_description, string weather_icon, float wind_speed, int wind_deg, int clouds, string dt_txt)
        {
            this.main = new MainInfo(temp, feel_like, temp_min, temp_max, pressure, humidity, sea_level, grnd_level);
            this.weather = new List<WeatherInfo>();
            this.weather.Add(new WeatherInfo(weather_main, weather_description, weather_icon));
            this.clouds = new CloudsInfo(clouds);
            this.wind = new WindInfo(wind_speed, wind_deg);
            this.dt_txt = dt_txt;
        }

        public MainInfo main; 
        public List<WeatherInfo> weather;
        public CloudsInfo clouds;
        public WindInfo wind;
        public string dt_txt;
    }
}