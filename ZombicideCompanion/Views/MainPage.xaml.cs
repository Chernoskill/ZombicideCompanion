using System.Text;
using ZombicideCompanion.Models;
using ZombicideCompanion.Views.SpawnCards;
using ZombicideCompanion.Views.Survivors;

namespace ZombicideCompanion.Views;

public partial class MainPage : ContentPage
{
    public static ZoCoConfiguration Config { get; set; } = default!;

    public MainPage()
    {
        Task.Run(async () =>
        {
            await SurvivorsConfigPage.LoadSurvivorsAsync();
            await SpawnCardsConfigPage.LoadSpawnCardDataAsync();
        });
    }

    private static async Task SaveConfigAsync()
    {
        StringBuilder sb = new StringBuilder();
        if (Config == null)
        {
            Config = new ZoCoConfiguration();
        }
        //sb.AppendLine($"survivorspath={Config.SurvivorsPath}");
        //await FileUtility.SaveFile(sb.ToString(), configFilePath);
    }

    private static async Task LoadConfigAsync()
    {
        //string configString = await FileUtility.LoadFile(configFilePath);
        //if(string.IsNullOrEmpty(configString))
        //{
        //    Console.WriteLine("[MainPage/LoadConfigAsync] configString is null or empty.");
        //    return;
        //}
        //Config = new ZoCoConfiguration();
        //string[] lines = configString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //Console.WriteLine($"[MainPage/LoadConfigAsync] configString lines.Lenmgth = {lines.Length}");
        //for (int i = 0; i < lines.Length; i++)
        //{
        //    string line = lines[i].TrimStart().TrimEnd().ToLower();
        //    Console.WriteLine($"[MainPage/LoadConfigAsync] line {i}: {line}");
        //    if (line.StartsWith("survivorspath"))
        //    {
        //        Console.WriteLine($"[MainPage/LoadConfigAsync] line {i} is survivorspath line");
        //        string[] lineParts = line.Split('=');
        //        if(lineParts.Length > 1)
        //        {
        //            Config.SurvivorsPath = lineParts[1].TrimStart().TrimEnd();
        //            Console.WriteLine($"[MainPage/LoadConfigAsync] survivorspath line has value \"{Config.SurvivorsPath}\".");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"[MainPage/LoadConfigAsync] survivorspath line has no value.");
        //        }
        //    }
        //}
        //return;
    }
}