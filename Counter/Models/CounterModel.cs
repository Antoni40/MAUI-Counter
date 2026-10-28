namespace Counter.Models
{
    internal class CounterModel
    {
        public int ID { get; private set; }
        public int CounterNumber { get; set; }
        public string CounterName { get; private set; }
        public int InitialValue { get; private set; }
        public string ColorHex { get; set; }
        public CounterModel() : this(0, "Counter") { }

        public CounterModel(int number, string name, string colorHex = "#FFD100")
        {
            this.CounterNumber = number;
            this.CounterName = name.Length > 0 ? name: "Counter";
            this.InitialValue = this.CounterNumber;
            this.ColorHex = colorHex;

            this.ID = Services.CounterStorage.
                AddCounter(this.CounterNumber, this.CounterName, this.InitialValue, this.ColorHex);
        }

        public CounterModel(int id, int counterNumber, string counterName, int initialValue, string colorHex)
        {
            this.ID = id;
            this.CounterNumber = counterNumber;
            this.CounterName = counterName;
            this.InitialValue = initialValue;
            this.ColorHex = colorHex;
        }

        public void Increment() => SaveCounterNumber(++this.CounterNumber);
        public void Decrement() => SaveCounterNumber(--this.CounterNumber);

        public void Reset()
        {
            this.CounterNumber = this.InitialValue;
            SaveCounterNumber(this.CounterNumber);
        }

        public void Delete() => Services.CounterStorage.DeleteCounter(ID);

        private void SaveCounterNumber(int number) => Services.CounterStorage.SaveValue(this.ID, number);

        public static CounterModel LoadCounter(int ID) => Services.CounterStorage.LoadCounter(ID);

        public static IEnumerable<CounterModel> LoadCounters() => 
            Services.CounterStorage.LoadAll().OrderByDescending(c => c.ID);
    }
}
