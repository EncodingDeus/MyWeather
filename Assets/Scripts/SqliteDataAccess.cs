using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

namespace MyWeather
{
    public static class SqliteDataAccess
    {
        public static SqliteConnection connection;
        public static SqliteCommand command;

        private const string fileName = @"database.db";
        public static string DBPath = @"C:\Users\Deus\Documents\GitHub\Unity3D\Test_assignments\MyWeather\Assets\StreamingAssets\database.db";

        static SqliteDataAccess()
        {
            DBPath = GetDatabasePath();
        }

        /// <summary> Возвращает путь к БД. Если её нет в нужной папке на Андроиде, то копирует её с исходного apk файла. </summary>
        private static string GetDatabasePath()
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, fileName);
#endif
#if UNITY_STANDALONE
            string filePath = Path.Combine(Application.dataPath, fileName);
            if (!File.Exists(filePath)) UnpackDatabase(filePath);
            return filePath;
#elif UNITY_ANDROID
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if(!File.Exists(filePath)) UnpackDatabase(filePath);
            return filePath;
#endif
        }

        public static void ConnectTo()
        {
            connection = new SqliteConnection("Data Source=" + DBPath);
            command = new SqliteCommand(connection);
            connection.Open();
        }

        public static void Close()
        {
            connection.Close();
            command.Dispose();
        }

        public static void ExecuteQuery(string query)
        {
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public static string ExecuteQueryWithAnswer(string query)
        {
            ConnectTo();
            command.CommandText = query;
            var answer = command.ExecuteScalar();
            Close();



            if (answer != null) return answer.ToString();
            else return null;
        }

        /// <summary> Этот метод возвращает таблицу, которая является результатом выборки запроса query. </summary>
        /// <param name="query"> Собственно запрос. </param>
        public static DataTable GetTable(string query)
        {
            ConnectTo();

            SqliteDataAdapter adapter = new SqliteDataAdapter(query, connection);

            DataSet DS = new DataSet();
            adapter.Fill(DS);
            adapter.Dispose();

            Close();

            return DS.Tables[0];
        }

        /// <summary> Распаковывает базу данных в указанный путь. </summary>
        /// <param name="toPath"> Путь в который нужно распаковать базу данных. </param>
        private static void UnpackDatabase(string toPath)
        {
            string fromPath = Path.Combine(Application.streamingAssetsPath, fileName);

            WWW reader = new WWW(fromPath);
            while (!reader.isDone) { }

            File.WriteAllBytes(toPath, reader.bytes);
        }
    }
}