using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Counter.ViewModels
{
    internal class AddNewCounterViewModel : INotifyPropertyChanged
    {
        private string _counterName = string.Empty;
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
                if (_counterName == value) return;
                this._counterInitialValue = value;
                OnPropertyChanged();
            }
        }
        public string AddNewCounterText => "Add new counter";
        public string CounterNameText => "CounterName";
        public string InitialValueText => "Initial Value";
        public string AddNewCounterButtonText => "Add";
        public string NameEntryPlaceholder => "Counter";
        public string InitialValueEntryPlaceholder => "0";

        public ICommand AddNewCounterCommand { get; }

        public AddNewCounterViewModel()
        {
            this.AddNewCounterCommand = new Command(async () => await AddNewCounter());
        }
        public async Task AddNewCounter()
        {
            if (!int.TryParse(this.CounterInitialValue, out int counterInitialValue)) return;
            new CounterViewModel(new Models.CounterModel(counterInitialValue, this.CounterName));

            ResetFormData();
            
            await Shell.Current.GoToAsync($"//{nameof(Views.CountersPage)}");
        }
        private void ResetFormData()
        {
            CounterName = String.Empty;
            CounterInitialValue = String.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
