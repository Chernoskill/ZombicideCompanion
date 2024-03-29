using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardsConfigPage : ContentPage
{
    public static ObservableCollection<SpawnCardData> SpawnCards { get; set; } = new ObservableCollection<SpawnCardData>();
    public ObservableCollection<SpawnCardData> SpawnCardsLocal => SpawnCards;
    public static Color CardBackgroundEnabled = new Color(96, 0, 255);
    public static Color CardBackgroundDisabled = new Color(66, 0, 176);
    public int SpawnCardsCount => SpawnCards.Count();
    public int SpawnCardsCountNormal => SpawnCards.Where(x => x.Type == SpawnCardDataType.Normal).Count();
    public int SpawnCardsCountMold => SpawnCards.Where(x => x.Type == SpawnCardDataType.Mold).Count();
    public int SpawnCardsCountAbomination => SpawnCards.Where(x => x.Type == SpawnCardDataType.Abomination).Count();
    public int SpawnCardsCountExtraActivation => SpawnCards.Where(x => x.Type == SpawnCardDataType.ExtraActivation).Count();

    public double SpawnCardsRatioNormal => Math.Round((double)SpawnCards.Where(x => x.Type == SpawnCardDataType.Normal).Count() / SpawnCards.Count() * 100, 2);
    public double SpawnCardsRatioMold => Math.Round((double)SpawnCards.Where(x => x.Type == SpawnCardDataType.Mold).Count() / SpawnCards.Count() * 100, 2);
    public double SpawnCardsRatioAbomination => Math.Round((double)SpawnCards.Where(x => x.Type == SpawnCardDataType.Abomination).Count() / SpawnCards.Count() * 100, 2);
    public double SpawnCardsRatioExtraActivation => Math.Round((double)SpawnCards.Where(x => x.Type == SpawnCardDataType.ExtraActivation).Count() / SpawnCards.Count() * 100, 2);
    
    public static string EventSpawnCardDataLoaded = "SPAWN CARD DATA LOADED";

    private const string SpawnFileName = "spawn.txt";
    private static Dictionary<SpawnCardDataType, string> SpawnCardDataTypeStrings { get; set; } = new Dictionary<SpawnCardDataType, string>()
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

            //StringBuilder sb = new StringBuilder();
            //string[] textlines = parts[3].Split("<br>");
            //for (int textlineIndex = 0; textlineIndex < textlines.Length; textlineIndex++)
            //{
            //    sb.Append(textlines[textlineIndex]);
            //    if (textlineIndex < textlines.Length - 1)
            //    {
            //        sb.Append('\n');
            //    }
            //}
            //card.Text = sb.ToString();
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
        if (SpawnCards == null)
        {
            return;
        }

        foreach (SpawnCardData card in SpawnCards)
        {

        }
    }

    public void SpawnCardTapped(object o, ItemTappedEventArgs e)
    {
        //int index = e.ItemIndex;
        SpawnCardData? card = e.Item as SpawnCardData;
        if (card == null)
        {
            return;
        }
        card.Include = !card.Include;
        Console.WriteLine($"[SpawnCardsConfigPage/SpawnCardTapped] {card.CardNumber}: {card.Title} Include: {card.Include}");
    }
}