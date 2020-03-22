using System;

namespace MyWeather
{
    [Serializable]
    public class SystemInfo
    {
        public int type; // Internal parameter
        public int id; // Internal parameter
        public string message; // Internal parameter
        public string country; // Country code (GB, JP etc.)
        public int sunrise; // Sunrise time, unix, UTC
        public int sunset; // Sunset time, unix, UTC
    }
}