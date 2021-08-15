using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Workaround.Classes.Model;
namespace Workaround
{
    /// <summary>
    /// Interaction logic for ClipTable.xaml
    /// </summary>
    public partial class ClipTable : Window
    {
        private SqliteConnection _conn = new SqliteConnection("Data Source=" + MainWindow.DBNAME);
        public ClipTable()
        {
            InitializeComponent();
            InitializeClipTable();
            tbClipListSearch.Focus();
        }
        
        private void OnClipListSearchEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                InitializeClipTable(tbClipListSearch.Text);
            }
        }
        
        private void InitializeClipTable(string search = null)
        {
            var clipList = new List<ClipModel>();
            using (_conn)
            {
                _conn.Open();
                
                var command = _conn.CreateCommand();
                if (search?.Length > 0)
                {
                    command.CommandText =
                        $"SELECT clip, created FROM clips WHERE clip LIKE '%{search}%' ORDER BY id DESC";
                }
                else
                {
                    command.CommandText = $"SELECT clip, created FROM clips ORDER BY id DESC";
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clipList.Add(new ClipModel(reader.GetString(0), reader.GetString(1)));
                    }
                }
                _conn.Close();
            }
            clipgrid.ItemsSource = clipList;
        }
    }
}
