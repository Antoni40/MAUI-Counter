using Counter.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Counter.Models
{
    // solve problems with null warnings
    internal class CounterModel
    {
        public int ID { get; private set; }
        private int _counterNumber;
        public int CounterNumber
        {
            get => _counterNumber;
            set
            {
                SaveCounterNumber(value);
                this._counterNumber = value;
            }
        }
        public string CounterName { get; private set; }
        public int InitialValue { get; private set; }

        public CounterModel() : this(0, "Default Name") { }

        public CounterModel(int number, string name)
        {
            this.CounterNumber = number >= 0 ? number : 0;
            this.CounterName = name.Length > 0 ? name: "Default Counter";
            this.InitialValue = this.CounterNumber;

            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");

            Console.WriteLine(filePath);
            if (!File.Exists(filePath))
            {
                XDocument document = new(
                    new XElement("Counters",
                        new XElement("Counter",
                            new XAttribute("CounterID", 0),
                            new XElement("CounterNumber", this.CounterNumber),
                            new XElement("CounterName", this.CounterName),
                            new XElement("InitialValue", this.InitialValue)
                        )
                    )
                );
                document.Save(filePath);
            }
            else
            {
                XDocument document = XDocument.Load(filePath);
                XElement? lastCounter = document.Root?.Elements("Counters").LastOrDefault();

                document.Root?.Add(new XElement("Counter",
                    new XAttribute("CounterID", lastCounter == null ? 0 : int.Parse(lastCounter.Value) + 1),
                    new XElement("CounterNumber", this.CounterNumber),
                    new XElement("CounterName", this.CounterName),
                    new XElement("InitialValue", this.InitialValue)
                ));
                document.Save(filePath);
            }
        }

        public CounterModel (int ID)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");
            XDocument document = XDocument.Load(filePath);
            XElement? counter = document.Root?.Elements().FirstOrDefault(c => int.TryParse(c?.Attribute("CounterID")?.Value, out int counterID) ? counterID == ID : false);

            //if there is null we have a really big problem cause we shouldn't create a model I'll throw an error that is not handled
            if (counter != null)
            {
                this.ID = ID;
                this.CounterName = counter?.Element("CounterName")?.Value ?? "Default Name";
                this.CounterNumber = (int.TryParse(counter?.Element("CounterNumber")?.Value, out int counterNumber)) ? counterNumber : 0;
                this.InitialValue = (int.TryParse(counter?.Element("InitialValue")?.Value, out int initialValue)) ? counterNumber : 0;
            } else
            {
                throw new Exception($"Counter with ID: {ID} cannot be found in .xml file");
            }
        }

        public void Increment()
        {
            SaveCounterNumber(++this.CounterNumber);
            
        }
        public void Decrement()
        {
            SaveCounterNumber(--this.CounterNumber);
        }

        public void Reset()
        {
            this.CounterNumber = this.InitialValue;
            SaveCounterNumber(this.CounterNumber);
        }

        public void Delete()
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");
            XDocument document = XDocument.Load(filePath);

            //there is no problem when deleting cause it won't be deleted
            document.Root?.Elements()
                .Where(counter => int.TryParse(counter?.Attribute("CounterID")?.Value, out int counterID) ? counterID == this.ID : false)
                .Remove();

            document.Save(filePath);
        }

        // dodatkowe zabezpieczenia na przyszłość, żeby nie było problemów gdy obiekt istnieje w pamięci, ale nie w pliku xml. 
        // To jest mało prawdopodobne, ale gdy będzie czas to można dodać. Wtedy zaimplementować to jako osobną metodę (to sprawdzanie).
        // ta metoda będzie działac tylko wtedy gdy z pliku xml zostaną usunięte niepowołanie dane
        private void SaveCounterNumber(int number)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");
            XDocument document = XDocument.Load(filePath);

            XElement? elm = document.Root?.Elements()
                .Where(counter => int.TryParse(counter?.Attribute("CounterID")?.Value, out int counterID) ? counterID == this.ID : false)
                .FirstOrDefault();

            elm?.Element("CounterNumber")?.SetValue(number);
            document.Save(filePath);
        }

        public static CounterModel? LoadCounter(int ID)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");
            if (!File.Exists(filePath))
                return null;

            XDocument document = XDocument.Load(filePath);

            if (document.Root == null || !document.Root.HasElements)
                return null;

            CounterModel? counter = document.Root.Elements("Counter")
                .Where((counter) => counter.HasAttributes && int.TryParse(counter?.Attribute("CounterID")?.Value, out int counterID) ? counterID == ID : false)
                .Select(counter => new CounterModel(int.TryParse(counter?.Attribute("CounterID")?.Value, out int counterID) 
                    ? (counterID == ID ? counterID : throw new Exception($"Counter with ID: {ID} cannot be found in .xml file")) : 
                    throw new Exception("Counter ID strored in xml file is not a number")))
                .FirstOrDefault();

            return counter;
        }

        public static IEnumerable<CounterModel> LoadCounters()
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");

            if (!File.Exists(filePath))
            {
                createEmptyXmlFile();
                return Enumerable.Empty<CounterModel>();
            }
            
            XDocument countersXmlDocument = XDocument.Load(filePath);
            
            List<CounterModel>? counters = countersXmlDocument.Root?.Elements("Counter")
                .Select(counter => new CounterModel(int.TryParse(counter?.Attribute("CounterID")?.Value, out int counterID) ? 
                    counterID : throw new Exception("Counter ID strored in xml file is not a number")))
                .ToList<CounterModel>();

            counters = counters ?? new List<CounterModel>();

            return counters;   
        }

        private static void createEmptyXmlFile()
        {
            string filePath = "counter.xml";
            XDocument document = new(
                new XElement("Counters"));

            document.Save(filePath);
        }
    }
}
