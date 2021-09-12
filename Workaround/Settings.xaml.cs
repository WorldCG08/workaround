using System.Windows;
using System.Windows.Controls;
using Workaround.Classes;

namespace Workaround
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            settings_ClipLimit.Text = SettingsManager.Load("settings_ClipLimit", "10000");
            settings_WslPath.Text = SettingsManager.Load("settings_WslPath", @"C:\Windows\System32\wsl.exe");
        }

        // Button for saving all settings
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in SettingsManager.FindVisualChildren<TextBox>(this))
            {
                SettingsManager.Save(tb.Name, tb.Text);
            }
            
            // Close window after applying
            this.Close();
        }
    }
}
