namespace Workaround.Classes.Model
{
    public class ClipModel
    {
        public string ClipName { get; set; }
        public string Created { get; set; }
        public int Id { get; set; }

        public ClipModel(string name, string date, int id)
        {
            ClipName = name;
            Created = date;
            Id = id;
        }
    }
}