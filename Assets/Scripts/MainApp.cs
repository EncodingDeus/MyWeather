using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Data;

namespace MyWeather
{
    public class MainApp : MonoBehaviour
    {
        private const string APIKEY = "e487ce3cd777104bda644f483932393c"; // private static readonly
        
        private const string UNITS = "&units=metric"; // UNITS: imperial ("&units=imperial");  metric ("&units=metric");  standard (""); 
        private const string LANGUAGE = "&lang=ru";

        public string city = "London"; // using nowhere

        public CityInfo[] cities;

        public WeatherResponse resp;

        void Start()
        {
            
        }

        private WeatherResponse GetWeatherFromFile(string path)
        {
            string weather = File.ReadAllText(path); // check existing file path

            return JsonUtility.FromJson<WeatherResponse>(weather);
        }

        private WeatherResponse GetWeather() // string city
        {
            string url = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=" + APIKEY + LANGUAGE + UNITS;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string response;

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            SaveWeatherInJSON(response, "tempCity");

            return JsonUtility.FromJson<WeatherResponse>(response);
        }

        void SaveWeatherInJSON(string weather, string path)
        {
            Debug.Log("file Saved = " + Application.dataPath + @"/" + path + ".json");
            File.WriteAllText(Application.dataPath + @"/" + path + ".json", weather);
        }

        void GetCitiesInfo()
        {

            string citiesInfoJsonPath = Application.streamingAssetsPath + @"\city.list.json"; // Move
            string citiesInfoJson = File.ReadAllText(citiesInfoJsonPath);

            cities = JsonHelper.FromJson<CityInfo>(citiesInfoJson);
            Debug.Log("UpdateCitiesInfo");
        }
        void SaveCitiesInfo()
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
