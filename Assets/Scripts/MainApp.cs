using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.UI;
using System.Data;
using System;
using System.Net.NetworkInformation;

namespace MyWeather
{
    public class MainApp : MonoBehaviour
    {
        private const string SITE = "openweathermap.org";

        [Header("Cities Buttons")]
        [SerializeField]
        private CityButton[] cityButtons;
        [Header("Cities Profile")]
        [SerializeField]
        private CityButton[] cityProfileButtons;

        [Header("Current Weather")]
        [SerializeField]
        private Text cityNameText;
        [SerializeField]
        private Text tempText;
        [SerializeField]
        private Text feels_likeText;
        [SerializeField]
        private Text pressureText;
        [SerializeField]
        private Text humidityText;
        [SerializeField]
        private Text wind_speedText;
        [SerializeField]
        private Text wind_degText;
        [SerializeField]
        private Text cloudsText;
        [SerializeField]
        private Text sunriseText;
        [SerializeField]
        private Text sunsetText;
        [SerializeField]
        private Text timezoneText;



        public CityInfo[] cities;

        public WeatherResponse resp;

        private string apiKey;
        private string units; // UNITS: imperial ("&units=imperial");  metric ("&units=metric");  standard (""); 
        private string language;

        private string url;
        private string responseJson;

        private int profile;

        [SerializeField]
        private WeatherResponse weatherInCity;

        private Dictionary<int, WeatherResponse> citiesProfile;

        private DataTable tableFindCity;
        private DataTable tableSettings;
        private DataTable tableProfileCitiesId;
        private DataTable tableWeatherInCities;

        private IPStatus citeStatus;


        void Start()
        {
            Debug.Log(CheckSiteStatus()); // Check Network

            GetSavedSettings(); // LoadSettings()
            GetProfileInfo();
            ShowProfile();

            //Debug.Log(SqliteDataAccess.GetTable($"SELECT * FROM Weather WHERE id_city = 1") == null);




            //resp = GetWeather(); // Проверить, что будет если не будет интернета, посмотреть на сайте
        }

        private void ShowProfile()
        {
            HideProfileCityButtons();

            int i = 0;
            foreach (KeyValuePair<int, WeatherResponse> weather in citiesProfile)
            {
                cityProfileButtons[i].Show(weather.Value.id, weather.Value.name);
                i++;
            }
        }

        public void ShowWeather(int cityId)
        {
            weatherInCity = GetWeather(cityId);

            cityNameText.text = weatherInCity.name;
            tempText.text = weatherInCity.main.temp.ToString();
            feels_likeText.text = weatherInCity.main.feels_like.ToString();
            pressureText.text = weatherInCity.main.pressure.ToString();
            humidityText.text = weatherInCity.main.humidity.ToString();
            wind_speedText.text = weatherInCity.wind.speed.ToString();
            wind_degText.text = weatherInCity.wind.deg.ToString();
            cloudsText.text = weatherInCity.clouds.all.ToString();
            sunriseText.text = weatherInCity.sys.sunrise.ToString();
            sunsetText.text = weatherInCity.sys.sunset.ToString();
            timezoneText.text = weatherInCity.timezone.ToString();
        }

        private bool CheckSiteStatus()
        {
            citeStatus = IPStatus.Unknown;
            try
            {
                citeStatus = new System.Net.NetworkInformation.Ping().Send(SITE).Status;
            }
            catch { }

            if (citeStatus == IPStatus.Success)
            {
                Debug.Log("Сервер работает");
                return true;
            }
            else
            {
                Debug.Log("Сервер временно недоступен!");
                return false;
            }
        }

        private void GetSavedSettings()
        {
            tableSettings = SqliteDataAccess.GetTable("SELECT * FROM AppSettings");

            language = tableSettings.Rows[0][1].ToString();
            profile = Convert.ToInt32(tableSettings.Rows[0][2].ToString());
            units = tableSettings.Rows[0][3].ToString();
            apiKey = tableSettings.Rows[0][4].ToString();

            Debug.Log($"api:{apiKey} units:{units} language:{language} profile:{profile}");
        }

        private void GetProfileInfo()
        {
            if (citiesProfile != null) citiesProfile.Clear();
            else citiesProfile = new Dictionary<int, WeatherResponse>();

            tableWeatherInCities = SqliteDataAccess.GetTable("SELECT Weather.id_city, Name, State, Country, longitude, City.latitude, Temp, Feels_like, Pressure, Humidity, Wind_speed, Wind_deg, Clouds, Sunrise, Sunset, Timezone " +
                "FROM Weather, City " +
                "WHERE Weather.id_city = City.id_city AND EXISTS " +
                "(SELECT id_city " +
                "FROM WeatherProfile " +
                $"WHERE id_user_profile = {profile} AND Weather.id_city = WeatherProfile.id_city)");

            // if (tableWeatherInCities == null) // do something 

            for (int i = 0; i < tableWeatherInCities.Rows.Count; i++)
            {
                weatherInCity = new WeatherResponse(
                    Convert.ToInt32(tableWeatherInCities.Rows[i][0].ToString()), // cityId
                    tableWeatherInCities.Rows[i][1].ToString(), // name
                    tableWeatherInCities.Rows[i][2].ToString(), //state
                    tableWeatherInCities.Rows[i][3].ToString(), // country
                    Convert.ToSingle(tableWeatherInCities.Rows[i][4].ToString()), // longitude
                    Convert.ToSingle(tableWeatherInCities.Rows[i][5].ToString()), // latitude
                    Convert.ToSingle(tableWeatherInCities.Rows[i][6].ToString()), // temp
                    Convert.ToSingle(tableWeatherInCities.Rows[i][7].ToString()), // feels_like
                    Convert.ToInt32(tableWeatherInCities.Rows[i][8].ToString()),  // pressure
                    Convert.ToInt32(tableWeatherInCities.Rows[i][9].ToString()),  // humidity
                    Convert.ToSingle(tableWeatherInCities.Rows[i][10].ToString()), // wind_speed
                    Convert.ToInt32(tableWeatherInCities.Rows[i][11].ToString()),  // wind_deg
                    Convert.ToInt32(tableWeatherInCities.Rows[i][12].ToString()),  // clouds
                    Convert.ToInt32(tableWeatherInCities.Rows[i][13].ToString()),  // sunrise
                    Convert.ToInt32(tableWeatherInCities.Rows[i][14].ToString()),  // sunset
                    Convert.ToInt32(tableWeatherInCities.Rows[i][15].ToString())  // timezone
                    );

                citiesProfile.Add(weatherInCity.id, weatherInCity); // (cityID, WeatherInCity)
            }

        }

