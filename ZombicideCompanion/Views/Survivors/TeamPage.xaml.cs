using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.Survivors;

public partial class TeamPage : ContentPage
{
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
                    int indexSurvivors = SurvivorsConfigPage.Survivors.ToList().FindIndex(x => x.Name == RandomTeam[RandomTeam.Count - 1].Name);
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
                    Survivor survivor = SurvivorsConfigPage.Survivors[SurvivorIndices[index]];
                    RandomTeam.Add(survivor);
                    SurvivorIndices.RemoveAt(index);
                }
            }
        }
    }

    public TeamPage()
    {
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"{GetType()} EVENT {m}");
            if (m == SurvivorsConfigPage.EventSurvivorsLoaded)
            {
               
            }
        });

        InitializeComponent();

        BindingContext = this;

        randomTeamListView.SetBinding(ListView.ItemsSourceProperty, "RandomTeam");
        teamSizeLabel.SetBinding(Label.TextProperty, "TeamSize");

        SurvivorIndices = Enumerable.Range(0, SurvivorsConfigPage.Survivors.Count).ToList();
        TeamSize = 6;
        RandomizeTeam();
    }

    private void RandomizeTeam_Clicked(object sender, EventArgs e)
    {
        RandomizeTeam();
    }

    private void RandomizeTeam()
    {
        SurvivorIndices = Enumerable.Range(0, SurvivorsConfigPage.Survivors.Count).ToList();
        if (SurvivorIndices.Count == 0)
        {
            return;
        }
        RandomTeam.Clear();
        Random random = new Random();
        for (int i = 0; i < TeamSize; i++)
        {
            int index = random.Next(SurvivorIndices.Count);
            Survivor survivor = SurvivorsConfigPage.Survivors[SurvivorIndices[index]];
            RandomTeam.Add(survivor);
            SurvivorIndices.RemoveAt(index);
        }
    }
   
    private void ReselectSurvivorButton_Clicked(object sender, EventArgs e)
    {
        if(SurvivorsConfigPage.Survivors.Count <= SurvivorIndices.Count)
        {
            return;
        }
        Button button = (Button)sender;
        Survivor survivorRemove = (Survivor)button.BindingContext;
        int indexRandomTeam = RandomTeam.ToList().FindIndex(x => x.Name == survivorRemove.Name);
        RandomTeam.RemoveAt(indexRandomTeam);

        Random random = new Random();
        int index = random.Next(SurvivorIndices.Count);
        Survivor survivorNew = SurvivorsConfigPage.Survivors[SurvivorIndices[index]];
        RandomTeam.Insert(indexRandomTeam, survivorNew);
        SurvivorIndices.RemoveAt(index);

        int indexSurvivors = SurvivorsConfigPage.Survivors.ToList().FindIndex(x => x.Name == survivorRemove.Name);
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