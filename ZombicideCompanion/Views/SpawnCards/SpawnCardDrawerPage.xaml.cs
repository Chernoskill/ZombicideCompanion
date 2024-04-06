using CommunityToolkit.Mvvm.Messaging;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardDrawerPage : ContentPage
{
    public List<int> SpawnCardIndices { get; set; } = new List<int>();
    private int _spawnCardIndicesCount;
    public int SpawnCardIndicesCount
    {
        get
        {
            return _spawnCardIndicesCount;
        }
        set
        {
            _spawnCardIndicesCount = value;
        }
    }
    public List<SpawnCardData> SpawnCardsIncluded = new List<SpawnCardData>();

    public SpawnCardData? SpawnCardDrawn { get; set; } = default!;
    private List<int> SpawnCardIndicesDrawn { get; set; } = new List<int>();
    public int SpawnCardIndex { get; set; }
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
    private Color[] DangerLevelColors = [new Color(0, 0, 255), new Color(255, 208, 0), new Color(255, 128, 0), new Color(255, 0, 0)];
    private List<View> _controls = new List<View>();

    public SpawnCardDrawerPage()
    {
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"{GetType()} EVENT {m}");
            if (m == SpawnCardsConfigPage.EventSpawnCardIncludeChanged)
            {
                ShuffleSpawnCards();
            }
        });

        InitializeComponent();

        BindingContext = this;

        DangerLevel = DangerLevel.Blue;
    }

    private void DrawSpawnCardButton_Clicked(object sender, EventArgs e)
    {
        DrawSpawnCard();
    }

    private void DrawSpawnCard()
    {
        if (SpawnCardsConfigPage.SpawnCards == null || SpawnCardsConfigPage.SpawnCards.Count == 0)
        {
            return;
        }

        if (SpawnCardIndices.Count == 0)
        {
            ShuffleSpawnCards();
        }
        int index = SpawnCardIndices[SpawnCardIndices.Count - 1];
        SpawnCardData spawnCard = SpawnCardsIncluded[index];

        SpawnCardDrawn = spawnCard;
        SpawnCardIndices.RemoveAt(SpawnCardIndices.Count - 1);
        SpawnCardIndicesCount = SpawnCardIndices.Count;
        SpawnCardIndicesDrawn.Add(index);
        SpawnCardIndicesDrawn.RemoveAt(0);
        SetupSpawnCardElements();
    }

    private void ShuffleSpawnCards()
    {
        SpawnCardsIncluded = SpawnCardsConfigPage.SpawnCards.Where(x => x.Include).ToList();
        List<int> spawnCardIndexPool = Enumerable.Range(0, SpawnCardsIncluded.Count).ToList();
        SpawnCardIndices.Clear();
        SpawnCardDataType cardTypeLast = SpawnCardDataType.Normal;
        int tries = 0;
        Random random = new Random();

        int abominationCount = SpawnCardsIncluded.Where(x => x.Type == SpawnCardDataType.Abomination).Count();
        int extraActivationCount = SpawnCardsIncluded.Where(x => x.Type == SpawnCardDataType.ExtraActivation).Count();
        bool preventDoubleAbominations = SpawnCardsIncluded.Count - abominationCount >= abominationCount * 2;
        bool preventDoubleExtraActivations = SpawnCardsIncluded.Count - extraActivationCount >= extraActivationCount * 2;

        //Console.WriteLine($"preventDoubleAbominations: {preventDoubleAbominations}, preventDoubleExtraActivations: {preventDoubleExtraActivations}");

        while (spawnCardIndexPool.Count > 0)
        {
            int index = random.Next(spawnCardIndexPool.Count);
            SpawnCardData spawnCard = SpawnCardsIncluded[spawnCardIndexPool[index]];
            //Console.WriteLine($"spawnCard.Type = {spawnCard.Type}, cardTypeLast: {cardTypeLast}");
            if ((preventDoubleAbominations && spawnCard.Type == SpawnCardDataType.Abomination && spawnCard.Type == cardTypeLast) || (preventDoubleExtraActivations && spawnCard.Type == SpawnCardDataType.ExtraActivation && spawnCard.Type == cardTypeLast))
            {
                //Console.WriteLine($"tries: {tries}");
                tries++;
                if (tries >= SpawnCardIndices.Count)
                {
                    //Console.WriteLine($"tries >= SpawnCardIndices.Count, shuffling cards.");
                    ShuffleSpawnCards();
                    return;
                }
                continue;
            }
            SpawnCardIndices.Add(spawnCardIndexPool[index]);
            spawnCardIndexPool.RemoveAt(index);
            cardTypeLast = spawnCard.Type;
            tries = 0;
        }

        //for (int i = 0; i < SpawnCardIndices.Count; i++)
        //{
        //    Console.WriteLine($"{i}: {SpawnCardsIncluded[SpawnCardIndices[i]].CardNumber} {SpawnCardsIncluded[SpawnCardIndices[i]].Title}");
        //}

        SpawnCardIndicesCount = SpawnCardIndices.Count;
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
                FontSize = 18,
                TextType = TextType.Html
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

    public void ShuffleSpawnCardsButton_Clicked(object sender, EventArgs e)
    {
        ShuffleSpawnCards();
    }
}