using System.Collections.ObjectModel;

namespace ZombicideCompanion.Models
{
    internal class SurvivorSelectionRandomizer
    {
        public ObservableCollection<Survivor> Survivors { get; set; } = new ObservableCollection<Survivor>();
        public Survivor RandomSurvivor { get; set; }

        public SurvivorSelectionRandomizer() =>
            LoadSurvivors();

        public void LoadSurvivors()
        {
            Survivors.Clear();
            RandomSurvivor = new Survivor("[None]");

            Survivors = new ObservableCollection<Survivor>()
            {
                new Survivor("Baraka"),
                new Survivor("Cole"),
                new Survivor("Jared"),
                new Survivor("Magnus"),
                new Survivor("Mitsuki"),
                new Survivor("Vivian"),
                new Survivor("Andrei"),
                new Survivor("Fiona"),
                new Survivor("Jean"),
                new Survivor("Kyle"),
                new Survivor("Maria"),
                new Survivor("Solomon"),
                new Survivor("'Butcher' Carl"),
                new Survivor("Captain Warlock"),
                new Survivor("Dorian The Grey"),
                new Survivor("Nura Satar"),
            };
            //Survivors = (ObservableCollection<string>)Survivors.Order();
        }

        public void SelectRandomSurvivor()
        {
            if (Survivors.Count == 0)
            {
                RandomSurvivor = new Survivor("[All taken!]");
                return;
            }
            Random random = new Random();
            int i = random.Next(Survivors.Count);
            RandomSurvivor = Survivors[i];
            Survivors.RemoveAt(i);
        }
    }
}
