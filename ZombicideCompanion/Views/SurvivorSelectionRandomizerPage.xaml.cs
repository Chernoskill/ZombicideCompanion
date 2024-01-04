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

    private void Reset_Clicked(object sender, EventArgs e)
    {
        ((Models.SurvivorSelectionRandomizer)BindingContext).LoadSurvivors();
        survivorCollection.ItemsSource = ((Models.SurvivorSelectionRandomizer)BindingContext).Survivors;
    }
}