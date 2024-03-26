namespace ZombicideCompanion.Models
{
    public class Survivor
    {
        public string Name { get; set; } = default!;
        private SurvivorClassType _classType;

        public SurvivorClassType ClassType
        {
            get
            {
                return _classType;
            }
            set
            {
                _classType = value;
                ClassTypeImageName = _classType == SurvivorClassType.Civilian ? "survivorclassciv.png" : "survivorclasssol.png";
            }
        }
        public string ClassTypeImageName { get; set; } = default!;

        public Survivor(string name, SurvivorClassType classType)
        {
            Name = name;
            ClassType = classType;
        }
    }
}
