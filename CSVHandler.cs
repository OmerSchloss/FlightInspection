using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace FlightInspection
{
    class CSVHandler
    {
        private List<string> featuresList;
        private string csvPath;

        public CSVHandler(string csvPath)
        {
            this.featuresList = new List<string>();
            this.csvPath = csvPath;
        }


        public List<string> csvParser()
        {
            string currentLine;
            List<string> csvLines = new List<string>();
            using (StreamReader sr = new StreamReader(this.csvPath))
            {
                while ((currentLine = sr.ReadLine()) != null)
                {
                    csvLines.Add(currentLine);
                }
            }
            return csvLines;
        }

        public int getNumOfLines()
        {
            return csvParser().Count;
        }



        public void createNewCSV()
        {
            string currentLine;
            string featuresLine = String.Join(",", this.featuresList.Select(x => x.ToString()).ToArray());
            string newCsv = "new_reg_flight.csv";
            using (StreamWriter sw = new StreamWriter(newCsv, true))
            {
                sw.WriteLine(featuresLine);
                using (StreamReader sr = new StreamReader(this.csvPath))
                {
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(currentLine);
                    }
                }
            }
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

    }
}
