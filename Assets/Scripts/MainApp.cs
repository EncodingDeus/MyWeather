﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.UI;
using System.Data;
using System;

namespace MyWeather
{
    public class MainApp : MonoBehaviour
    {

        [SerializeField]
        private CityButton[] cityButtons;


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


        void Start()
        {
            // Check Network

            //resp = GetWeather(); // Проверить, что будет если не будет интернета, посмотреть на сайте
            resp = GetWeather(5174);
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

            tableProfileCitiesId = SqliteDataAccess.GetTable($"SELECT * FROM WeatherProfile WHERE id_user_profile = {profile}");

            for (int i = 0; i < tableProfileCitiesId.Rows.Count; i++)
            {
                tableWeatherInCities = SqliteDataAccess.GetTable($"SELECT * FROM Weather WHERE id_city = {tableProfileCitiesId.Rows[i][2].ToString()}");

                Debug.Log($"city: {tableProfileCitiesId.Rows[i][2]} Found?:{tableWeatherInCities != null}");

                if (tableWeatherInCities == null) continue;

                weatherInCity = new WeatherResponse(
                    Convert.ToInt32(tableWeatherInCities.Rows[0][1].ToString()), // cityId
                    Convert.ToSingle(tableWeatherInCities.Rows[0][2]), // temp
                    Convert.ToSingle(tableWeatherInCities.Rows[0][3]), // feels_like
                    Convert.ToInt32(tableWeatherInCities.Rows[0][4]),  // pressure
                    Convert.ToInt32(tableWeatherInCities.Rows[0][5]),  // humidity
                    Convert.ToSingle(tableWeatherInCities.Rows[0][6]), // wind_speed
                    Convert.ToInt32(tableWeatherInCities.Rows[0][7]),  // wind_deg
                    Convert.ToInt32(tableWeatherInCities.Rows[0][8]),  // clouds 
                    Convert.ToInt32(tableWeatherInCities.Rows[0][9]),  // sunrise
                    Convert.ToInt32(tableWeatherInCities.Rows[0][10]), // sunset
                    Convert.ToInt32(tableWeatherInCities.Rows[0][11])  // timezone
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
            url = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={APIKEY}&lang={LANGUAGE}&units={UNITS}";

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
            url = $"http://api.openweathermap.org/data/2.5/weather?id={cityId}&appid={APIKEY}&lang={LANGUAGE}&units={UNITS}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseJson = streamReader.ReadToEnd();
            }

            return JsonUtility.FromJson<WeatherResponse>(responseJson);
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
