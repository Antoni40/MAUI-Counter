namespace Counter.Views;

public partial class AddNewCounterPage : ContentPage
{
	public AddNewCounterPage()
	{
		InitializeComponent();
		BindingContext = new ViewModels.AddNewCounterViewModel();
	}
}