using System.Text;
using ZombicideCompanion.Models;
using ZombicideCompanion.Views.SpawnCards;
using ZombicideCompanion.Views.Survivors;

namespace ZombicideCompanion.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        Task.Run(async () =>
        {
            await SurvivorsConfigPage.LoadSurvivorsAsync();
            await SpawnCardsConfigPage.LoadSpawnCardDataAsync();
        });
    }

}