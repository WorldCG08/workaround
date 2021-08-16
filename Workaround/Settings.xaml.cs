using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        }

        // Button for saving all settings
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in SettingsManager.FindVisualChildren<TextBox>(this))
            {
                SettingsManager.Save(tb.Name, tb.Text);
            }
        }
    }
}
