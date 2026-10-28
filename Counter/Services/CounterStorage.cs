using Counter.Models;
using System.Xml;
using System.Xml.Linq;


namespace Counter.Services
{
    internal static class CounterStorage
    {
        private static string FilePath => Path.Combine(FileSystem.AppDataDirectory, "counter.xml");

        private static XDocument LoadDocument()
        {
            if(File.Exists(FilePath))
            {
                try
                {
                    return XDocument.Load(FilePath);
                } catch (XmlException e)
                {
                    Console.WriteLine(e);
                    XDocument newDoc = new(new XElement("Counters"));
                    newDoc.Save(FilePath);
                    return newDoc;
                }
            }
            XDocument doc = new(new XElement("Counters"));
            doc.Save(FilePath);
            return doc;
            
        }
        private static CounterModel ParseCounter(XElement elm)
        {
            if (!int.TryParse(elm?.Attribute("CounterID")?.Value, out int id))
                throw new Exception("Counter ID is missing not valid .xml file");

            string counterName = elm?.Element("CounterName")?.Value 
                ?? throw new Exception("Counter name is not set in .xml file");
            int counterNumber = (int.TryParse(elm?.Element("CounterNumber")?.Value, out int n)) ? n 
                : throw new Exception("Counter value is not set in .xml file");
            int initialValue = (int.TryParse(elm?.Element("InitialValue")?.Value, out int i)) ? i 
                : throw new Exception("Counter initial value is not set in .xml file");
            string colorHex = elm?.Element("CounterColor")?.Value 
                ?? throw new Exception("Counter color is not set in .xml file");

            return new CounterModel(id, counterNumber, counterName, initialValue, colorHex);
        }
        public static IEnumerable<CounterModel> LoadAll()
        {
            XDocument doc = LoadDocument();

            return doc.Root?.Elements("Counter")
                .Select(counter => ParseCounter(counter))
                .ToList<CounterModel>() ?? new List<CounterModel>();
        }
        public static CounterModel LoadCounter(int id)
        {
            XDocument doc = LoadDocument();
            XElement? counterElm = doc.Root?.Elements("Counter").
                FirstOrDefault(counter => int.TryParse(counter?.Attribute("CounterID")?.Value, out int cid) 
                ? cid == id : false);
            
            if (counterElm == null) 
                throw new Exception($"Counter with ID: {id} can't be found in .xml file");

            return ParseCounter(counterElm);
        }
        public static int AddCounter(int counterNumber, string counterName, int initialValue, string colorHex)
        {
            XDocument doc = LoadDocument();

            int newId = (doc.Root?.Elements("Counter")
                .Select(c => int.TryParse(c?.Attribute("CounterID")?.Value, out int id) ? id : -1)
                .DefaultIfEmpty(-1)
                .Max() ?? -1) + 1;

                doc.Root?.Add(new XElement("Counter",
                            new XAttribute("CounterID", newId),
                            new XElement("CounterNumber", counterNumber),
                            new XElement("CounterName", counterName),
                            new XElement("InitialValue", initialValue),
                            new XElement("CounterColor", colorHex)
                        )
                );
                doc.Save(FilePath);
            return newId;
        }
        public static void SaveValue(int id, int value)
        {
            XDocument document = LoadDocument();

            XElement? counterElm = document.Root?.Elements("Counter")
                .FirstOrDefault(c => int.TryParse(c?.Attribute("CounterID")?.Value, out int cId) 
                ? cId == id : false);
            
            if (counterElm == null)
                throw new Exception($"Counter with ID: {id} can't be found in .xml file");

            counterElm.Element("CounterNumber")?.SetValue(value);
            document.Save(FilePath);
        }
        public static void DeleteCounter(int ID)
        {
            XDocument document = LoadDocument();

            document.Root?.Elements("Counter")
                .FirstOrDefault(counter => int.TryParse(counter?.Attribute("CounterID")?.Value, out int counterID) 
                    ? counterID == ID : false)
                ?.Remove();
            
            document.Save(FilePath);
        }
    }
}
