using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Sqlite;

namespace Workaround
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string _dbName = "db.db";
        private SqliteConnection _conn;
        public MainWindow()
        {
            InitializeComponent();
            _conn = InitializeDb();
            InitializeClipList(GetClips());
        }

        private void MenuItem_openEcMultipleConsole(object sender, RoutedEventArgs e)
        {
            var multipleConsoleWindow = new MultipleInstancesOpenWindow();
            multipleConsoleWindow.ShowDialog();
        }

        // Set selected clip to buffer
        private void clipList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = this.ClipList.SelectedItem;
            if (index == null) return;
            {
                Clipboard.SetText(this.ClipList.SelectedItem.ToString()!);
            }
        }
        
        // Initialization of DB and returning of connection
        private SqliteConnection InitializeDb()
        {
            if (!File.Exists(_dbName))
            {
                File.WriteAllBytes(_dbName, new byte[0]);
                
                using (var connection = new SqliteConnection("Data Source=" + _dbName))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"
                        CREATE TABLE clips (id INTEGER	constraint clips_pk primary key autoincrement,
                        clip text, created text)
                        ";
                    command.ExecuteReader();
                    connection.Close();

                    return connection;
                }
            }
            else
            {
                return new SqliteConnection("Data Source=" + _dbName);
            }
        }
        
        // Add new clip to DB.
        private void AddClip(string clip)
        {
            _conn.Open();
            var command = _conn.CreateCommand();
            command.CommandText =
                $"INSERT INTO clips (clip, created) VALUES ('{clip}','{DateTime.Now}')";
            command.ExecuteReader();
            _conn.Close();
        }
        
        // Get a list of clips
        private List<string> GetClips()
        {
            var clipList = new List<string>();
            using (_conn)
            {
                _conn.Open();

                var command = _conn.CreateCommand();
                command.CommandText =
                    @"
                        SELECT clip
                        FROM clips
                        ORDER BY id DESC
                    ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var clip = reader.GetString(0);
                        clipList.Add(clip);
                    }
                }
            }

            return clipList;
        }

        private void InitializeClipList(List<string> clipList)
        {
            if (clipList == null) return;
            foreach (var clip in clipList)
            {
                ClipList.Items.Add(clip);
            }
        }
    }
}
