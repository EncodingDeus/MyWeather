using UnityEngine;
using UnityEngine.UI;

namespace MyWeather
{
    public class CityButton : MonoBehaviour
    {
        [SerializeField]
        private Text cityNameText;
        [SerializeField]
        private Text cityTempText;

        [SerializeField]
        private MainApp mainApp;

        private int cityId;
        private string cityName;
        private float temp;


        public void Show(int id, string name, float temp)
        {
            this.cityId = id;
            this.cityName = name;
            this.temp = temp;

            cityNameText.text = name;
            cityTempText.text = temp.ToString();
            gameObject.SetActive(true);
        }

        public void Show(int id, string name)
        {
            this.cityId = id;
            this.cityName = name;

            cityNameText.text = name;
            gameObject.SetActive(true);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnClick()
        {
            Debug.Log($"id:{cityId} name:{cityName}");
        }

        public void ShowWeather()
        {
            mainApp.ShowCurrentWeather(cityId);
        }

        public void AddCityInProfile()
        {
            mainApp.AddCityInProfile(cityId);
        }

        public void DeleteCityFromProfile()
        {
            mainApp.RemoveCityFromProfile(cityId);
        }
    }
}