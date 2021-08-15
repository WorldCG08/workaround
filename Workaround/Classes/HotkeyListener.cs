using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace Workaround.Classes
{
    // Listener for hotkeys which are pressed in focused application
    public class HotkeyListener
    {
        // Key Up
        const int WM_KEYUP = 0x0101;

        // CTRL + ALT + G
        const int CTRL_ALT_G = 71;

        public HotkeyListener()
        {
            ComponentDispatcher.ThreadPreprocessMessage +=
                ComponentDispatcher_ThreadPreprocessMessage;
        }

        // Use to find needed combination
        //Debug.Print(msg.wParam.ToString());
        void ComponentDispatcher_ThreadPreprocessMessage(
            ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYUP)
            {
                // Google clipboard
                if ((int) msg.wParam == CTRL_ALT_G)
                    MessageBox.Show("!"); // debug command for checking is shortcut working
                if (Clipboard.ContainsText())
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
    }
}