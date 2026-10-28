using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Counter.ViewModels
{
    internal class AddNewCounterViewModel : INotifyPropertyChanged
    {
        private string _counterName = String.Empty;
        public string CounterName
        {
            get => this._counterName;
            set
            {
                if (_counterName == value) return;
                this._counterName = value;
                OnPropertyChanged();
            }
        }
        private string _counterInitialValue = String.Empty;
        public string CounterInitialValue
        {
            get => this._counterInitialValue;
            set
            {
                if (_counterInitialValue == value) return;
                this._counterInitialValue = value;
                OnPropertyChanged();
            }
        }

        public record ColorRecord(string ColorName, string ColorHex);
        public List<ColorRecord> ColorsList { get; } = new()
        {
            new ColorRecord("Red", "#E53935"),
            new ColorRecord("Orange", "#FB8C00"),
            new ColorRecord("Yellow", "#FDD835"),
            new ColorRecord("Green", "#43A047"),
            new ColorRecord("Blue", "#1E88E5"),
            new ColorRecord("Purple", "#8E24AA"),
        };
        private ColorRecord _selectedColor = new ColorRecord("Czerwony", "#E53935");
        public ColorRecord SelectedColor {
            get => this._selectedColor;
            set
            {
                if (this._selectedColor == value) return;
                this._selectedColor = value;
                OnPropertyChanged();
            }
        }

        public string AddNewCounterText => "Add new counter";
        public string CounterNameText => "CounterName";
        public string InitialValueText => "Initial Value";
        public string AddNewCounterButtonText => "Add";
        public string NameEntryPlaceholder => "Counter";
        public string InitialValueEntryPlaceholder => "0";
        public string CounterColorText => "Counter Color";

        public ICommand AddNewCounterCommand { get; }

        public AddNewCounterViewModel()
        {
            this.AddNewCounterCommand = new Command(AddNewCounter);
        }
        public async void AddNewCounter()
        {
            if (!int.TryParse(this.CounterInitialValue, out int counterInitialValue)) return;

            string selectedColorHex = SelectedColor.ColorHex;
            new CounterViewModel(new Models.CounterModel(counterInitialValue, this.CounterName, selectedColorHex));

            ResetFormData();
            
            await Shell.Current.GoToAsync($"//{nameof(Views.CountersPage)}");
        }
        private void ResetFormData()
        {
            CounterName = String.Empty;
            CounterInitialValue = String.Empty;
            SelectedColor = ColorsList[0];
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
