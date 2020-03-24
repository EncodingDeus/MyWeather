using UnityEngine;
using UnityEngine.UI;

namespace MyWeather
{
    public class CityButton : MonoBehaviour
    {
        [SerializeField]
        private Text buttonText;

        [SerializeField]
        private MainApp mainApp;

        private int id;
        private string name;

        public void Show(int id, string name)
        {
            this.id = id;
            this.name = name;

            buttonText.text = name;
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
            Debug.Log($"id:{id} name:{name}");
        }

        public void ShowWeather()
        {
            mainApp.ShowWeather(id);
        }
    }
}