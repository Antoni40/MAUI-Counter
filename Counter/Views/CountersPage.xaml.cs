namespace Counter.Views;

public partial class CountersPage : ContentPage
{
	public CountersPage()
	{
		InitializeComponent();
		BindingContext = new ViewModels.CountersViewModel();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = new ViewModels.CountersViewModel();
    }
}