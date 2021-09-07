using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Data.Sqlite;

namespace Workaround.Classes
{
    // Class for saving, loading settings and configuring DB first time.
    public class SettingsManager
    {
        public const string Dbname = "db.db";
        public static readonly SqliteConnection CONN = new SqliteConnection("Data Source=" + Dbname);

        // Save setting
        public static void Save(string setting, string value)
        {
            using (var connection = new SqliteConnection("Data Source=" + Dbname))
            {
                connection.Open();
                var command = connection.CreateCommand();
                //Check is setting exist.
                if (IsSettingExist(setting))
                {
                    command.CommandText = $"UPDATE settings SET value = {value} WHERE setting = '{setting}'";
                }
                else
                {
                    command.CommandText = $"INSERT INTO settings (setting, value) VALUES ('{setting}','{value}')";
                }
                command.ExecuteReader();
                connection.Close();
            }
        }

        // Load setting
        public static string Load(string setting, string defVal = null)
        {
            using (var connection = new SqliteConnection("Data Source=" + Dbname))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT value FROM settings WHERE setting = '{setting}' limit 1";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
                connection.Close();
                return defVal;
            }
        }

        public static bool IsSettingExist(string setting)
        {
            using (var connection = new SqliteConnection("Data Source=" + Dbname))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT id FROM settings WHERE setting = '{setting}'";
                var HasRows = command.ExecuteReader().HasRows;
                connection.Close();
                return HasRows;
            }
        }

        // Initialization of DB and returning of connection
        public SqliteConnection InitializeDb()
        {
            if (!File.Exists(Dbname))
            {
                File.WriteAllBytes(Dbname, new byte[0]);

                using (var connection = new SqliteConnection("Data Source=" + Dbname))
                {
                    connection.Open();
                    // Clips saver
                    var clipCreateCommand = connection.CreateCommand();
                    // Big clips saver
                    var bigClipCreateCommand = connection.CreateCommand();
                    // Settings saver
                    var SettingsCreateCommand = connection.CreateCommand();
                    // Path saver
                    var PathCreateCommand = connection.CreateCommand();

                    // Create table for simple clips (less 10000 length)
                    clipCreateCommand.CommandText =
                        @"
                        CREATE TABLE clips (id INTEGER	constraint clips_pk primary key autoincrement,
                        clip text, created text)
                        ";
                    clipCreateCommand.ExecuteReader();

                    // Create table for big clips (>10000 length)
                    bigClipCreateCommand.CommandText =
                        @"
                        CREATE TABLE bigclips (id INTEGER	constraint bigclips_pk primary key autoincrement,
                        clip text, created text)
                        ";
                    bigClipCreateCommand.ExecuteReader();

                    // Create table for settings
                    SettingsCreateCommand.CommandText =
                        @"
                        CREATE TABLE settings (id INTEGER	constraint settings_pk primary key autoincrement,
                        setting text, value text)
                        ";
                    SettingsCreateCommand.ExecuteReader();
                    
                    // Create table for simple clips (less 10000 length)
                    PathCreateCommand.CommandText =
                        @"
                        CREATE TABLE paths (id INTEGER	constraint paths_pk primary key autoincrement,
                        path text, created text)
                        ";
                    PathCreateCommand.ExecuteReader();
                    
                    connection.Close();
                    return connection;
                }
            }
            else
            {
                return new SqliteConnection("Data Source=" + Dbname);
            }
        }
        
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;
    
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child != null && child is T)
                    yield return (T)child;
                
                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}