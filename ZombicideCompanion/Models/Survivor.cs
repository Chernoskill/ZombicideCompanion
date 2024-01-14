namespace ZombicideCompanion.Models
{
    public class Survivor
    {
        public string Name { get; set; } = default!;
        public SurvivorClassType ClassType { get; set; } = SurvivorClassType.Civilian!;

        public static string[] SurvivorClassNames = ["C", "S"];

        public Survivor(string name) 
        {
            Name = name;
        }
        public Survivor(string name, SurvivorClassType classType)
        {
            Name = name;
            ClassType = classType;
        }
    }
}
