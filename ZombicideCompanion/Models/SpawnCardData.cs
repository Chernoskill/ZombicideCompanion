using System.Text;
using ZombicideCompanion.Views.SpawnCards;

namespace ZombicideCompanion.Models
{
    public class SpawnCardData
    {
        private const string EmptyString = "n/A";
        public int CardNumber {  get; set; }
        public SpawnCardDataType Type { get; set; } = SpawnCardDataType.Normal;
        private string _title = default!;
        public string Title
        {
            get
            {
                return Type == SpawnCardDataType.ExtraActivation ? $"{_title}: {(Activations.Length > 1 ? Activations[1] : EmptyString)}" : _title;
            }
            set
            {
                _title = value;
            }
        }

        private string _text = default!;
        public string Text 
        { 
            get
            {
                return Type == SpawnCardDataType.ExtraActivation ? $"Blau: {(Activations.Length > 0 ? Activations[0] : EmptyString)} | Gelb: {(Activations.Length > 1 ? Activations[1] : EmptyString)} | Orange: {(Activations.Length > 2 ? Activations[2] : EmptyString)} | Rot: {(Activations.Length > 3 ? Activations[3] : EmptyString)}" : _text;
            }
            set
            {
                _text = value;
            }
        }
        public bool HasText => Text != null && !string.IsNullOrEmpty(Text);
        public int[] SpawnAmounts { get; set; } = default!;
        public string[] Activations { get; set; } = default!;
        public bool Include { get; set; } = true;
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
