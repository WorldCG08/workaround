using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Workaround.Classes;
using Workaround.Classes.HotKeys;
using Workaround.Interfaces;

namespace Workaround
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string DBNAME = "db.db";
        private SqliteConnection _conn;

        public MainWindow()
        {
            InitializeComponent();
            _conn = InitializeDb();
            InitializeClipList(GetClips());

            //Configuration for global hotkeys.
            HotkeysManager.SetupSystemHook();

            // You can create a globalhotkey object and pass it like so
            HotkeysManager.AddHotkey(ModifierKeys.Control, Key.NumPad0, () => { AddToList(new SearchInGoogle()); });
            HotkeysManager.AddHotkey(ModifierKeys.Control, Key.NumPad1, () => { AddToList(new OpenClipTable()); });
            // or do it like this. both end up doing the same thing, but this is probably simpler.
            // HotkeysManager.AddHotkey(ModifierKeys.Control, Key.A, () => { AddToList("Ctrl+A Fired"); });
            // HotkeysManager.AddHotkey(ModifierKeys.Control, Key.D, () => { AddToList("Ctrl+D Fired"); });
            // HotkeysManager.AddHotkey(ModifierKeys.Shift, Key.D, () => { AddToList("Shift+D Fired"); });

            Closing += MainWindow_Closing;
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
            if (Clipboard.ContainsText() && Clipboard.GetText().Length < 10000)
            {
                AddClip(Clipboard.GetText());
            }
            else if (Clipboard.ContainsText() && Clipboard.GetText().Length > 10000)
            {
                AddClip(Clipboard.GetText(), true);
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
        
        private void ClipCalendar_SelectedDatesChanged(object sender,
            SelectionChangedEventArgs e)
        {
            // ... Get reference.
            var calendar = sender as Calendar;

            // ... See if a date is selected.
            if (calendar.SelectedDate.HasValue)
            {
                // ... Display SelectedDate in Title.
                DateTime date = calendar.SelectedDate.Value;
                string selectedDay = date.ToShortDateString();
                
                InitializeClipList(GetClips(selectedDay));
            }
        }
        
        // Initialization of DB and returning of connection
        private SqliteConnection InitializeDb()
        {
            if (!File.Exists(DBNAME))
            {
                File.WriteAllBytes(DBNAME, new byte[0]);
                
                using (var connection = new SqliteConnection("Data Source=" + DBNAME))
                {
                    connection.Open();

                    var clipCreateCommand = connection.CreateCommand();
                    var bigClipCreateCommand = connection.CreateCommand();
                    
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
                    
                    
                    connection.Close();

                    return connection;
                }
            }
            else
            {
                return new SqliteConnection("Data Source=" + DBNAME);
            }
        }
        
        // Add new clip to DB and put it into list.
        private void AddClip(string clip, bool isBig = false)
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
                if (isBig)
                {
                    command.CommandText =
                        $"INSERT INTO bigclips (clip, created) VALUES ('{ClipFormatSave(clip)}','{DateTime.Now}')";
                }
                else
                {
                    command.CommandText =
                        $"INSERT INTO clips (clip, created) VALUES ('{ClipFormatSave(clip)}','{DateTime.Now}')";
                }

                command.ExecuteReader();
                _conn.Close();
            
                // Putting to list if selected date = null or selected today.
                if (ClipCalendar.SelectedDate == null || 
                    DateTime.Now.ToShortDateString() == ClipCalendar.SelectedDate.Value.ToShortDateString())
                {
                    if (!isBig) ClipList.Items.Insert(0, clip);
                }
            }
        }
        
        // Get a list of clips
        private List<string> GetClips(string date = null)
        {
            var clipList = new List<string>();
            using (_conn)
            {
                _conn.Open();

                var command = _conn.CreateCommand();
                if (date != null)
                {
                    command.CommandText = $"SELECT clip FROM clips WHERE created LIKE '{date}%' ORDER BY id DESC";
                }
                else
                {
                    string today = DateTime.Today.ToShortDateString();
                    command.CommandText = $"SELECT clip FROM clips WHERE created LIKE '{today}%' ORDER BY id DESC";
                }

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

        // Show list with clips in ClipList.
        private void InitializeClipList(List<string> clipList)
        {
            ClipList.Items.Clear();
            if (clipList == null) return;
            foreach (var clip in clipList)
            {
                ClipList.Items.Add(clip);
            }
        }

        // Repeat single quote for giving ability to save to sqlite
        private string ClipFormatSave(string str)
        {
            return str.Replace("'", "''");
        }
        
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Need to shutdown the hook. idk what happens if
            // you dont, but it might cause a memory leak.
            HotkeysManager.ShutdownSystemHook();
        }
        
        public void AddToList(IRunCommand runCommand)
        {
            runCommand.RunCommand();
        }
    }
}
