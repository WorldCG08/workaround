using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using Workaround.Interfaces;

namespace Workaround.Classes.HotKeys
{
    public class MultipleInstancesOpen : IRunCommand
    {
        public void RunCommand()
        {
            string pattern = "i-[a-z0-9]+";
            string clip = Clipboard.GetText();
            
            
                foreach (var instances in Regex.Matches(clip, pattern))
                {
                 System.Diagnostics.Process.Start(new ProcessStartInfo
                 {
                  FileName = "https://eu-west-1.console.aws.amazon.com/systems-manager/session-manager/" + instances.ToString() + "?region=eu-west-1",
                  UseShellExecute = true
                 });
                }
        }
    }
}