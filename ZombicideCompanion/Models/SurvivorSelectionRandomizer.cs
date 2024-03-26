using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using ZombicideCompanion.Views;

namespace ZombicideCompanion.Models
{
    internal class SurvivorSelectionRandomizer
    {
        public ObservableCollection<Survivor> Survivors { get; set; } = new ObservableCollection<Survivor>();
        public Survivor RandomSurvivor { get; set; } = default!;
        public int SurvivorCount { get; set; }

        public SurvivorSelectionRandomizer()
        {
            LoadSurvivorsAsync();
        }

        public void LoadSurvivorsAsync()
        {
            //Survivors.Clear();
            //RandomSurvivor = new Survivor("[None]");

            //string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string survivorsFile = "Survivors.txt";
            //string survivorsPath = System.IO.Path.Combine(documentsDirectory, survivorsFile);
            //SurvivorLoader.WriteTextToFile("Blubb", survivorsPath);
            //string s = SurvivorLoader.ReadTextFile(survivorsPath);
            //Console.WriteLine("*************************: " + survivorsPath);


            //Survivors = new ObservableCollection<Survivor>()
            //{
            //    new Survivor("Andrei", SurvivorClassType.Soldier),
            //    new Survivor("Baraka", SurvivorClassType.Soldier),
            //    new Survivor("'Butcher' Carl", SurvivorClassType.Civilian),
            //    new Survivor("Captain Warlock", SurvivorClassType.Civilian),
            //    new Survivor("Cloud", SurvivorClassType.Soldier),
            //    new Survivor("Cole", SurvivorClassType.Civilian),
            //    new Survivor("Cyborg Eddie", SurvivorClassType.Soldier),
            //    new Survivor("Doctor Stakefrieze", SurvivorClassType.Civilian),
            //    new Survivor("Dorian The Grey", SurvivorClassType.Civilian),
            //    new Survivor("Fiona", SurvivorClassType.Soldier),
            //    new Survivor("Hugo Harker", SurvivorClassType.Civilian),
            //    new Survivor("Inshish Vak", SurvivorClassType.Soldier),
            //    new Survivor("Jared", SurvivorClassType.Soldier),
            //    new Survivor("Jean", SurvivorClassType.Soldier),
            //    new Survivor("Kyle", SurvivorClassType.Soldier),
            //    new Survivor("Magnus", SurvivorClassType.Soldier),
            //    new Survivor("Major Sharp", SurvivorClassType.Soldier),
            //    new Survivor("Maria", SurvivorClassType.Soldier),
            //    new Survivor("Mitsuki", SurvivorClassType.Civilian),
            //    new Survivor("Norton", SurvivorClassType.Civilian),
            //    new Survivor("Nura Satar", SurvivorClassType.Civilian),
            //    new Survivor("'Ook Ook'", SurvivorClassType.Civilian),
            //    new Survivor("Samurai Eddie", SurvivorClassType.Soldier),
            //    new Survivor("Solomon", SurvivorClassType.Soldier),
            //    new Survivor("Vivian", SurvivorClassType.Civilian),
            //};
            //Survivors = (ObservableCollection<string>)Survivors.Order();
            SurvivorCount = Survivors.Count;
        }
    }
}
