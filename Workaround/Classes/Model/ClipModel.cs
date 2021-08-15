namespace Workaround.Classes.Model
{
    public class ClipModel
    {
        public string ClipName { get; set; }
        public string Created { get; set; }

        public ClipModel(string name, string date)
        {
            ClipName = name;
            Created = date;
        }
    }
}