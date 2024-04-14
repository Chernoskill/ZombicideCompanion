using CommunityToolkit.Mvvm.Messaging;
using ZombicideCompanion.Models;
using static Java.Util.Jar.Attributes;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardDrawerPage : ContentPage
{
    public List<int> SpawnCardIndices { get; set; } = new List<int>();
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
        ShuffleSpawnCards();
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
        SpawnCardIndicesDrawn.Add(index);
        SpawnCardIndicesDrawn.RemoveAt(0);
        SetupSpawnCardElements();

        UpdateSpawnCardsInDeckLabel();
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
        UpdateSpawnCardsInDeckLabel();
        SpawnCardTextVerticalLayout.Clear();
    }

    private void UpdateSpawnCardsInDeckLabel()
    {
        SpawnCardsInDeckLabel.Text = $"Karten im Deck: {SpawnCardIndices.Count} von {SpawnCardsIncluded.Count}";
    }

    private void SetupSpawnCardElements()
    {
        SpawnCardTextVerticalLayout.Clear();

        if (SpawnCardDrawn == null)
        {
            return;
        }

        Label titleLabel = new Label()
        {
            Text = SpawnCardDrawn.Title,
            TextTransform = TextTransform.Uppercase,
            StyleId = "SpawnCardDrawnTitleLabel",
            FontSize = 18,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };
        SpawnCardTextVerticalLayout.Add(titleLabel);

        if (SpawnCardDrawn.Type == SpawnCardDataType.Normal)
        {
            for (int i = SpawnCardDrawn.SpawnAmounts.Length - 1; i >= 0; i--)
            {
                Label label = new Label
                {
                    Text = SpawnCardDrawn.SpawnAmounts[i].ToString() + (i == (int)DangerLevel ? " <<" : ""),
                    TextColor = DangerLevelColors[i],
                    FontSize = 18,
                    FontAttributes = i == (int)DangerLevel ? FontAttributes.Bold : FontAttributes.None
                };
                SpawnCardTextVerticalLayout.Add(label);
            }
        }

        if (SpawnCardDrawn.Type == SpawnCardDataType.ExtraActivation)
        {
            for (int i = SpawnCardDrawn.Activations.Length - 1; i >= 0; i--)
            {
                Label label = new Label
                {
                    Text = SpawnCardDrawn.Activations[i],
                    TextColor = DangerLevelColors[i],
                    FontSize = 18,
                    FontAttributes = i == (int)DangerLevel ? FontAttributes.Bold : FontAttributes.None
                };
                SpawnCardTextVerticalLayout.Add(label);
            }
        }
        else if (SpawnCardDrawn.Type == SpawnCardDataType.Mold)
        {
            Label label = new Label
            {
                Text = $"x{SpawnCardDrawn.SpawnAmounts[0]}",
                FontSize = 18,
            };
            SpawnCardTextVerticalLayout.Add(label);
        }

        if (string.IsNullOrEmpty(SpawnCardDrawn.Text) == false)
        {
            Label label = new Label
            {
                Text = SpawnCardDrawn.Text,
                FontSize = 18,
                TextType = TextType.Html
            };
            SpawnCardTextVerticalLayout.Add(label);
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