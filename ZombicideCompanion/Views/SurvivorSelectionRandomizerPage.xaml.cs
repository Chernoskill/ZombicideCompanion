using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views;

public partial class SurvivorSelectionRandomizerPage : ContentPage
{
    public ObservableCollection<Survivor> Survivors
    {
        get; set;

    } = default!;
    private const string SurvivorsFileName = "survivors.txt";
    public string RandomTeamString { get; set; } = default!;

    public SurvivorSelectionRandomizerPage()
	{
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"EVENT {m}");
            if (m == "SURVIVORS LOADED")
            {
                survivorsListView.SetBinding(ListView.ItemsSourceProperty, "Survivors");
            }

        });

        InitializeComponent();

        BindingContext = this;

        Task.Run(async () =>
        {
            await LoadSurvivorsAsync();
            WeakReferenceMessenger.Default.Send("SURVIVORS LOADED");

        });
    }

    protected override void OnAppearing()
    {
        //((Models.SurvivorSelectionRandomizer)BindingContext).LoadSurvivors();
        //survivorCollection.ItemsSource = ((Models.SurvivorSelectionRandomizer)BindingContext).Survivors;
    }

    private void SelectRandomTeam_Clicked(object sender, EventArgs e)
    {
        RandomTeamString = "";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            Random random = new Random();
            int index = random.Next(Survivors.Count);
            Survivor survivor = Survivors[index];
            sb.Append($"{i + 1}: ");
            sb.Append($"[{Survivor.SurvivorClassNames[(int)survivor.ClassType]}] ");
            sb.Append(survivor.Name);
            sb.Append('\n');
            Survivors.RemoveAt(index);
        }
        RandomTeamString += sb.ToString();
        randomTeamLabel.SetBinding(Label.TextProperty, "RandomTeamString");
        Console.WriteLine($"[SurvivorSelectionRandomizerPage/SelectRandomTeam_Clicked] RandomTeamString: {RandomTeamString}");

        Task.Run(async () =>
        {
            await LoadSurvivorsAsync();
            WeakReferenceMessenger.Default.Send("SURVIVORS LOADED");

        });
    }

    private async void MainPageButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    public async Task LoadSurvivorsAsync()
    {
        //Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] SurvivorsPath: {MainPage.Config.SurvivorsPath}");
        string appDataDir = FileSystem.Current.AppDataDirectory;
        string survivorsString = await FileUtility.LoadFile(Path.Combine(appDataDir,SurvivorsFileName));
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
            string line = lines[i].TrimStart().TrimEnd();
            Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] line {i}: {line}");
            string name = line.Remove(line.Length - 1, 1).TrimEnd();
            string classString = line.Remove(0, line.Length - 2);
            SurvivorClassType suvClass = SurvivorClassType.Civilian;
            if(int.TryParse(classString, out int classInt))
            {
                suvClass = (SurvivorClassType)classInt;
            }
            
            survivorList.Add(new Survivor(name, suvClass));
            Console.WriteLine($"[SurvivorSelectionRandomizerPage/LoadSurvivorsAsync] Added survivor: {name} {suvClass}");
        }

        Survivors = new ObservableCollection<Survivor>(survivorList.OrderBy(x => x.Name));

        return;

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
        string targetPath = Path.Combine(appDataDir, Path.GetFileName(sourcePath!));
        await FileUtility.SaveFile(survivorsString, targetPath);
        Console.WriteLine($"[SurvivorSelectionRandomizer/LoadSurvivors] Saved survivorsFile to \"{targetPath}\".");

        await LoadSurvivorsAsync();
    }
}