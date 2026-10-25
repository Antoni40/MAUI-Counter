using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Counter.Models
{
    // ten model na każdą zmiane musi aktualizować plik .xml (bo miał być automatyczny zapis)
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
                document.Root?.Add(new XElement("Counter",
                    new XAttribute("CounterID",
                        document.Root.Elements().LastOrDefault().Attribute("CounterID") == null ? 0 :
                        int.Parse(document.Root.Elements().LastOrDefault().Attribute("CounterID").Value) + 1),
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
            XElement counter = document.Root?.Elements().FirstOrDefault(counter => int.Parse(counter.Attribute("CounterID").Value) == ID);

            this.ID = ID;
            this.CounterName = counter.Element("CounterName").Value;
            this.CounterNumber = int.Parse(counter.Element("CounterNumber").Value);
            this.InitialValue = int.Parse(counter.Element("InitialValue").Value);
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

            document.Root?.Elements()
                .Where(counter => int.Parse(counter.Attribute("CounterID").Value) == this.ID)
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
                .Where(counter => int.Parse(counter.Attribute("CounterID").Value) == this.ID)
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
                .Where((counter) => counter.HasAttributes && (int.Parse(counter.Attribute("CounterID").Value) == ID))
                .Select(counter => new CounterModel(int.Parse(counter.Attribute("CounterID").Value)))
                .FirstOrDefault();

            return counter;
        }

        public static IEnumerable<CounterModel> LoadCounters()
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "counter.xml");

            // tutaj trzeba stworzyć ten plik jak go nie ma
            // to bardzo ważne
            if (!File.Exists(filePath))
            {
                createEmptyXmlFile();
                return Enumerable.Empty<CounterModel>();
            }
            
            XDocument countersXmlDocument = XDocument.Load(filePath);

            //if (countersXmlDocument.Root == null || !countersXmlDocument.Root.HasElements)
            //    return Enumerable.Empty<CounterModel>();
            
            
            List<CounterModel> counters = countersXmlDocument.Root.Elements("Counter")
                .Select(counter => new CounterModel(int.Parse(counter.Attribute("CounterID").Value)))
                .ToList<CounterModel>();

            if (counters.Count == 0)
            {
                counters.Add(new CounterModel());
            }

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
