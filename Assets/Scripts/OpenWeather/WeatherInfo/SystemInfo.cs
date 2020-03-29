using System;

namespace MyWeather
{
    [Serializable]
    public class SystemInfo
    {
        public SystemInfo(string country, string state, int sunrise, int sunset)
        {
            this.country = country;
            this.state = state;
            this.sunrise = sunrise;
            this.sunset = sunset;
        }

        public int type; // Internal parameter
        public int id; // Internal parameter
        public string message; // Internal parameter
        public string state; // City state
        public string country; // Country code (GB, JP etc.)
        public int sunrise; // Sunrise time, unix, UTC
        public int sunset; // Sunset time, unix, UTC
    }
}