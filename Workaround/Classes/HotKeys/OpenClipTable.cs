using System;
using System.Linq;
using System.Windows;
using Workaround.Interfaces;

namespace Workaround.Classes.HotKeys
{
    public class OpenClipTable : IRunCommand
    {
        private Window _window;
        public OpenClipTable(Window window)
        {
            _window = window;
        }
        
        public void RunCommand()
        {
            var clipTable = new ClipTable();
            clipTable.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowWindow(clipTable, _window);
            //clipTable.Topmost = true;
        }
        
        // Method for preventing open multiple windows at once
        public void ShowWindow(Object Obj, Object ObjThis) {
            Window winObj = (Window)Obj;
            Type windowType = winObj.GetType();
            foreach(Window openWindow in System.Windows.Application.Current.Windows)
            {
                if (openWindow.IsVisible && windowType == openWindow.GetType())
                    return;
            }
            winObj.Owner = (Window)ObjThis;
            winObj.ShowInTaskbar = false;
            winObj.Show();
        }
    }


}