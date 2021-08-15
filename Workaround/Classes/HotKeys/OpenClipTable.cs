using Workaround.Interfaces;

namespace Workaround.Classes.HotKeys
{
    public class OpenClipTable : IRunCommand
    {
        public void RunCommand()
        {
            var clipTable = new ClipTable();
            clipTable.ShowDialog();
        }
    }
}