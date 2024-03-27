using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views;

public partial class SurvivorSelectionRandomizerPage : ContentPage
{
    public ObservableCollection<Survivor> Survivors { get; set; } = new ObservableCollection<Survivor>();
    public ObservableCollection<Survivor> RandomTeam { get; set; } = new ObservableCollection<Survivor>();
    public List<int> SurvivorIndices { get; set; } = default!;
    private int _teamSize;
    public int TeamSize 
    { 
        get
        {
            return _teamSize;
        }        
        set
        {
            int teamSizeLast = _teamSize;
            _teamSize = Math.Clamp(value, 1,8);
            teamSizeLabel.SetBinding(Label.TextProperty, "TeamSize");
            if(_teamSize < teamSizeLast)
            {
                for(int i = _teamSize; i < teamSizeLast; i++)
                {
                    int indexSurvivors = Survivors.ToList().FindIndex(x => x.Name == RandomTeam[RandomTeam.Count - 1].Name);
                    SurvivorIndices.Add(indexSurvivors);
                    RandomTeam.RemoveAt(RandomTeam.Count - 1);
                }
            }
            else if (_teamSize > teamSizeLast)
            {
                for (int i = teamSizeLast; i < _teamSize; i++)
                {
                    Random random = new Random();
                    int index = random.Next(SurvivorIndices.Count);
                    Survivor survivor = Survivors[SurvivorIndices[index]];
                    RandomTeam.Add(survivor);
                    SurvivorIndices.RemoveAt(index);
                }
            }
        }
    }

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
        teamSizeLabel.SetBinding(Label.TextProperty, "TeamSize");

        Task.Run(async () =>
        {
            await LoadSurvivorsAsync();
            WeakReferenceMessenger.Default.Send(EventSurvivorsLoaded);
            SurvivorIndices = Enumerable.Range(0, Survivors.Count).ToList();
            TeamSize = 6;
            RandomizeTeam();
        });
    }

    protected override void OnAppearing()
    {
        //((Models.SurvivorSelectionRandomizer)BindingContext).LoadSurvivors();
        //survivorCollection.ItemsSource = ((Models.SurvivorSelectionRandomizer)BindingContext).Survivors;
    }

    private void RandomizeTeam_Clicked(object sender, EventArgs e)
    {
        RandomizeTeam();
    }

    private void RandomizeTeam()
    {
        SurvivorIndices = Enumerable.Range(0, Survivors.Count).ToList();
        if (SurvivorIndices.Count == 0)
        {
            return;
        }
        RandomTeam.Clear();
        Random random = new Random();
        for (int i = 0; i < TeamSize; i++)
        {
            int index = random.Next(SurvivorIndices.Count);
            Survivor survivor = Survivors[SurvivorIndices[index]];
            RandomTeam.Add(survivor);
            SurvivorIndices.RemoveAt(index);
        }
    }

    private async Task LoadSurvivorsAsync()
    {
        //Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] SurvivorsPath: {MainPage.Config.SurvivorsPath}");
        string appDataDir = FileSystem.Current.AppDataDirectory;
        string survivorsString = await FileUtility.LoadFile(Path.Combine(appDataDir, SurvivorsFileName));
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
        survivorsListView.SetBinding(ListView.ItemsSourceProperty, "Survivors");
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

    private void ReselectSurvivorButton_Clicked(object sender, EventArgs e)
    {
        if(Survivors.Count <= SurvivorIndices.Count)
        {
            return;
        }
        Button button = (Button)sender;
        Survivor survivorRemove = (Survivor)button.BindingContext;
        int indexRandomTeam = RandomTeam.ToList().FindIndex(x => x.Name == survivorRemove.Name);
        RandomTeam.RemoveAt(indexRandomTeam);

        Random random = new Random();
        int index = random.Next(SurvivorIndices.Count);
        Survivor survivorNew = Survivors[SurvivorIndices[index]];
        RandomTeam.Insert(indexRandomTeam, survivorNew);
        SurvivorIndices.RemoveAt(index);

        int indexSurvivors = Survivors.ToList().FindIndex(x => x.Name == survivorRemove.Name);
        SurvivorIndices.Add(indexSurvivors);
    }

    public void TeamSizeIncreaseButton_Clicked(object sender, EventArgs e)
    {
        TeamSize++;
    }

    public void TeamSizeDecreaseButton_Clicked(object sender, EventArgs e)
    {
        TeamSize--;
    }
}