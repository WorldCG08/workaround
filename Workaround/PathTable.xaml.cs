using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Workaround.Classes;

namespace Workaround
{
    /// <summary>
    /// Interaction logic for PathTable.xaml
    /// </summary>
    public partial class PathTable : Window
    {
        private SqliteConnection _conn;

        public PathTable()
        {
            InitializeComponent();
            _conn = new SettingsManager().InitializeDb();
            InitializePathList(GetPaths());
        }

        private void ButtonPathAdd(object sender, RoutedEventArgs e)
        {
            // add path from clipboard
            if (Clipboard.ContainsText() && Clipboard.GetText().Length < 500)
            {
                AddPath(Clipboard.GetText());
            }
        }

        private void ButtonPathRemove(object sender, RoutedEventArgs e)
        {
            var selected = PathList.SelectedItem?.ToString();
            if (selected != null)
            {
                _conn.Open();
                var command = _conn.CreateCommand();
                command.CommandText = $"DELETE FROM paths WHERE path = '{selected}';";
                command.ExecuteReader();
                _conn.Close();
                InitializePathList(GetPaths());
            }
        }

        // Add new path to DB and put it into list.
        private void AddPath(string path)
        {
            bool inList = false;
            if (PathList.SelectedItem != null && PathList.SelectedItem.ToString() == path) return;

            foreach (var listBoxItem in PathList.Items)
            {
                if (listBoxItem.ToString() == path) inList = true;
            }

            if (!inList)
            {
                // Adding to DB.
                _conn.Open();
                var command = _conn.CreateCommand();
                command.CommandText =
                    $"INSERT INTO paths (path, created) VALUES ('{MainWindow.ClipFormatSave(path)}','{DateTime.Now}')";

                command.ExecuteReader();
                _conn.Close();

                PathList.Items.Insert(0, path);
            }
        }

        // Get a list of paths
        private List<string> GetPaths()
        {
            var pathList = new List<string>();
            using (_conn)
            {
                _conn.Open();

                var command = _conn.CreateCommand();
                command.CommandText = $"SELECT path FROM paths ORDER BY id DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var path = reader.GetString(0);
                        pathList.Add(path);
                    }
                }
            }

            return pathList;
        }

        // Show list with clips in ClipList.
        private void InitializePathList(List<string> pathList)
        {
            PathList.Items.Clear();
            if (pathList == null) return;
            foreach (var clip in pathList)
            {
                PathList.Items.Add(clip);
            }
        }

        private void pathList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = PathList.SelectedItem;
            var wslPath = SettingsManager.Load("settings_WslPath");
            if (index == null) return;

            if (Keyboard.IsKeyDown(Key.LeftShift) && wslPath != null)
            {
                Process.Start(wslPath, $"--cd \"{PathList.SelectedItem.ToString()!}\"");
            }
            else
            {
                Process.Start("explorer.exe", PathList.SelectedItem.ToString()!);
            }
        }
    }
}