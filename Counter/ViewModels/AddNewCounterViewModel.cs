using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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
                this._counterName = value;
                OnPropertyChanged();
            }
        }
        private string _counterInitialValue = "";
        public string CounterInitialValue
        {
            get => this._counterInitialValue;
            set
            {
                this._counterInitialValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddNewCounterCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public AddNewCounterViewModel()
        {
            this.AddNewCounterCommand = new Command(async () => await AddNewCounter());
        }
        private void resetData()
        {
            CounterName = "";
            CounterInitialValue = "";
        }
        public async Task AddNewCounter()
        {
            if (!int.TryParse(this.CounterInitialValue, out int counterInitialValue)) return;
            new ViewModels.CounterViewModel(new Models.CounterModel(counterInitialValue, this.CounterName));
            resetData();
            
            await Shell.Current.GoToAsync($"///{nameof(Views.CountersPage)}");
        }
        private void OnPropertyChanged([CallerMemberName] string? property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
