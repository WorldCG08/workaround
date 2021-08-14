using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Workaround
{
    /// <summary>
    /// Interaction logic for MultipleInstancesOpenWindow.xaml
    /// </summary>
    public partial class MultipleInstancesOpenWindow : Window
    {
        public MultipleInstancesOpenWindow()
        {
            InitializeComponent();
        }

        private void OpenInstancesClick(object sender, RoutedEventArgs e)
        {
            string[] InstIds = awsInstances.Text.Split(',');

            foreach (var inst in InstIds)
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "https://eu-west-1.console.aws.amazon.com/systems-manager/session-manager/" + inst + "?region=eu-west-1",
                    UseShellExecute = true
                });
            }
        }
    }
}
