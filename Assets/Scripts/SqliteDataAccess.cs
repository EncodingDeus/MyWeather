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
        public static string DBPath = @"..MyWeather\Assets\StreamingAssets\database.db";

        static SqliteDataAccess()
        {
            DBPath = GetDatabasePath();
        }

        /// <summary> Returns the path to the database. </summary>
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

        /// <summary> Connect to database.<summary>
        public static void ConnectTo()
        {
            connection = new SqliteConnection("Data Source=" + DBPath);
            command = new SqliteCommand(connection);
            connection.Open();
        }

        /// <summary> Close connection to database. </summary>
        public static void Close()
        {
            connection.Close();
            command.Dispose();
        }

        /// <summary> Execute request without answer (fetch, delete, insert, etc.)</summary>
        public static void ExecuteQuery(string query)
        {
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        /// <summary> Returns a table that is the result of a query fetch. </summary>
        public static DataTable GetTable(string query)
        {
            ConnectTo();

            SqliteDataAdapter adapter = new SqliteDataAdapter(query, connection);

            DataSet DS = new DataSet();
            adapter.Fill(DS);
            adapter.Dispose();

            Close();
            if (DS.Tables[0].Rows.Count == 0)
                return null;
            else
                return DS.Tables[0];
        }

        /// <summary> Unpacks the database to the specified path. </summary>
        /// <param name="toPath"> The path to unpack the database. </param>
        private static void UnpackDatabase(string toPath)
        {
            string fromPath = Path.Combine(Application.streamingAssetsPath, fileName);

            WWW reader = new WWW(fromPath);
            while (!reader.isDone) { }

            File.WriteAllBytes(toPath, reader.bytes);
        }
    }
}