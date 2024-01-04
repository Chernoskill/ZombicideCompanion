namespace ZombicideCompanion.Models
{
    public class Survivor
    {
        public string Name { get; set; } = default!;
        public Survivor(string name) 
        {
            Name = name;
        }
    }
}
