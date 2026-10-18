namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für das Geschlecht
    /// </summary>
    public class Gender
    {
        public Gender(int genderId, string name)
        {
            GenderId = genderId;
            Name = name;
        }

        public int GenderId { get; set; }
        public string Name { get; set; }
    }
}
