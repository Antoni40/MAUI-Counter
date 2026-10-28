namespace Counter.Views;
[QueryProperty(nameof(CounterID), nameof(CounterID))]

public partial class CounterPage : ContentPage
{
	public int CounterID { set =>  LoadCounter(value); }
	public CounterPage()
	{
		InitializeComponent();
	}
	private void LoadCounter(int id)
	{
        BindingContext = new ViewModels.CounterViewModel(id);
    }
}