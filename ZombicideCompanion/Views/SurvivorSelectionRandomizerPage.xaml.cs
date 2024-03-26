using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using ZombicideCompanion.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZombicideCompanion.Views;

public partial class SurvivorSelectionRandomizerPage : ContentPage
{
    public ObservableCollection<Survivor> Survivors { get; set; } = new ObservableCollection<Survivor>();
    public ObservableCollection<Survivor> RandomTeam { get; set; } = new ObservableCollection<Survivor>();
    public List<int> SurvivorIndices { get; set; } = default!;

    private const string SurvivorsFileName = "survivors.txt";
    private const string EventSurvivorsLoaded = "SURVIVORS LOADED";

    public SurvivorSelectionRandomizerPage()
    {
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"EVENT {m}");
            if (m == EventSurvivorsLoaded)
            {
                survivorsListView.SetBinding(ListView.ItemsSourceProperty, "Survivors");
            }
        });

        InitializeComponent();

        BindingContext = this;

        randomTeamListView.SetBinding(ListView.ItemsSourceProperty, "RandomTeam");

        Task.Run(async () =>
        {
            await LoadSurvivorsAsync();
            WeakReferenceMessenger.Default.Send(EventSurvivorsLoaded);
        });
    }

    protected override void OnAppearing()
    {
        //((Models.SurvivorSelectionRandomizer)BindingContext).LoadSurvivors();
        //survivorCollection.ItemsSource = ((Models.SurvivorSelectionRandomizer)BindingContext).Survivors;
    }

    private void RandomizeTeam_Clicked(object sender, EventArgs e)
    {
        SurvivorIndices = Enumerable.Range(0, Survivors.Count).ToList();
        RandomTeam.Clear();
        Random random = new Random();
        for (int i = 0; i < 6; i++)
        {
            int index = random.Next(SurvivorIndices.Count);
            Survivor survivor = Survivors[SurvivorIndices[index]];
            RandomTeam.Add(survivor);
            SurvivorIndices.RemoveAt(index);
        }
        //randomTeamLabel.SetBinding(Label.TextProperty, "RandomTeamString");
    }

    public async Task LoadSurvivorsAsync()
    {
        //Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] SurvivorsPath: {MainPage.Config.SurvivorsPath}");
        string appDataDir = FileSystem.Current.AppDataDirectory;
        string survivorsString = await FileUtility.LoadFile(Path.Combine(appDataDir, SurvivorsFileName));
        if (string.IsNullOrEmpty(survivorsString))
        {
            Console.WriteLine("[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] survivorsString is null or empty.");
            return;
        }
        List<Survivor> survivorList = new List<Survivor>();
        string[] lines = survivorsString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] survivorsString lines.Length = {lines.Length}");
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] line {i}: {line}");
            string name = line.Remove(line.Length - 1, 1).TrimEnd();
            string classString = line.Remove(0, line.Length - 2);
            SurvivorClassType suvClass = SurvivorClassType.Civilian;
            if (int.TryParse(classString, out int classInt))
            {
                suvClass = (SurvivorClassType)classInt;
            }

            survivorList.Add(new Survivor(name, suvClass));
            Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] Added survivor: {name} {suvClass}");
        }

        //Survivors = new ObservableCollection<Survivor>(survivorList.OrderBy(x => x.Name));
        Survivors = new ObservableCollection<Survivor>(survivorList.OrderBy(x => Regex.Replace(x.Name, @"[^A-Za-z0-9]", "")));

        return;
    }

    private async void PickSurvivorFileButton_Clicked(object sender, EventArgs e)
    {
        await PickSurvivorFileAsync();
    }

    private async Task PickSurvivorFileAsync()
    {
        string? sourcePath = await FileUtility.PickFile();
        if (string.IsNullOrEmpty(sourcePath))
        {
            Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] sourcePath \"{sourcePath}\" is null or empty.");
        }
        string survivorsString = await FileUtility.LoadFile(sourcePath!);
        if (string.IsNullOrEmpty(survivorsString))
        {
            Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] survivorsString \"{survivorsString}\" is null or empty.");
        }
        string appDataDir = FileSystem.Current.AppDataDirectory;
        string targetPath = Path.Combine(appDataDir, SurvivorsFileName);
        await FileUtility.SaveFile(survivorsString, targetPath);
        Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] Saved survivorsFile to \"{targetPath}\".");

        await LoadSurvivorsAsync();
    }
}