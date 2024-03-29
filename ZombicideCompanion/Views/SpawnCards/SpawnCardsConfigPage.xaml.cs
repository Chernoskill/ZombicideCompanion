using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardsConfigPage : ContentPage
{
    public static ObservableCollection<SpawnCardData> SpawnCards { get; set; } = new ObservableCollection<SpawnCardData>();
    public ObservableCollection<SpawnCardData> SpawnCardsLocal => SpawnCards;

    private const string SpawnFileName = "spawn.txt";
    public static string EventSpawnCardDataLoaded = "SPAWN CARD DATA LOADED";
    private static Dictionary<SpawnCardDataType,string> SpawnCardDataTypeStrings { get; set; } = new Dictionary<SpawnCardDataType, string>() 
    {
        { SpawnCardDataType.Normal, "Xeno-Brut" },
        { SpawnCardDataType.Abomination, "Monstrum" },
        { SpawnCardDataType.Mold, "Fäulnis-Brut" },
        { SpawnCardDataType.ExtraActivation, "Extra-Aktivierung" }
    };

    public SpawnCardsConfigPage()
	{
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"{GetType()} EVENT {m}");
            if (m == EventSpawnCardDataLoaded)
            {
                SpawnCardsListView.SetBinding(ListView.ItemsSourceProperty, "SpawnCardsLocal");
            }
        });

        InitializeComponent();

        BindingContext = this;

        SpawnCardsListView.SetBinding(ListView.ItemsSourceProperty, "SpawnCardsLocal");
    }

    public static async Task LoadSpawnCardDataAsync()
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(SpawnFileName);
        using StreamReader reader = new StreamReader(fileStream);
        string spawnCardDataString = await reader.ReadToEndAsync();

        SpawnCards.Clear();
        string[] lines = spawnCardDataString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            string[] parts = line.Split(';');
            SpawnCardData card = new SpawnCardData();

            //CardNumber
            if (int.TryParse(parts[0], out int cardNumber) == false)
            {
                Console.WriteLine($"[SpawnHandlerPage/LoadSpawnCardDataAsync] Could not parse CardNumber: \"{parts[0]}\"");
                continue;
            }
            card.CardNumber = cardNumber;

            //Type
            if (int.TryParse(parts[1], out int type) == false)
            {
                Console.WriteLine($"[SpawnHandlerPage/LoadSpawnCardDataAsync] Could not parse Type: \"{parts[1]}\"");
                continue;
            }
            card.Type = (SpawnCardDataType)type;

            //Title
            card.Title = parts[2];

            //Text
            card.Text = parts[3];

            //SpawnAmount
            switch (card.Type)
            {
                case SpawnCardDataType.Normal:
                    {
                        card.SpawnAmounts = [int.Parse(parts[4]), int.Parse(parts[5]), int.Parse(parts[6]), int.Parse(parts[7])];
                        break;
                    }
                case SpawnCardDataType.Mold:
                    {
                        card.SpawnAmounts = [int.Parse(parts[4])];
                        break;
                    }
                case SpawnCardDataType.ExtraActivation:
                    {
                        card.Activations = [parts[4], parts[5], parts[6], parts[7]];
                        break;
                    }
            }
            card.TypeText = SpawnCardDataTypeStrings[card.Type];
            SpawnCards.Add(card);
        }
        WeakReferenceMessenger.Default.Send(EventSpawnCardDataLoaded);
    }

    private void AddControls()
    {
        if(SpawnCards == null)
        {
            return;
        }

        foreach(SpawnCardData card in SpawnCards)
        {

        }
    }

}