        private WeatherResponse GetWeatherFromFile(string path)
        {
            string weather = File.ReadAllText(path); // check existing file path

            return JsonUtility.FromJson<WeatherResponse>(weather);
        } // I think this no need more

        private WeatherResponse GetWeather(string cityName) 
        {
            url = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&lang={language}&units={units}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJson = streamReader.ReadToEnd();
            }

            //SaveWeatherInJSON(responseJson, "tempCity");

            return JsonUtility.FromJson<WeatherResponse>(responseJson);
        }

        private WeatherResponse GetWeather(int cityId) 
        {
            url = $"http://api.openweathermap.org/data/2.5/weather?id={cityId}&appid={apiKey}&lang={language}&units={units}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJson = streamReader.ReadToEnd();
            }

            return JsonUtility.FromJson<WeatherResponse>(responseJson);
        }

        private void SaveWeather(WeatherResponse weather)
        {
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery("INSERT INTO Weather(id_city, Temp, Feels_like, Pressure, Humidity, Wind_speed, Wind_deg, Clouds, Sunrise, Sunset, Timezone) " + 
                $"VALUES({weather.id}, '{weather.main.temp}', '{weather.main.feels_like}', {weather.main.pressure}, {weather.main.humidity}, '{weather.wind.speed}', {weather.wind.deg}, {weather.clouds.all}, {weather.sys.sunrise}, {weather.sys.sunset}, {weather.timezone}) " +
                $"ON CONFLICT(id_city) DO UPDATE SET Temp = '{weather.main.temp}', Feels_like = '{weather.main.feels_like}', Pressure = {weather.main.pressure}, Humidity = {weather.main.humidity}, Wind_speed = '{weather.wind.speed}', Wind_deg = {weather.wind.deg}, Clouds = {weather.clouds.all}, Sunrise = {weather.sys.sunrise}, Sunset = {weather.sys.sunset}, Timezone = {weather.timezone}");

            Debug.Log("Weather successfully saved");

            SqliteDataAccess.Close();
        }

        private void SaveCityInProfile(int cityID)
        {
            SqliteDataAccess.ConnectTo();

            // Need to check if row exists then no need save duplicate
            SqliteDataAccess.ExecuteQuery("INSERT INTO WeatherProfile(id_user_profile, id_city) " +
                $"VALUES({profile}, {cityID})");

            SqliteDataAccess.Close();
        }

        public void FindCity(string cityName) // Find City
        {
            HideCityButtons();

            tableFindCity = SqliteDataAccess.GetTable("SELECT * " +
                "FROM City " +
                $"WHERE name like  '%{cityName}%' " + // ORDER BY name
                "LIMIT 10");
            Debug.Log(tableFindCity.Rows.Count);
            ShowCityButtons(tableFindCity);
        }

        private void ShowCityButtons(DataTable tableCity)
        {
            for (int i = 0; i < tableCity.Rows.Count; i++)
            {
                cityButtons[i].Show(Convert.ToInt32(tableCity.Rows[i][0].ToString()), tableCity.Rows[i][1].ToString()); // Rows[i]["id_city"], Rows[i]["Name"]
            }
        }

        private void HideCityButtons()
        {
            for (int i = 0; i < cityButtons.Length; i++)
                cityButtons[i].Hide();
        }

        private void HideProfileCityButtons()
        {
            for (int i = 0; i < cityProfileButtons.Length; i++)
                cityProfileButtons[i].Hide();
        }

        void SaveWeatherInJSON(string weather, string path)
        {
            Debug.Log("file Saved = " + Application.dataPath + @"/" + path + ".json");
            File.WriteAllText(Application.dataPath + @"/" + path + ".json", weather);
        } // I think this no need more

        void GetCitiesInfoFromFile()
        {

            string citiesInfoJsonPath = Application.streamingAssetsPath + @"\city.list.json"; // Move
            string citiesInfoJson = File.ReadAllText(citiesInfoJsonPath);

            cities = JsonHelper.FromJson<CityInfo>(citiesInfoJson);
            Debug.Log("UpdateCitiesInfo");
        }

        void SaveCitiesInfoInDB()
        {
            string query;

            SqliteDataAccess.ConnectTo();
            for (int i = 0; i < cities.Length; i++)
            { 
                query = "INSERT INTO City (id_city, Name, State, Country, Longitude, latitude) " +
                    $"VALUES( {cities[i].id}, \"{cities[i].name.Replace("\"", "\"\"")}\", \"{cities[i].state}\", \"{cities[i].country}\", '{cities[i].coord.lon}', '{cities[i].coord.lat}')";
                SqliteDataAccess.ExecuteQuery(query);
            }
            SqliteDataAccess.Close();

            Debug.Log("Cities Saved!");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
