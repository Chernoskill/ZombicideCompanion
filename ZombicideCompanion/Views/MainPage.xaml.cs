using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using ZombicideCompanion.Models;
using static Android.Provider.Contacts;

namespace ZombicideCompanion.Views;

public partial class MainPage : ContentPage
{
    public static ZoCoConfiguration Config { get; set; } = default!;

    public MainPage()
	{
        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            Console.WriteLine($"EVENT {m}");
            if (m == "DOWNLOAD COMPLETED")
            {
                Console.WriteLine("Event accepted.");
            }

        });

        Console.WriteLine($"FileSystem.AppDataDirectory: {FileSystem.AppDataDirectory}");

        //Task.Run(async () =>
        //{
        //    Console.WriteLine("Loading config");
        //    await LoadConfigAsync();
        //    Console.WriteLine("Loaded config");
        //    WeakReferenceMessenger.Default.Send("DOWNLOAD COMPLETED");

        //});
    }

    private static async Task SaveConfigAsync()
    {
        StringBuilder sb = new StringBuilder();
        if(Config == null)
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

    private async void SurvivorRandomizerButton_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new SurvivorSelectionRandomizerPage((IFilePicker)Handler!.MauiContext!.Services.GetService(typeof(IFilePicker))!));
        await Navigation.PushAsync(new SurvivorSelectionRandomizerPage());
    }
}