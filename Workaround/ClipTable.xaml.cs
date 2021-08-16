using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Workaround.Classes;
using Workaround.Classes.Model;
namespace Workaround
{
    /// <summary>
    /// Interaction logic for ClipTable.xaml
    /// </summary>
    public partial class ClipTable : Window
    {
        private SqliteConnection _conn = new SqliteConnection("Data Source=" + SettingsManager.Dbname);
        
        public ClipTable()
        {
            InitializeComponent();
            tbClipsLimit.Text = SettingsManager.Load("settings_ClipLimit", "10000");
            
            InitializeClipTable();
            // Set focus to search block
            tbClipListSearch.Focus();
        }
        
        // Listener for click ENTER in search textboxes.
        private void OnClipListSearchEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                InitializeClipTable(tbClipListSearch.Text);
            }
        }
        
        // Listener for click ENTER in limit or search textboxes.
        private void OnClipListLimitEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                InitializeClipTable(tbClipListSearch.Text);
            }
        }

        private void InitializeClipTable(string search = null)
        {
            var clipList = new List<ClipModel>();
            int clipCount = 0;
            int clipLimit = Int32.Parse((tbClipsLimit?.Text.Length > 0) ? tbClipsLimit.Text : "0");
            using (_conn)
            {
                _conn.Open();
                
                var command = _conn.CreateCommand();
                if (search?.Length > 0)
                {
                    command.CommandText =
                        $"SELECT clip, created, id FROM clips WHERE clip LIKE '%{search}%' ORDER BY id DESC";
                }
                else
                {
                    command.CommandText = $"SELECT clip, created, id FROM clips ORDER BY id DESC";
                }

                if (clipLimit == 0)
                {
                    command.CommandText += " LIMIT 10000";
                }
                else
                {
                    command.CommandText += " LIMIT " + clipLimit;
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clipList.Add(
                            new ClipModel(reader.GetString(0), reader.GetString(1), Int32.Parse(reader.GetString(2))));
                        clipCount++;
                    }
                }
                _conn.Close();
            }
            clipgrid.ItemsSource = clipList;
            lblCount.Content = clipCount;
        }

    }
}
