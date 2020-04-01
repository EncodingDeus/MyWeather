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
        private TimeSpan timeToRequest = new TimeSpan(12, 0, 0); // 12 hours

        [Header("Cities Buttons")]
        [SerializeField]
        private CityButton[] cityButtons;
        [Header("Cities Profile")]
        [SerializeField]
        private CityButton[] cityProfileButtons;

        [Header("Current Weather")]
        [SerializeField]
        private GameObject panelWeather;

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

        [Header("Weather Forecast")]
        [SerializeField]
        private GameObject panelWeatherForecast;
        [SerializeField]
        private WeatherCard[] weatherCards;

        [SerializeField]
        private Text cityNameForecastText;
        [SerializeField]
        private Text dateForecastText;
        [SerializeField]
        private Text dayOfWeekForecastText;
        [SerializeField]
        private Text timeForecastText;
        [SerializeField]
        private Text tempForecastText;
        [SerializeField]
        private Text tempMinForecastText;
        [SerializeField]
        private Text tempMaxForecastText;
        [SerializeField]
        private Text feels_likeForecastText;
        [SerializeField]
        private Text seaLevelForecastText;
        [SerializeField]
        private Text grndLevelForecastText;
        [SerializeField]
        private Text pressureForecastText;
        [SerializeField]
        private Text humidityForecastText;
        [SerializeField]
        private Text windSpeedForecastText;
        [SerializeField]
        private Text windDegForecastText;
        [SerializeField]
        private Text cloudsForecastText;

        [Header("Offline mode")]
        [SerializeField]
        private Animator offlinePanelAnimator;

        [Space(16)]

        private string apiKey;
        private string units;
        private string language;
        private string url;
        private string responseJson;

        private int profile;

        private bool serverStatus = false;

        private WeatherResponse currentWeatherResp;

        private WeatherForecastResponse weatherForecastResp;

        private Dictionary<int, string> citiesList;
        private Dictionary<int, WeatherResponse> citiesProfile;

        private DataTable tableFindCity;
        private DataTable tableSettings;
        private DataTable tableProfileCitiesId;
        private DataTable tableWeatherInCities;
        private DataTable tableProfileCities;
        private DataTable tableProfile;

        private IPStatus siteStatus;


        void Start()
        {
            LoadApplicationSettings();
            serverStatus = CheckServerStatus();
            LoadProfileSettings();
            LoadCurrentWeatherFromProfile();
            ShowProfileCities();
        }

        /// <summary>Loads cities to the list of cities and receives a weather forecast if there is a connection with the server</summary>
        private void LoadProfileSettings()
        {
            // Load cities
            tableProfileCities = SqliteDataAccess.GetTable("SELECT WeatherProfile.id_city, Name, Datetime_request " +
                "FROM WeatherProfile, City " + 
                $"WHERE id_user_profile = {profile} AND WeatherProfile.id_city == City.id_city");

            citiesList = new Dictionary<int, string>();

            for (int i = 0; i < tableProfileCities.Rows.Count; i++)
            {
                int cityId = Convert.ToInt32(tableProfileCities.Rows[i][0]);
                string cityName = tableProfileCities.Rows[i][1].ToString();
                DateTime datetimeRequest = Convert.ToDateTime(tableProfileCities.Rows[i][2]);

                citiesList.Add(cityId, cityName);

                if (serverStatus)
                {
                    // Check weather forecast actuality
                    if ((DateTime.Now - datetimeRequest) >= timeToRequest)
                    {
                        SaveWeatherForecastInDB(GetWeatherForecast(cityId));
                    }
                    // Get and Save current weather
                    SaveCurrentWeather(GetCurrentWeather(cityId));
                }
            }
        }

        /// <summary>Show city list on the screen</summary>
        private void ShowProfileCities()
        {
            HideProfileCities();

            int i = 0;
            foreach (KeyValuePair<int, WeatherResponse> weather in citiesProfile)
            {
                cityProfileButtons[i].Show(weather.Value.id, weather.Value.name, weather.Value.main.temp);
                i++;
            }

            cityProfileButtons[0].ShowWeather();
        }

        /// <summary>Hide city list from the screen</summary>
        private void HideProfileCities()
        {
            for (int i = 0; i < cityProfileButtons.Length; i++)
                cityProfileButtons[i].Hide();
        }

        /// <summary>Show current weather by city id on the screen</summary>
        public void ShowCurrentWeather(int cityId)
        {
            long utcSunriseDate = currentWeatherResp.sys.sunrise;
            long utcSunsetDate = currentWeatherResp.sys.sunset;
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime sunriseDate = start.AddSeconds(utcSunriseDate).ToLocalTime();
            DateTime sunsetDate = start.AddSeconds(utcSunsetDate).ToLocalTime();
            TimeSpan timezone = new TimeSpan(0, 0, 0, currentWeatherResp.timezone);

            currentWeatherResp = citiesProfile[cityId];

            cityNameText.text = currentWeatherResp.name;
            tempText.text = currentWeatherResp.main.temp.ToString();
            feels_likeText.text = currentWeatherResp.main.feels_like.ToString();
            pressureText.text = currentWeatherResp.main.pressure.ToString();
            humidityText.text = currentWeatherResp.main.humidity.ToString();
            wind_speedText.text = currentWeatherResp.wind.speed.ToString();
            wind_degText.text = currentWeatherResp.wind.deg.ToString();
            cloudsText.text = currentWeatherResp.clouds.all.ToString();
            sunriseText.text = sunriseDate.ToShortTimeString();
            sunsetText.text = sunsetDate.ToShortTimeString();

            ShowWeatherForecast(cityId);
        }

        /// <summary>Show weather forecast by city id on the screen</summary>
        public void ShowWeatherForecast(int cityId)
        {
            weatherForecastResp = LoadWeatherForecastFromDBbyCityId(cityId);

            for (int i = 0; i < weatherCards.Length; i++)
            {
                weatherCards[i].ShowCard(weatherForecastResp.list[i]);
            } 
        }

        /// <summary>Show full info about weather forecast</summary>
        public void ShowFullWeatherByCard(WeatherForecastInfo weather)
        {

            DateTime dateTime = Convert.ToDateTime(weather.dt_txt);

            cityNameForecastText.text = "";

            dateForecastText.text = dateTime.ToShortDateString();

            dayOfWeekForecastText.text = dateTime.DayOfWeek.ToString();

            timeForecastText.text = dateTime.TimeOfDay.ToString();

            tempForecastText.text = weather.main.temp.ToString();

            tempMinForecastText.text = weather.main.temp_min.ToString();

            tempMaxForecastText.text = weather.main.temp_max.ToString();

            feels_likeForecastText.text = weather.main.feels_like.ToString();

            seaLevelForecastText.text = weather.main.sea_level.ToString();

            grndLevelForecastText.text = weather.main.grnd_level.ToString();

            pressureForecastText.text = weather.main.pressure.ToString();

            humidityForecastText.text = weather.main.humidity.ToString();

            windSpeedForecastText.text = weather.wind.speed.ToString();

            windDegForecastText.text = weather.wind.deg.ToString();

            cloudsForecastText.text = weather.clouds.all.ToString();
        }

        /// <summary>Add city in profile by city id. if a connection with the server is lost, return void</summary>
        public void AddCityInProfile(int cityId)
        {
            if (citiesList.ContainsKey(cityId)) return;
            serverStatus = CheckServerStatus();
            if (!serverStatus) return;

            SaveCurrentWeather(GetCurrentWeather(cityId));
            SaveWeatherForecastInDB(GetWeatherForecast(cityId));
            SaveCityInProfile(cityId);

            LoadProfileSettings();
            LoadCurrentWeatherFromProfile();
            ShowProfileCities();
        }

        /// <summary>Remove city from profile by city id </summary>
        public void RemoveCityFromProfile(int cityId)
        {
            if (citiesList.Count == 1) return;

            DeleteWeatherForecastFromDB(cityId);
            DeleteCurrentWeatherFromDB(cityId);
            DeleteCityFromProfile(cityId);

            ShowProfileCities();
        }

        /// <summary>Check server status</summary>
        private bool CheckServerStatus()
        {
            siteStatus = IPStatus.Unknown;
            try
            {
                siteStatus = new System.Net.NetworkInformation.Ping().Send(SITE).Status;
            }
            catch { }

            if (siteStatus == IPStatus.Success)
            {
                offlinePanelAnimator.SetBool("Show", false);
                return true;
            }
            else
            {
                offlinePanelAnimator.SetBool("Hide", true);
                return false;
            }
        }

        /// <summary>Load application settings</summary>
        private void LoadApplicationSettings()
        {
            tableSettings = SqliteDataAccess.GetTable("SELECT * FROM AppSettings");

            language = tableSettings.Rows[0][1].ToString();
            profile = Convert.ToInt32(tableSettings.Rows[0][2].ToString());
            units = tableSettings.Rows[0][3].ToString();
            apiKey = tableSettings.Rows[0][4].ToString();

            Debug.Log($"api:{apiKey} units:{units} language:{language} profile:{profile}");
        }

        /// <summary>Load current weather all cities by profile from database</summary>
        private void LoadCurrentWeatherFromProfile()
        {
            if (citiesProfile != null) citiesProfile.Clear();
            else citiesProfile = new Dictionary<int, WeatherResponse>();

            tableWeatherInCities = SqliteDataAccess.GetTable("SELECT Weather.id_city, Name, State, Country, longitude, City.latitude, Temp, Feels_like, Pressure, Humidity, Wind_speed, Wind_deg, Clouds, Sunrise, Sunset, Timezone " +
                "FROM Weather, City " +
                "WHERE Weather.id_city = City.id_city AND EXISTS " +
                "(SELECT id_city " +
                "FROM WeatherProfile " +
                $"WHERE id_user_profile = {profile} AND Weather.id_city = WeatherProfile.id_city)");

            for (int i = 0; i < tableWeatherInCities.Rows.Count; i++)
            {
                currentWeatherResp = new WeatherResponse(
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

                citiesProfile.Add(currentWeatherResp.id, currentWeatherResp); // (cityID, WeatherInCity)
            }
        }

        /// <summary>Load weather forecast by city id from database</summary>
        private WeatherForecastResponse LoadWeatherForecastFromDBbyCityId(int cityId)
        {
            WeatherForecastResponse tempForecast = new WeatherForecastResponse();
            tempForecast.list = new List<WeatherForecastInfo>();
            WeatherForecastInfo tempForecastInfo;

            DataTable table = SqliteDataAccess.GetTable($"SELECT * FROM WeatherForecast WHERE city_id = {cityId}");

            for (int i = 0; i < table.Rows.Count; i++)
            {
                tempForecastInfo = new WeatherForecastInfo(
                    Convert.ToSingle(table.Rows[i][3]),
                    Convert.ToSingle(table.Rows[i][4]),
                    Convert.ToSingle(table.Rows[i][5]),
                    Convert.ToSingle(table.Rows[i][6]),
                    Convert.ToInt32(table.Rows[i][7]),
                    Convert.ToInt32(table.Rows[i][8]),
                    Convert.ToInt32(table.Rows[i][9]),
                    Convert.ToInt32(table.Rows[i][10]),
                    table.Rows[i][11].ToString(),
                    table.Rows[i][12].ToString(),
                    table.Rows[i][13].ToString(),
                    Convert.ToSingle(table.Rows[i][14]),
                    Convert.ToInt32(table.Rows[i][15]),
                    Convert.ToInt32(table.Rows[i][16]),
                    table.Rows[i][2].ToString()
                    );

                tempForecast.list.Add(tempForecastInfo);
            }
            return tempForecast;
        }

        /// <summary>Delete current weather by city id from database</summary>
        private void DeleteCurrentWeatherFromDB(int cityId)
        {
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery(
                "DELETE FROM Weather " +
                $"WHERE id_city = {cityId};"
                );

            SqliteDataAccess.Close();
        }

        /// <summary>Delete weather forecast by city id from database</summary>
        private void DeleteWeatherForecastFromDB(int cityId)
        {
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery(
                "DELETE FROM WeatherForecast " +
                $"WHERE city_id = {cityId};"
                );

            SqliteDataAccess.Close();
        }

        /// <summary>Get current weather by city id from server</summary>
        private WeatherResponse GetCurrentWeather(int cityId)
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

        /// <summary>Get current weather by city name from server</summary>
        private WeatherResponse GetCurrentWeather(string cityName) 
        {
            url = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&lang={language}&units={units}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJson = streamReader.ReadToEnd();
            }

            return JsonUtility.FromJson<WeatherResponse>(responseJson);
        }

        /// <summary>Get weather forecast by city id from server</summary>
        public WeatherForecastResponse GetWeatherForecast(int cityId)
        {
            url = $"http://api.openweathermap.org/data/2.5/forecast?id={cityId}&appid={apiKey}&lang={language}&units={units}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJson = streamReader.ReadToEnd();
            }

            return JsonUtility.FromJson<WeatherForecastResponse>(responseJson);
        }

        /// <summary>Get weather forecast by city name from server</summary>
        public WeatherForecastResponse GetWeatherForecast(string cityName)
        {
            url = $"http://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={apiKey}&lang={language}&units={units}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJson = streamReader.ReadToEnd();
            }

            return JsonUtility.FromJson<WeatherForecastResponse>(responseJson);
        }

        /// <summary>Save current weather in database</summary>
        private void SaveCurrentWeather(WeatherResponse weather)
        {
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery("INSERT INTO Weather(id_city, Temp, Feels_like, Pressure, Humidity, Wind_speed, Wind_deg, Clouds, Sunrise, Sunset, Timezone, Weather_id, Weather_main, Weather_description, Weather_icon) " + 
                $"VALUES({weather.id}, '{weather.main.temp}', '{weather.main.feels_like}', {weather.main.pressure}, {weather.main.humidity}, '{weather.wind.speed}', {weather.wind.deg}, {weather.clouds.all}, {weather.sys.sunrise}, {weather.sys.sunset}, {weather.timezone}, {weather.weather[0].id}, \"{weather.weather[0].main}\", \"{weather.weather[0].description}\", \"{weather.weather[0].icon}\") " +
                $"ON CONFLICT(id_city) DO UPDATE SET Temp = '{weather.main.temp}', Feels_like = '{weather.main.feels_like}', Pressure = {weather.main.pressure}, Humidity = {weather.main.humidity}, Wind_speed = '{weather.wind.speed}', Wind_deg = {weather.wind.deg}, Clouds = {weather.clouds.all}, Sunrise = {weather.sys.sunrise}, Sunset = {weather.sys.sunset}, Timezone = {weather.timezone}, Weather_id = {weather.weather[0].id}, Weather_main = \"{weather.weather[0].main}\", Weather_description = \"{weather.weather[0].description}\", Weather_icon = \"{weather.weather[0].icon}\"");

            Debug.Log("Weather successfully saved");

            SqliteDataAccess.Close();
        }

        /// <summary>Save weather forecast in database</summary>
        private void SaveWeatherForecastInDB(WeatherForecastResponse weather)
        {
            DeleteWeatherForecastFromDB(weather.city.id);

            SqliteDataAccess.ConnectTo();

            foreach (WeatherForecastInfo item in weather.list)
            {
                SqliteDataAccess.ExecuteQuery(
                    "INSERT INTO WeatherForecast(city_id, DateTime, Temp, Temp_feels_like, Temp_min, Temp_max, Pressure, Humidity, Sea_level, Grnd_level, Weather_main, Weather_description, Weather_icon, Wind_speed, Wind_deg, Clouds) " +
                    $"VALUES ({weather.city.id}, \"{item.dt_txt}\", '{item.main.temp}', '{item.main.feels_like}', '{item.main.temp_min}', '{item.main.temp_max}', {item.main.pressure}, {item.main.humidity}, {item.main.sea_level}, {item.main.grnd_level}, \"{item.weather[0].main}\", \"{item.weather[0].description}\", \"{item.weather[0].icon}\", '{item.wind.speed}', {item.wind.deg}, {item.clouds.all})");
            }
            SqliteDataAccess.Close();


            UpdateProfile(weather.city.id);
        }

        /// <summary>Save city id and current date time by profile in database</summary>
        private void SaveCityInProfile(int cityId)
        {
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery("INSERT INTO WeatherProfile(id_user_profile, id_city, Datetime_request) " +
                $"VALUES ({profile}, {cityId}, \"{DateTime.Now}\")");

            SqliteDataAccess.Close();
        }

        /// <summary>Delete city by city id from database</summary>
        public void DeleteCityFromProfile(int cityId)
        {
            Debug.Log("DeleteCityFromProfile");
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery("DELETE FROM WeatherProfile " +
                $"WHERE id_city = {cityId}");

            SqliteDataAccess.Close();

            citiesProfile.Remove(cityId);
            citiesList.Remove(cityId);
        }

        /// <summary>Update datetime request by city id in WeatherProfile table</summary>
        private void UpdateProfile(int cityID)
        {
            SqliteDataAccess.ConnectTo();

            SqliteDataAccess.ExecuteQuery("UPDATE WeatherProfile " +
                $"SET Datetime_request = \"{DateTime.Now}\" " +
                $"WHERE id_user_profile = {profile} AND id_city = {cityID}"
            );

            SqliteDataAccess.Close();
        }

        /// <summary>Find city by city name</summary>
        public void FindCity(string cityName)
        {
            HideCityButtons();

            tableFindCity = SqliteDataAccess.GetTable("SELECT * " +
                "FROM City " +
                $"WHERE name like  '%{cityName}%' " + // ORDER BY name
                "LIMIT 10");

            ShowCityButtons(tableFindCity);
        }

        /// <summary>Show on the screen cities by table city</summary>
        /// <param name="tableCity">Table rows -> [id_city], [Name]</param>
        private void ShowCityButtons(DataTable tableCity)
        {
            if (tableCity != null)
                for (int i = 0; i < tableCity.Rows.Count; i++)
                {
                    cityButtons[i].Show(Convert.ToInt32(tableCity.Rows[i][0].ToString()), tableCity.Rows[i][1].ToString()); // Rows[i]["id_city"], Rows[i]["Name"]
                }
        }

        /// <summary>Hide from the screen cities</summary>
        private void HideCityButtons()
        {
            for (int i = 0; i < cityButtons.Length; i++)
                cityButtons[i].Hide();
        }
    }
}
