using System.Globalization;

namespace ZombicideCompanion.Views.SpawnCards
{
    public class ColorChangedConverter : IValueConverter
    {
        public ColorChangedConverter()
        {
            Console.WriteLine($"[ColorChangedConverter/ColorChangedConverter] ");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine($"[ColorChangedConverter/Convert] value: {value}");
            if ((bool)value == true)
            {
                return SpawnCardsConfigPage.CardBackgroundEnabled;
            }
            else
            {
                return SpawnCardsConfigPage.CardBackgroundEnabled;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
