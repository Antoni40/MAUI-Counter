using Counter.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Counter.ViewModels
{
    internal class CounterViewModel : INotifyPropertyChanged
    {
        public int CounterID { get; private set; }
        public CounterModel Model { get; private set; }
        public string CounterName => Model.CounterName;
        private int _counter;
        public int Counter
        {
            get => _counter;
            set
            {
                if (_counter == value) return;
                _counter = value;
                OnPropertyChanged(nameof(Counter));
            }
        }
        public string DeleteText => "Delete";

        public ICommand IncrementCounterCommand { get; }
        public ICommand DecrementCounterCommand { get; }
        public ICommand ResetCounterCommand { get; }
        public ICommand DeleteCounterCommand { get; }

        public CounterViewModel() : this(new CounterModel()) {}
        public CounterViewModel(int counterID) : this(CounterModel.LoadCounter(counterID) ?? throw new Exception("This counter can't be found")) {}
        public CounterViewModel(CounterModel counterModel)
        {
            this.Model = counterModel;
            CounterID = this.Model.ID;
            Counter = Model.CounterNumber;
            IncrementCounterCommand = new Command(IncrementCounter);
            DecrementCounterCommand = new Command(DecrementCounter);
            ResetCounterCommand = new Command(ResetCounter);
            DeleteCounterCommand = new Command(async () => await DeleteCounter());
        }
        private void IncrementCounter()
        {
            this.Model.Increment();
            RefreshCounter();
        }
        private void DecrementCounter()
        {
            this.Model.Decrement();
            RefreshCounter();
        }
        private void ResetCounter()
        {
            this.Model.Reset();
            RefreshCounter();
        }
        private void RefreshCounter() => this.Counter = this.Model.CounterNumber;

        public static IEnumerable<CounterViewModel> LoadCounterViewModels()
        {
            List<CounterViewModel> counters = CounterModel.LoadCounters()
                .Select(counterModel => new CounterViewModel(counterModel))
                .ToList<CounterViewModel>();

            return counters;
        }
        // find out if Action is the best way to handle event
        public event Action<CounterViewModel>? Deleted;
        private async Task DeleteCounter()
        {
            this.Model.Delete();
            Deleted?.Invoke(this);
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
