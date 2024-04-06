using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Text;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views.SpawnCards;

public partial class SpawnCardsConfigPage : ContentPage
{
    public static ObservableCollection<SpawnCardData> SpawnCards { get; set; } = new ObservableCollection<SpawnCardData>();
    public ObservableCollection<SpawnCardData> SpawnCardsLocal => SpawnCards;
    public static SpawnCardData[] SpawnCardsIncluded { get; set; } = [];
    public static int AbominationCount { get; set; }
    public static int ExtraActivationCount { get; set; }
    private const int AbominationBaseRatio = 24;
    private Color _storedColor;
    private readonly Color _warningColor = Color.FromRgb(255,0,0);

    public static Color CardBackgroundEnabled = new Color(0, 0, 255);
    public static Color CardBackgroundDisabled = new Color(255, 0, 0);
    public int SpawnCardsCount => SpawnCards.Where(x => x.Include).Count();
    public int SpawnCardsCountNormal => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Normal).Count();
    public int SpawnCardsCountMold => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Mold).Count();
    public int SpawnCardsCountAbomination => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Abomination).Count();
    public int SpawnCardsCountExtraActivation => SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.ExtraActivation).Count();

    public double SpawnCardsRatioNormal => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Normal).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);
    public double SpawnCardsRatioMold => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Mold).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);
    public double SpawnCardsRatioAbomination => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.Abomination).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);
    public double SpawnCardsRatioExtraActivation => Math.Round((double)SpawnCards.Where(x => x.Include && x.Type == SpawnCardDataType.ExtraActivation).Count() / SpawnCardsCount * 100, RatioDecimalPlaces);

    public static string EventSpawnCardDataLoaded = "SPAWN CARD DATA LOADED";
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

        _storedColor = SpawnCardsCountAbominationLabel.TextColor;

        InitializeSpawnCardControls();
    }

    public static async Task LoadSpawnCardDataAsync()
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(SpawnFileName);
        using StreamReader reader = new StreamReader(fileStream);
        string spawnCardDataString = await reader.ReadToEndAsync();
        bool commentBlockStarted = false;

        SpawnCards.Clear();
        string[] lines = spawnCardDataString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if(line.StartsWith("//"))
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
            if(commentBlockStarted)
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
        }

        #region Load SpawnCard User Config
        //string? config = await FileUtility.LoadFile($"{FileSystem.Current.AppDataDirectory}/{SpawnUserConfigFileName}");
        //if (string.IsNullOrEmpty(config) == false)
        //{
        //    Console.WriteLine($"config start");
        //    Console.WriteLine($"config: {config}");
        //    Console.WriteLine($"config end");

        //    foreach (SpawnCardData spawnCard in SpawnCards)
        //    {
        //        spawnCard.Include = false;
        //    }

        //    string[] linesConfig = config.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //    Dictionary<int, SpawnCardData> spawnCardsDict = SpawnCards.ToDictionary(x => x.CardNumber);
        //    Console.WriteLine($"linesConfig.Length: {linesConfig.Length}");
        //    for (int i = 0; i < linesConfig.Length; i++)
        //    {
        //        string line = linesConfig[i].Trim();
        //        Console.WriteLine($"config line {i}: {line}");
        //        if (int.TryParse(line, out int cardNumber))
        //        {
        //            spawnCardsDict[cardNumber].Include = true;
        //            Console.WriteLine($"cardNumber {cardNumber}");
        //        }
        //    }
        //}
        #endregion

        //WeakReferenceMessenger.Default.Send(EventSpawnCardDataLoaded);

        UpdateSpawnCardsIncluded();
    }

    public void OnCardIncludeCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender!;
        SpawnCardData card = (SpawnCardData)checkBox.BindingContext;
        (checkBox.Parent as Grid)!.BackgroundColor = card.Include ? SpawnCardsConfigPage.CardBackgroundEnabled : SpawnCardsConfigPage.CardBackgroundDisabled;
        UpdateSpawnCardsIncluded();
        WeakReferenceMessenger.Default.Send(EventSpawnCardIncludeChanged);

        UpdateCountRatioLabels();
    }

    private static void UpdateSpawnCardsIncluded()
    {
        SpawnCardsIncluded = SpawnCards.Where(x => x.Include).ToArray();
        AbominationCount = SpawnCardsIncluded.Where(x => x.Type == SpawnCardDataType.Abomination).Count();
        ExtraActivationCount = SpawnCardsIncluded.Where(x => x.Type == SpawnCardDataType.ExtraActivation).Count();
    }

    private void UpdateCountRatioLabels()
    {
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

    async void SaveButtonClickedAsync(object sender, EventArgs e)
    {
        //await SaveSpawnUserConfigAsync();
        StringBuilder sb = new StringBuilder(String.Empty);
        for (int i = 0; i < SpawnCardsIncluded.Length; i++)
        {
            SpawnCardData spawnCard = SpawnCardsIncluded[i];
            sb.Append($"{spawnCard.CardNumber}{Environment.NewLine}");
        }
        Console.WriteLine($"sb: {sb}");
        await FileUtility.SaveFile(sb.ToString(), $"{folder!.Path}/{SpawnUserConfigFileName}");
    }

    private async void SaveSpawnUserConfigAsync()
    {
        StringBuilder sb = new StringBuilder(String.Empty);
        for(int i = 0; i < SpawnCardsIncluded.Length; i++)
        {
            SpawnCardData spawnCard = SpawnCardsIncluded[i];
            sb.Append($"{spawnCard.CardNumber}{Environment.NewLine}");
        }
        Console.WriteLine($"sb: {sb}");
        await FileUtility.SaveFile(sb.ToString(), $"{FileSystem.Current.AppDataDirectory}/{SpawnUserConfigFileName}");
    }

    private Folder? folder;

    private async void PickFolderButtonClickedAsync(object sender, EventArgs e)
    {
        //SaveSpawnUserConfig();
        CancellationToken cancellationToken = new();
        folder = await FileUtility.PickFolder(cancellationToken);
    }

    private async void LoadButtonClickedAsync(object sender, EventArgs e)
    {
        string file = await FileUtility.PickFile();
        string config = await FileUtility.LoadFile(file);

        if (string.IsNullOrEmpty(config) == false)
        {
            Console.WriteLine($"config start");
            Console.WriteLine($"config: {config}");
            Console.WriteLine($"config end");

            List<string> linesConfig = config.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Console.WriteLine($"linesConfig.Count: {linesConfig.Count}");
            foreach (SpawnCardData spawnCard in SpawnCards)
            {
                bool include = linesConfig.IndexOf(spawnCard.CardNumber.ToString()) != -1;
                spawnCard.Include = include;
            }
            //Dictionary<int, SpawnCardData> spawnCardsDict = SpawnCards.ToDictionary(x => x.CardNumber);
            //Console.WriteLine($"linesConfig.Length: {linesConfig.Length}");
            //for (int i = 0; i < linesConfig.Length; i++)
            //{
            //    string line = linesConfig[i].Trim();
            //    Console.WriteLine($"config line {i}: {line}");
            //    if (int.TryParse(line, out int cardNumber))
            //    {
            //        spawnCardsDict[cardNumber].Include = true;
            //        Console.WriteLine($"cardNumber {cardNumber}");
            //    }
            //}
        }
        //WeakReferenceMessenger.Default.Send(EventSpawnCardDataLoaded);
        UpdateSpawnCardsIncluded();
        InitializeSpawnCardControls();
    }

    private void InitializeSpawnCardControls()
    {
        SpawnCardsVerticalStackLayout.Clear();

        for (int i = 0; i < SpawnCards.Count; i++) 
        {
            SpawnCardData spawnCard = SpawnCards[i];

            Grid grid = new Grid();
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() {Height = GridLength.Auto},
                new RowDefinition() {Height = GridLength.Auto}
            };

            Label titleLabel = new Label()
            {
                Text = spawnCard.Title,
                FontSize = 18,
                TextTransform = TextTransform.Uppercase,
                HorizontalTextAlignment = TextAlignment.Center
            };
            grid.Add(titleLabel);

            CheckBox includeCheckBox = new CheckBox()
            {
                StyleId = $"SpawnCard{spawnCard.CardNumber}CheckBox",
                IsChecked = spawnCard.Include,
                HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End},
                BindingContext = spawnCard
            };
            includeCheckBox.SetBinding(CheckBox.IsCheckedProperty, "Include");
            includeCheckBox.CheckedChanged += (sender, e) =>
            {
                // Perform required operation after examining e.Value
                includeCheckBox.IsChecked = e.Value;
                spawnCard.Include = includeCheckBox.IsChecked;
                UpdateSpawnCardsIncluded();
                UpdateCountRatioLabels();
                Console.WriteLine($"includeCheckBox.IsChecked: {includeCheckBox.StyleId} {includeCheckBox.IsChecked}");
                Console.WriteLine($"{SpawnCards[spawnCard.CardNumber - 1].CardNumber} {SpawnCards[spawnCard.CardNumber - 1].Title}: {SpawnCards[spawnCard.CardNumber - 1].Include}");
            };
            grid.Add(includeCheckBox);

            Grid subGrid = new Grid();
            subGrid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() {Width = GridLength.Star},
                new ColumnDefinition() {Width = GridLength.Auto}
            };

            Label typeLabel = new Label()
            {
                Text = spawnCard.TypeText,
                FontSize = 18,
                Margin = new Thickness(0,6,0,0)
            };
            subGrid.Add(typeLabel);

            Label cardNumberLabel = new Label()
            {
                Text = spawnCard.CardNumber.ToString(),
                FontSize = 18,
                Margin = new Thickness(0, 0, 6, 0),
                HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End },
                HorizontalTextAlignment = TextAlignment.End,
                BindingContext = spawnCard
            };
            cardNumberLabel.SetBinding(Label.TextProperty, "CardNumber", BindingMode.Default, null, "{0:D2}");
            subGrid.Add(cardNumberLabel, column:1);

            if(spawnCard.HasText)
            {
                Label textLabel = new Label()
                {
                    Text = spawnCard.CardNumber.ToString(),
                    FontSize = 18,
                    TextType = TextType.Html,
                    BindingContext = spawnCard
                };
                textLabel.SetBinding(Label.TextProperty, "Text");
                subGrid.Add(textLabel, row: 2);
            }

            grid.Add(subGrid, row:1);

            SpawnCardsVerticalStackLayout.Add(grid);
        }
    }
}