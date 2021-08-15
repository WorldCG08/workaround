using System.Diagnostics;
using System.Windows;
using Workaround.Interfaces;

namespace Workaround.Classes.HotKeys
{
    public class SearchInGoogle : IRunCommand
    {
        public void RunCommand()
        {
            var text = Clipboard.GetText();
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = $"https://www.google.com/search?q={text}",
                UseShellExecute = true
            });
        }
    }
}