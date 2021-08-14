﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using System.Windows.Interop;
using Workaround.Classes;

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
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Initialize the clipboard now that we have a window soruce to use
            var windowClipboardManager = new ClipboardManager(this);
            windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }

        private void ClipboardChanged(object sender, EventArgs e)
        {
            // Handle your clipboard update here:
            if (Clipboard.ContainsText())
            {
                AddClip(Clipboard.GetText());
            }
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
        
        // Add new clip to DB and put it into list.
        private void AddClip(string clip)
        {
            bool InList = false;
            if (ClipList.SelectedItem != null && ClipList.SelectedItem.ToString() == clip) return;
            
            foreach (var listBoxItem in ClipList.Items)
            {
                if (listBoxItem.ToString() == clip) InList = true;
            }

            if (!InList)
            {
                // Adding to DB.
                _conn.Open();
                var command = _conn.CreateCommand();
                command.CommandText =
                    $"INSERT INTO clips (clip, created) VALUES ('{clip}','{DateTime.Now}')";
                command.ExecuteReader();
                _conn.Close();
            
                // Putting to list.
                ClipList.Items.Insert(0, clip);
            }
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

        // private string ClipFormatSave(string str)
        // {
        //     
        // }
    }
}
