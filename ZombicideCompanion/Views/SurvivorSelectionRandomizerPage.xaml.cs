using System.Text;
using ZombicideCompanion.Models;

namespace ZombicideCompanion.Views;

public partial class SurvivorSelectionRandomizerPage : ContentPage
{
	public SurvivorSelectionRandomizerPage()
	{
		InitializeComponent();
        BindingContext = new Models.SurvivorSelectionRandomizer();
    }

    protected override void OnAppearing()
    {
        ((Models.SurvivorSelectionRandomizer)BindingContext).LoadSurvivors();
        survivorCollection.ItemsSource = ((Models.SurvivorSelectionRandomizer)BindingContext).Survivors;
    }

    private void SelectRandomSurvivor_Clicked(object sender, EventArgs e)
    {
        ((Models.SurvivorSelectionRandomizer)BindingContext).SelectRandomSurvivor();
        RandomSurvivorLabel.Text = ((Models.SurvivorSelectionRandomizer)BindingContext).RandomSurvivor.Name;
    }

    private void SelectRandomTeam_Clicked(object sender, EventArgs e)
    {
        if(((Models.SurvivorSelectionRandomizer)BindingContext).Survivors.Count < ((Models.SurvivorSelectionRandomizer)BindingContext).SurvivorCount)
        {
            ResetSurvivors();
        }
        StringBuilder sb = new StringBuilder();
        RandomTeamLabel.Text = string.Empty;
        for (int i = 0; i < 6; i++)
        {
            Survivor survivor = ((SurvivorSelectionRandomizer)BindingContext).SelectRandomSurvivor()!;
            sb.Append($"{i + 1}: ");
            sb.Append($"[{Survivor.SurvivorClassNames[(int)survivor.ClassType]}] ");
            sb.Append(survivor.Name);
            sb.Append('\n');
        }
        RandomTeamLabel.Text += sb.ToString();
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        ResetSurvivors();
    }

    private void ResetSurvivors()
    {
        ((Models.SurvivorSelectionRandomizer)BindingContext).LoadSurvivors();
        survivorCollection.ItemsSource = ((Models.SurvivorSelectionRandomizer)BindingContext).Survivors;
    }
}