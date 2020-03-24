﻿using System;

namespace MyWeather
{
    [Serializable]
    public class WeatherResponse
    {
        public WeatherResponse(int cityID, float temp, float feels_like, int preassure, int humidity, float windSpeed, int windDeg, int clouds, int sunrise, int sunset, int timezone)
        {
            this.id = cityID;
            this.main = new MainInfo(temp, feels_like, preassure, humidity);
            this.wind = new WindInfo(windSpeed, windDeg);
            this.clouds = new CloudsInfo(clouds);
            this.sys = new SystemInfo(sunrise, sunset);
            this.timezone = timezone;
        }

        public CityCoordInfo coord; // City geo location, longitude and latitude
        public string[] weather; // more info Weather condition codes // Now don't working

        public string Base; // Internal parameter // should be "base". Now don't working

        public MainInfo main;
        public int visibility; // idk what is this
        public WindInfo wind;
        public CloudsInfo clouds;
        public int dt; // Time of data calculation, unix, UTC
        public SystemInfo sys;
        public int timezone; // Shift in seconds from UTC
        public int id; // City ID
        public string name; // City name
        public int cod; // internal parameter
    }
}
