using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyWeather
{
    public class WeatherCard : MonoBehaviour
    {
        [SerializeField]
        private MainApp mainApp;

        [SerializeField]
        private Text DateText;
        [SerializeField]
        private Text TimeText;
        [SerializeField]
        private Text TempText;
        [SerializeField]
        private Text CloudsText;
        [SerializeField]
        private Text WindSpeedText;
        [SerializeField]
        private Text PressureText;
        [SerializeField]
        private Text HumidityText;

        private WeatherForecastInfo weather;

        public void ShowCard(WeatherForecastInfo weather)
        {
            DateTime dateTime = Convert.ToDateTime(weather.dt_txt);

            this.weather = weather;

            DateText.text = dateTime.DayOfWeek.ToString();
            TimeText.text = dateTime.ToShortTimeString();
            TempText.text = weather.main.temp.ToString();
            CloudsText.text = weather.clouds.all.ToString();
            WindSpeedText.text = weather.wind.speed.ToString();
            PressureText.text = weather.main.pressure.ToString();
            HumidityText.text = weather.main.humidity.ToString();
        }

        public void ShowFullWeather()
        {
            mainApp.ShowFullWeatherByCard(weather);
        }
    }
}