using System.Text;
using ZombicideCompanion.Views.SpawnCards;

namespace ZombicideCompanion.Models
{
    public class SpawnCardData
    {
        public int CardNumber {  get; set; }
        public SpawnCardDataType Type { get; set; } = SpawnCardDataType.Normal;
        public string Title { get; set; } = default!;
        public string Text { get; set; } = default!;
        public bool HasText => Text != null && !string.IsNullOrEmpty(Text);
        public int[] SpawnAmounts { get; set; } = default!;
        public string[] Activations { get; set; } = default!;
        public bool Include { get; set; } = true;
        public Color Color { get; set; } = SpawnCardsConfigPage.CardBackgroundEnabled;
        public string TypeText { get; set; } = default!;

        public SpawnCardData() { }
        public SpawnCardData(int cardNumber, SpawnCardDataType type, string title)
        {
            CardNumber = cardNumber;
            Type = type;
            Title = title;
        }

        public SpawnCardData(int cardNumber, SpawnCardDataType type, string title, int spawnAmount)
        {
            CardNumber = cardNumber;
            Type = type;
            Title = title;
            SpawnAmounts = [spawnAmount];
        }

        public SpawnCardData(int cardNumber, SpawnCardDataType type, string title, int spawnAmountBlue, int spawnAmountYellow, int spawnAmountOrange, int spawnAmountRed)
        {
            CardNumber = cardNumber;
            Type = type;
            Title = title;
            SpawnAmounts = [spawnAmountBlue, spawnAmountYellow, spawnAmountOrange, spawnAmountRed];
        }

        public SpawnCardData(int cardNumber, SpawnCardDataType type, string title, string activationBlue, string activationOrange, string activationRed, string activationYellow)
        {
            CardNumber = cardNumber;
            Type = type;
            Title = title;
            Activations = [activationBlue, activationOrange, activationRed, activationYellow];
        }
    }
}
