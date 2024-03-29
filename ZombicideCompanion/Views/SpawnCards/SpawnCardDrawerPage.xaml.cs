using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardDrawerPage : ContentPage
{
    //public ObservableCollection<SpawnCardData> SpawnCards { get; set; } = new ObservableCollection<SpawnCardData>();
    public List<int> SpawnCardIndices { get; set; } = default!;
    public SpawnCardData? SpawnCardDrawn { get; set; } = default!;
    private DangerLevel _dangerLevel;
    public DangerLevel DangerLevel
    {
        get
        {
            return _dangerLevel;
        }
        set
        {
            if (value is < 0 or > (DangerLevel)3)
            {
                return;
            }
            _dangerLevel = value;
            DangerLevelLabel.Text = DangerLevelStrings[(int)value];
            DangerLevelLabel.TextColor = DangerLevelColors[(int)value];
            SetupSpawnCardElements();
        }
    }
    private string[] DangerLevelStrings { get; set; } = ["Blau", "Gelb", "Orange", "Rot"];
    //private const string SpawnFileName = "spawn.txt";
    private Color[] DangerLevelColors = [new Color(0, 0, 255), new Color(255, 208, 0), new Color(255, 128, 0), new Color(255, 0, 0)];
    //private string EventSpawnCardDataLoaded = "SPAWN CARD DATA LOADED";
    private List<View> _controls = new List<View>();

    public SpawnCardDrawerPage()
    {
        //WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        //{
        //    Console.WriteLine($"EVENT {m}");
        //    if (m == EventSpawnCardDataLoaded)
        //    {

        //    }
        //});

        InitializeComponent();

        BindingContext = this;

        DangerLevel = DangerLevel.Blue;

        //Task.Run(async () =>
        //{
        //    await LoadSpawnCardDataAsync();
        //    WeakReferenceMessenger.Default.Send(EventSpawnCardDataLoaded);
        //});
    }

    //private async Task LoadSpawnCardDataAsync()
    //{
    //    using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(SpawnFileName);
    //    using StreamReader reader = new StreamReader(fileStream);
    //    string spawnCardDataString = await reader.ReadToEndAsync();

    //    SpawnCards.Clear();
    //    string[] lines = spawnCardDataString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        string line = lines[i].Trim();
    //        string[] parts = line.Split(';');
    //        SpawnCardData card = new SpawnCardData();

    //        //CardNumber
    //        if (int.TryParse(parts[0], out int cardNumber) == false)
    //        {
    //            Console.WriteLine($"[SpawnHandlerPage/LoadSpawnCardDataAsync] Could not parse CardNumber: \"{parts[0]}\"");
    //            continue;
    //        }
    //        card.CardNumber = cardNumber;

    //        //Type
    //        if (int.TryParse(parts[1], out int type) == false)
    //        {
    //            Console.WriteLine($"[SpawnHandlerPage/LoadSpawnCardDataAsync] Could not parse Type: \"{parts[1]}\"");
    //            continue;
    //        }
    //        card.Type = (SpawnCardDataType)type;

    //        //Title
    //        card.Title = parts[2];

    //        //Text
    //        card.Text = parts[3];

    //        //SpawnAmount
    //        switch (card.Type)
    //        {
    //            case SpawnCardDataType.Normal:
    //                {
    //                    card.SpawnAmounts = [int.Parse(parts[4]), int.Parse(parts[5]), int.Parse(parts[6]), int.Parse(parts[7])];
    //                    break;
    //                }
    //            case SpawnCardDataType.Mold:
    //                {
    //                    card.SpawnAmounts = [int.Parse(parts[4])];
    //                    break;
    //                }
    //            case SpawnCardDataType.ExtraActivation:
    //                {
    //                    card.Activations = [parts[4], parts[5], parts[6], parts[7]];
    //                    break;
    //                }
    //        }
    //        SpawnCards.Add(card);
    //    }
    //}

    private void DrawSpawnCardButton_Clicked(object sender, EventArgs e)
    {
        DrawSpawnCard();
    }

    private void DrawSpawnCard()
    {
        if(SpawnCardsConfigPage.SpawnCards == null || SpawnCardsConfigPage.SpawnCards.Count == 0)
        {
            return;
        }
        if (SpawnCardIndices == null || SpawnCardIndices.Count == 0)
        {
            SpawnCardIndices = Enumerable.Range(0, SpawnCardsConfigPage.SpawnCards.Count).ToList();
        }
        Random random = new Random();
        int index = random.Next(SpawnCardIndices.Count);
        SpawnCardData spawnCard = SpawnCardsConfigPage.SpawnCards[SpawnCardIndices[index]];
        SpawnCardDrawn = spawnCard;
        SpawnCardIndices.RemoveAt(index);

        SetupSpawnCardElements();
    }

    private void SetupSpawnCardElements()
    {
        if (SpawnCardDrawn == null)
        {
            return;
        }
        SpawnCardDrawnTitleLabel.Text = SpawnCardDrawn.Title;
        SpawnCardDrawnTitleLabel.TextTransform = TextTransform.Uppercase;

        foreach (View control in _controls)
        {
            RootLayout.Remove(control);
        }

        if (SpawnCardDrawn.Type == SpawnCardDataType.Normal)
        {
            for (int i = SpawnCardDrawn.SpawnAmounts.Length - 1; i >= 0; i--)
            {
                Label label = new Label
                {
                    Parent = RootLayout,
                    Text = SpawnCardDrawn.SpawnAmounts[0].ToString(),
                    TextColor = DangerLevelColors[i],
                    FontSize = 18,
                    FontAttributes = i == (int)DangerLevel ? FontAttributes.Bold : FontAttributes.None
                };
                RootLayout.Add(label);
                _controls.Add(label);
            }
        }
        if (SpawnCardDrawn.Type == SpawnCardDataType.ExtraActivation)
        {
            for (int i = SpawnCardDrawn.Activations.Length - 1; i >= 0; i--)
            {
                Label label = new Label
                {
                    Parent = RootLayout,
                    Text = SpawnCardDrawn.Activations[0],
                    TextColor = DangerLevelColors[i],
                    FontSize = 18,
                    FontAttributes = i == (int)DangerLevel ? FontAttributes.Bold : FontAttributes.None
                };
                RootLayout.Add(label);
                _controls.Add(label);
            }
        }
        else if (SpawnCardDrawn.Type == SpawnCardDataType.Mold)
        {
            Label label = new Label
            {
                Parent = RootLayout,
                Text = $"x{SpawnCardDrawn.SpawnAmounts[0]}",
                FontSize = 18,
            };
            RootLayout.Add(label);
            _controls.Add(label);
        }
        if (string.IsNullOrEmpty(SpawnCardDrawn.Text) == false)
        {
            Label label = new Label
            {
                Parent = RootLayout,
                Text = SpawnCardDrawn.Text,
                FontSize = 18
            };
            RootLayout.Add(label);
            _controls.Add(label);
        }
    }

    public void DangerLevelIncreaseButton_Clicked(object sender, EventArgs e)
    {
        DangerLevel++;
    }

    public void DangerLevelDecreaseButton_Clicked(object sender, EventArgs e)
    {
        DangerLevel--;
    }
}