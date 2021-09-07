using System;
using System.Windows;
using Workaround.Interfaces;

namespace Workaround.Classes.HotKeys
{
    public class OpenPathTable : IRunCommand
    {
        private Window _window;
        
        public OpenPathTable(Window window)
        {
            _window = window;
        }
        public void RunCommand()
        {
            var pathTable = new PathTable();
            pathTable.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowWindow(pathTable, _window);
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
            winObj.Topmost = true;
            winObj.Show();
        }
    }
}