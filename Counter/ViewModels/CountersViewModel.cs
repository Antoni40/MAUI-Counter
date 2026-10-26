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
        public ObservableCollection<CounterViewModel> Counters { get; private set; }
        public string AddCounterText => "Add Counter";
        public string CounterNameLabel => "Counter Name";
        public string CountLabel => "Counter Value";
        public string DangerZoneText => "Danger zone";
        public string NoCountersText => "There are no counters added";
        public ICommand CounterSelectionCommand { get;  }
        public ICommand AddNewCounterCommand { get; }

        public CountersViewModel()
        {
            Counters = new ObservableCollection<CounterViewModel>(CounterViewModel.LoadCounterViewModels());
            foreach (var counter in Counters)
            {
                // signing event handler after deleting
                counter.Deleted += OnCounterDeleted;
            }

            CounterSelectionCommand = new Command(OnCounterSelected);
            AddNewCounterCommand = new Command(AddNewCounter);
        }

        public async void AddNewCounter()
        {
            await Shell.Current.GoToAsync($"{nameof(Views.AddNewCounterPage)}");
        }
        // I have a different solve for that, when counter is deleted i rise event and handle it in OnCounterDeleted in this class
        //private void ReloadCountersCollection()
        //{
        //    Counters = new ObservableCollection<CounterViewModel>(CounterViewModel.LoadCounterViewModels());
        //}
        private async void OnCounterSelected()
        {
            if (SelectedCounter is null) return;

            int id = SelectedCounter.CounterID;

            await Shell.Current.GoToAsync($"{nameof(Views.CounterPage)}?{nameof(CounterViewModel.CounterID)}={id}");
            SelectedCounter = null;
            
        }
        private void OnCounterDeleted(CounterViewModel counter)
        {
            counter.Deleted -= OnCounterDeleted;
            Counters.Remove(counter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
