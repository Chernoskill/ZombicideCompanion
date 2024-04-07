using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardsConfigPage : ContentPage
{
    public static SpawnCardsConfigPage? Instance { get; set; }
    public static ObservableCollection<SpawnCardData> SpawnCards { get; set; } = new ObservableCollection<SpawnCardData>();
    public ObservableCollection<SpawnCardData> SpawnCardsLocal => SpawnCards;
    public static SpawnCardData[] SpawnCardsIncluded { get; set; } = [];
    public static int AbominationCount { get; set; }
    public static int ExtraActivationCount { get; set; }
    private const int AbominationBaseRatio = 24;
    private Color _storedColor;
    private readonly Color _warningColor = Color.FromRgb(255, 0, 0);

    public int SpawnCardsCount => SpawnCards.Where(x => x.Include).Count();
    public int SpawnCardsCountNormal => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Normal).Count();
    public int SpawnCardsCountMold => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Mold).Count();
    public int SpawnCardsCountAbomination => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Abomination).Count();
    public int SpawnCardsCountExtraActivation => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.ExtraActivation).Count();

    public double SpawnCardsRatioNormal => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Normal).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);
    public double SpawnCardsRatioMold => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Mold).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);
    public double SpawnCardsRatioAbomination => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Abomination).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);
    public double SpawnCardsRatioExtraActivation => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.ExtraActivation).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);

    public static string EventSpawnCardIncludeChanged = "SPAWN CARD INCLUDE CHANGED";

    private const int RatioDecimalPlaces = 0;
    private const string SpawnFileName = "spawn.txt";
    private const string SpawnUserConfigFileName = "spawnuserconfig.txt";
    private static Dictionary<SpawnCardDataType, string> SpawnCardDataTypeStrings { get; set; } = new Dictionary<SpawnCardDataType, string>()
    {
        { SpawnCardDataType.Normal, "Xeno-Brut" },
        { SpawnCardDataType.Abomination, "Monstrum" },
        { SpawnCardDataType.Mold, "Fäulnis-Brut" },
        { SpawnCardDataType.ExtraActivation, "Extra-Aktivierung" }
    };

    public SpawnCardsConfigPage()
    {
        InitializeComponent();

        BindingContext = this;

        Instance = this;

        _storedColor = SpawnCardsCountAbominationLabel.TextColor;

        UpdateSpawnCardsIncluded();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            InitializeSpawnCardControls();
        });
    }

    public static async Task LoadSpawnCardDataAsync()
    {
        Console.WriteLine($"[SpawnCardsConfigPage/LoadSpawnCardDataAsync]");
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(SpawnFileName);
        using StreamReader reader = new StreamReader(fileStream);
        string spawnCardDataString = await reader.ReadToEndAsync();
        bool commentBlockStarted = false;

        SpawnCards.Clear();
        string[] lines = spawnCardDataString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"[SpawnCardsConfigPage/LoadSpawnCardDataAsync] lines.Length: {lines.Length}");
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("//"))
            {
                continue;
            }
            if (line.StartsWith("*/"))
            {
                commentBlockStarted = false;
                continue;
            }
            if (line.StartsWith("/*") && commentBlockStarted == false)
            {
                commentBlockStarted = true;
                continue;
            }
            if (commentBlockStarted)
            {
                continue;
            }
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
            //Console.WriteLine($"[SpawnCardsConfigPage/LoadSpawnCardDataAsync] SpawnCardData added: {card.CardNumber} {card.Title}");
        }

        await LoadSpawnUserConfigAsync();
    }

    public static async Task LoadSpawnUserConfigAsync()
    {
        Console.WriteLine($"[SpawnCardsConfigPage/LoadSpawnUserConfigAsync]");
        string? config = await FileUtility.LoadFile($"{FileSystem.Current.AppDataDirectory}/{SpawnUserConfigFileName}");
        if (string.IsNullOrEmpty(config))
        {
            Console.WriteLine("[SpawnCardsConfigPage/LoadSpawnUserConfigAsync] Spawncard config couldn't be loaded or is empty.");
            UpdateSpawnCardsIncluded();
            return;
        }

        List<string> linesConfig = config.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (SpawnCardData spawnCard in SpawnCards)
        {
            bool include = linesConfig.IndexOf(spawnCard.CardNumber.ToString()) != -1;
            spawnCard.Include = include;
        }
    }

    private async Task SaveSpawnUserConfigAsync()
    {
        Console.WriteLine($"[SpawnCardsConfigPage/SaveSpawnUserConfigAsync]");
        StringBuilder sb = new StringBuilder(String.Empty);
        for (int i = 0; i < SpawnCardsIncluded.Length; i++)
        {
            SpawnCardData spawnCard = SpawnCardsIncluded[i];
            sb.Append($"{spawnCard.CardNumber}{Environment.NewLine}");
        }
        await FileUtility.SaveFile(sb.ToString(), $"{FileSystem.Current.AppDataDirectory}/{SpawnUserConfigFileName}");
    }

    private static void UpdateSpawnCardsIncluded()
    {
        Console.WriteLine($"[SpawnCardsConfigPage/UpdateSpawnCardsIncluded]");
        SpawnCardsIncluded = SpawnCards.Where(x => x.Include).ToArray();
        Console.WriteLine($"[SpawnCardsConfigPage/UpdateSpawnCardsIncluded] SpawnCardsIncluded.Count: {SpawnCardsIncluded.Length}");
        AbominationCount = SpawnCardsIncluded.Where(x => x.Type == SpawnCardDataType.Abomination).Count();
        ExtraActivationCount = SpawnCardsIncluded.Where(x => x.Type == SpawnCardDataType.ExtraActivation).Count();

        Instance?.UpdateCountRatioLabels();
    }

    private void UpdateCountRatioLabels()
    {
        Console.WriteLine($"[SpawnCardsConfigPage/UpdateCountRatioLabels]");
        SpawnCardsCountLabel.Text = SpawnCardsCount.ToString();
        SpawnCardsCountNormalLabel.Text = SpawnCardsCountNormal.ToString();
        SpawnCardsCountMoldLabel.Text = SpawnCardsCountMold.ToString();
        SpawnCardsCountAbominationLabel.Text = SpawnCardsCountAbomination.ToString();
        SpawnCardsCountExtraActivationLabel.Text = SpawnCardsCountExtraActivation.ToString();

        SpawnCardsRatioNormalLabelMain.Text = SpawnCardsRatioNormal.ToString();
        SpawnCardsRatioMoldLabelMain.Text = SpawnCardsRatioMold.ToString();
        SpawnCardsRatioAbominationLabelMain.Text = SpawnCardsRatioAbomination.ToString();
        SpawnCardsRatioExtraActivationLabelMain.Text = SpawnCardsRatioExtraActivation.ToString();

        bool canPreventDoubleAbominations = SpawnCardsIncluded.Length - AbominationCount >= AbominationCount * 2;
        bool canPreventDoubleExtraActivations = SpawnCardsIncluded.Length - ExtraActivationCount >= ExtraActivationCount * 2;

        SpawnCardsCountAbominationLabel.TextColor = canPreventDoubleAbominations ? _storedColor : _warningColor;
        SpawnCardsRatioAbominationLabel.TextColor = canPreventDoubleAbominations ? _storedColor : _warningColor;

        if (canPreventDoubleExtraActivations == false)
        {
            SpawnCardsCountExtraActivationLabel.TextColor = canPreventDoubleExtraActivations ? _storedColor : _warningColor;
            SpawnCardsRatioExtraActivationLabel.TextColor = canPreventDoubleExtraActivations ? _storedColor : _warningColor;
        }

        double abominationRatioDeviation = SpawnCardsRatioAbomination - AbominationBaseRatio;

        string abominationRatioDeviationString = string.Empty;

        if (abominationRatioDeviation != 0)
        {
            abominationRatioDeviationString = " (" + (abominationRatioDeviation > 0 ? "+" : "") + abominationRatioDeviation.ToString() + "%)";
        }

        SpawnCardsRatioAbominationLabelDeviation.Text = abominationRatioDeviationString;
    }

    private void InitializeSpawnCardControls()
    {
        Console.WriteLine($"[SpawnCardsConfigPage/InitializeSpawnCardControls]");
        SpawnCardsVerticalStackLayout.Clear();
        Console.WriteLine($"[SpawnCardsConfigPage/InitializeSpawnCardControls] SpawnCards.Count: {SpawnCards.Count}");

        BatchBegin();

        for (int i = 0; i < SpawnCards.Count; i++)
        {
            SpawnCardData spawnCard = SpawnCards[i];

            //Console.WriteLine($"[SpawnCardsConfigPage/InitializeSpawnCardControls] spawnCard: {spawnCard.CardNumber} {spawnCard.Title}");

            Grid grid = new Grid();
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() {Height = GridLength.Auto},
                new RowDefinition() {Height = GridLength.Auto}
            };

            Label cardNumberLabel = new Label()
            {
                Text = spawnCard.CardNumber.ToString(),
                FontSize = 14,
                Margin = new Thickness(6, 0, 0, 0),
                HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Start },
                BindingContext = spawnCard
            };
            cardNumberLabel.SetBinding(Label.TextProperty, "CardNumber", stringFormat: "{0:D2}");
            grid.Add(cardNumberLabel, row: 0);

            Label typeLabel = new Label()
            {
                Text = spawnCard.TypeText,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(6, 0, 0, 0)
            };
            grid.Add(typeLabel, row: 0);

            Label titleLabel = new Label()
            {
                Text = spawnCard.Title,
                FontSize = 18,
                TextTransform = TextTransform.Uppercase,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            grid.Add(titleLabel, row: 1);

            CheckBox includeCheckBox = new CheckBox()
            {
                StyleId = $"SpawnCard{spawnCard.CardNumber}CheckBox",
                Margin = new Thickness(0, -2, 6, 0),
                IsChecked = spawnCard.Include,
                HorizontalOptions = LayoutOptions.End,
                BindingContext = spawnCard
            };
            includeCheckBox.SetBinding(CheckBox.IsCheckedProperty, "Include");
            includeCheckBox.CheckedChanged += (sender, e) =>
            {
                // Perform required operation after examining e.Value
                includeCheckBox.IsChecked = e.Value;
                spawnCard.Include = includeCheckBox.IsChecked;
                Task.Run(SaveSpawnUserConfigAsync);
                UpdateSpawnCardsIncluded();
                UpdateCountRatioLabels();
                WeakReferenceMessenger.Default.Send(EventSpawnCardIncludeChanged);
            };
            grid.Add(includeCheckBox, row: 0);

            if (spawnCard.HasText)
            {
                grid.AddRowDefinition(new RowDefinition() { Height = GridLength.Auto });
                Label textLabel = new Label()
                {
                    Text = spawnCard.CardNumber.ToString(),
                    FontSize = 14,
                    TextType = TextType.Html,
                    BindingContext = spawnCard
                };
                textLabel.SetBinding(Label.TextProperty, "Text");
                grid.Add(textLabel, row: 2);
            }

            Border border = new Border()
            {
                Content = grid,
                StrokeThickness = 2.0,
                Padding = new Thickness(2, 2, 2, 2),
                Margin = new Thickness(0, 4, 0, 4),
            };
            SpawnCardsVerticalStackLayout.Add(border);
        }
        BatchCommit();
    }
}