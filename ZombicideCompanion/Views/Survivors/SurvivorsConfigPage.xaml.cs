using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.Survivors;

public partial class SurvivorsConfigPage : ContentPage
{
    public static ObservableCollection<Survivor> Survivors { get; set; } = new ObservableCollection<Survivor>();
    public ObservableCollection<Survivor> SurvivorsLocal => Survivors;

    private const string SurvivorsFileName = "survivors.txt";
    public static string EventSurvivorsLoaded = "SURVIVORS LOADED";

    public SurvivorsConfigPage()
    {
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"{GetType()} EVENT {m}");
            if (m == EventSurvivorsLoaded)
            {
                SurvivorsListView.SetBinding(ListView.ItemsSourceProperty, "SurvivorsLocal");
            }
        });

        InitializeComponent();

        BindingContext = this;

        SurvivorsListView.SetBinding(ListView.ItemsSourceProperty, "SurvivorsLocal");
    }

    public static async Task LoadSurvivorsAsync()
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(SurvivorsFileName);
        using StreamReader reader = new StreamReader(fileStream);
        string survivorsString = await reader.ReadToEndAsync();
        if (string.IsNullOrEmpty(survivorsString))
        {
            Console.WriteLine("[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] survivorsString is null or empty.");
            return;
        }
        //List<Survivor> survivorList = new List<Survivor>();
        Survivors.Clear();
        string[] lines = survivorsString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] survivorsString lines.Length = {lines.Length}");
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            //Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] line {i}: {line}");
            string name = line.Remove(line.Length - 1, 1).TrimEnd();
            string classString = line.Remove(0, line.Length - 2);
            SurvivorClassType suvClass = SurvivorClassType.Civilian;
            if (int.TryParse(classString, out int classInt))
            {
                suvClass = (SurvivorClassType)classInt;
            }

            Survivors.Add(new Survivor(name, suvClass));
            //survivorList.Add(new Survivor(name, suvClass));
            //Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] Added survivor: {name} {suvClass}");
        }

        Survivors = new ObservableCollection<Survivor>(Survivors.OrderBy(x => Regex.Replace(x.Name, @"[^A-Za-z0-9]", "")));
        WeakReferenceMessenger.Default.Send(EventSurvivorsLoaded);
    }
}