using Counter.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Counter.ViewModels
{
    internal class CounterViewModel : INotifyPropertyChanged
    {
        public CounterModel Model { get; private set; }
        public int CounterID => Model.ID;
        public string CounterName => Model.CounterName;
        public int Counter
        {
            get => this.Model.CounterNumber;
            set
            {
                if (this.Model.CounterNumber == value) return;
                this.Model.CounterNumber = value;
                OnPropertyChanged(nameof(Counter));
            }
        }

        public Color CounterColor => Color.FromArgb(this.Model.ColorHex);
        public string DeleteText => "Delete";


        public string SelectedColor { get; set; } = String.Empty;

        public ICommand IncrementCounterCommand { get; }
        public ICommand DecrementCounterCommand { get; }
        public ICommand ResetCounterCommand { get; }
        public ICommand DeleteCounterCommand { get; }

        public CounterViewModel() : this(new CounterModel()) {}
        public CounterViewModel(int counterID) : this(CounterModel.LoadCounter(counterID) ?? throw new Exception("This counter can't be found")) {}
        public CounterViewModel(CounterModel counterModel)
        {
            this.Model = counterModel;
            this.Counter = Model.CounterNumber;
            IncrementCounterCommand = new Command(IncrementCounter);
            DecrementCounterCommand = new Command(DecrementCounter);
            ResetCounterCommand = new Command(ResetCounter);
            DeleteCounterCommand = new Command(async () => await DeleteCounter());
        }
        private void IncrementCounter()
        {
            this.Model.Increment();
            OnPropertyChanged(nameof(Counter));
        }
        private void DecrementCounter()
        {
            this.Model.Decrement();
            OnPropertyChanged(nameof(Counter));
        }
        private void ResetCounter()
        {
            this.Model.Reset();
            OnPropertyChanged(nameof(Counter));
        }

        public static IEnumerable<CounterViewModel> LoadCounterViewModels()
        {
            List<CounterViewModel> counters = CounterModel.LoadCounters()
                .Select(counterModel => new CounterViewModel(counterModel))
                .ToList<CounterViewModel>();

            return counters;
        }
        public event EventHandler? Deleted;
        private async Task DeleteCounter()
        {
            this.Model.Delete();
            Deleted?.Invoke(this, EventArgs.Empty);
            await Shell.Current.GoToAsync($"///{nameof(Views.CountersPage)}");
            return;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}
