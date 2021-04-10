using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FlightInspection
{
    class DataHandler
    {
        private List<string> featuresList;
        private List<string> linesList;

        public DataHandler()
        {

        }

        public float getFeatureValueByLineAndColumn(int line, int column)
        {
            return float.Parse(this.linesList[line].Split(',')[column]);
        }

        public void csvParser(string csv)
        {
            string currentLine;
            List<string> csvLines = new List<string>();
            using (StreamReader sr = new StreamReader(csv))
            {
                if (csv == @"C:\Users\User\Desktop\CS\שנה ב\סמסטר ב\Advanced Programming 2\תרגילים\תרגיל 1\anomaly_flight.csv") currentLine = sr.ReadLine();
                while ((currentLine = sr.ReadLine()) != null)
                {
                    csvLines.Add(currentLine);
                }
            }
            this.linesList = csvLines;
        }

        public static Dictionary<int, List<string>> getOutputTxt(string outputFile)
        {
            string currentLine;
            Dictionary<int, List<string>> lineAndFeature = new Dictionary<int, List<string>>();
            using (StreamReader sr = new StreamReader(outputFile))
            {
                while ((currentLine = sr.ReadLine()) != null)
                {
                    int lineNumber = int.Parse(currentLine.Split(',')[0]);
                    string feature = currentLine.Split(',')[1];

                    if (lineAndFeature.ContainsKey(lineNumber))
                    {
                        lineAndFeature[lineNumber].Add(feature);
                    }
                    lineAndFeature.Add(lineNumber, new List<string>());
                    lineAndFeature[lineNumber].Add(feature);
                }
            }
            return lineAndFeature;
        }

        public int getNumOfLines()
        {
            return this.linesList.Count;
        }

        public string getDetectionCsvLine(int line)
        {
            return linesList[line];
        }

        public static void createTxtFileFromTwoFiles(string learnCsv, string detectCsv, List<string> featuresList)
        {
            string currentLine;
            if (featuresList != null)
            {
                string featuresLine = String.Join(",", featuresList.Select(x => x.ToString()).ToArray());
                string newTxtFile = "input.txt";
                using (StreamWriter sw = new StreamWriter(newTxtFile, true))
                {
                    sw.WriteLine(featuresLine);
                    using (StreamReader sr = new StreamReader(learnCsv))
                    {
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(currentLine);
                        }
                        sw.WriteLine("done");
                    }
                    using (StreamReader sr = new StreamReader(detectCsv))
                    {
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(currentLine);
                        }
                        sw.WriteLine("done");
                    }
                }
            }
        }


        public static List<string> getFeaturesFromXml(string xmlPath)
        {
            XDocument xml = XDocument.Load(xmlPath);
            IEnumerable<string> temp = xml.Descendants("output").Descendants("name").Select(name => (string)name);
            return temp.ToList();
        }

        public void setFeaturesList(List<string> featuresList)
        {
            this.featuresList = featuresList;
        }

        public int getColumnByFeature(string feature)
        {

            if (this.featuresList != null)
            {
                for (int i = 0; i < this.featuresList.Count; i++)
                {
                    if (this.featuresList[i].Equals(feature))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public List<float> getValuesOfFeature(int column)
        {
            List<float> listOfValues = new List<float>();
            for (int i = 0; i < linesList.Count; i++)
            {
                listOfValues.Add(getFeatureValueByLineAndColumn(i, column));
            }
            return listOfValues;
        }
        public List<float> getValuesOfFeature(string feature)
        {
            List<float> listOfValues = new List<float>();
            int column = getColumnByFeature(feature);
            for (int i = 0; i < linesList.Count; i++)
            {
                listOfValues.Add(getFeatureValueByLineAndColumn(i, column));
            }
            return listOfValues;
        }
    }
}
