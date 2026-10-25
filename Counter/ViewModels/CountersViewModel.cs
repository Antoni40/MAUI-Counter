using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Counter.ViewModels
{
    internal class CountersViewModel : INotifyPropertyChanged
    {
        private CounterViewModel? _selectedCounter;

        public CounterViewModel? SelectedCounter
        {
            get => _selectedCounter;
            set
            {
                _selectedCounter = value;
                OnPropertyChanged();
            }
        }
        //observable collection nie wysyła eventa kiedy zmienia się wartość licznika w obiekcie trzeba albo dodac obługe takich eventów albo odświerzać obiekty
        // chyba najlepiej dodać event
        public ObservableCollection<CounterViewModel> Counters { get; private set; }
        public ICommand CounterSelectionCommand { get;  }
        public ICommand AddNewCounterCommand { get; }

        public CountersViewModel()
        {
            Counters = new ObservableCollection<CounterViewModel>(CounterViewModel.LoadCounterViewModels());
            foreach (var counter in Counters)
            {
                // tutaj jest to event handler po uruchomieniu eventa w counterviewmodel
                counter.Deleted += OnCounterDeleted;
            }
            CounterSelectionCommand = new Command(OnCounterSelected);
            AddNewCounterCommand = new Command(AddNewCounter);
        }

        private void OnCounterDeleted(CounterViewModel counter)
        {
            counter.Deleted -= OnCounterDeleted;
            Counters.Remove(counter);
        }
        public async void AddNewCounter()
        {
            await Shell.Current.GoToAsync($"{nameof(Views.AddNewCounterPage)}");
        }
        //albo musi być jakiś force reloada, chociaż to oczywiście działa
        private void ReloadCountersCollection()
        {
            Counters = new ObservableCollection<CounterViewModel>(CounterViewModel.LoadCounterViewModels());
        }
        private async void OnCounterSelected()
        {
            if (SelectedCounter is null) return;

            int id = SelectedCounter.CounterID;

            await Shell.Current.GoToAsync($"{nameof(Views.CounterPage)}?{nameof(ViewModels.CounterViewModel.CounterID)}={id}");
            SelectedCounter = null;
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
