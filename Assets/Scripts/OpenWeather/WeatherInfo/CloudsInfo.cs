using System;

namespace MyWeather
{
    [Serializable]
    public class CloudsInfo
    {
        public CloudsInfo (int percent)
        {
            this.all = percent;
        }

        public int all;
    }
}