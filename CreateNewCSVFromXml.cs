using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace FlightInspection
{
    class CreateNewCSVFromXml
    {
        private List<string> featuresList;
        private Dictionary<string, List<float>> mapFromCSV;

        public CreateNewCSVFromXml()
        {
            this.featuresList = new List<string>();
            this.mapFromCSV = new Dictionary<string, List<float>>();
        }

        public void setFeaturesFromXml(string xmlPath)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList featuresNames = xmlDoc.GetElementsByTagName("name");


            int i = 0;

            while (i < featuresNames.Count)
            {
                this.featuresList.Add(featuresNames[i].InnerText);
                i++;
                if (featuresNames[i].InnerText.Equals("aileron")) break;

            }
            featuresList.TrimExcess();
        }
        public void createNewCSV(string currentCsv)
        {
            string currentLine;
            string featuresLine = String.Join(",", this.featuresList.Select(x => x.ToString()).ToArray());
            string newCsv = "new_reg_flight.csv";
            using (StreamWriter sw = new StreamWriter(newCsv, true))
            {
                sw.WriteLine(featuresLine);
                using (StreamReader sr = new StreamReader(currentCsv))
                {
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(currentLine);
                    }
                }
            }
        }

    }
}
