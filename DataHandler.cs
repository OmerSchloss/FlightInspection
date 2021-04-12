using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using OxyPlot;

namespace FlightInspection
{
    class DataHandler
    {
        private List<string> featuresList;
        private List<string> linesList;
        public static Dictionary<string, int> featureAndRadius;
        public static Dictionary<string, DataPoint> featureAndCenterPoint;
        public static string detectionAlgorithm;

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
                while ((currentLine = sr.ReadLine()) != null)
                {
                    csvLines.Add(currentLine);
                }
            }
            this.linesList = csvLines;
        }

        public static void setDetectionAlgorithm(string algo)
        {
            detectionAlgorithm = algo;
        }
        public static Dictionary<int, List<string>> getOutputTxt(string outputFile)
        {
            int counter = 0;
            string currentLine;
            featureAndCenterPoint = new Dictionary<string, DataPoint>();
            featureAndRadius = new Dictionary<string, int>();
            Dictionary<int, List<string>> lineAndFeature = new Dictionary<int, List<string>>();

            using (StreamReader sr = new StreamReader(outputFile))
            {
                currentLine = sr.ReadLine();
                setDetectionAlgorithm(currentLine);
                if (detectionAlgorithm == "Line")
                {
                    while ((currentLine = sr.ReadLine()) != "done")
                    {
                        int lineNumber = int.Parse(currentLine.Split(',')[0]);
                        string feature = currentLine.Split(',')[1];

                        if (lineAndFeature.ContainsKey(lineNumber))
                        {
                            lineAndFeature[lineNumber].Add(feature);
                        }
                        else
                        {
                            lineAndFeature.Add(lineNumber, new List<string>());
                            lineAndFeature[lineNumber].Add(feature);
                        }
                    }
                }
                if (detectionAlgorithm == "Circle")
                {
                    while ((currentLine = sr.ReadLine()) != "done" && counter != 2)
                    {
                        if (counter == 0)
                        {
                            string feature = currentLine.Split(',')[0];
                            int X = int.Parse(currentLine.Split(',')[1]);
                            int Y = int.Parse(currentLine.Split(',')[2]);
                            int radius = int.Parse(currentLine.Split(',')[3]);
                            featureAndRadius.Add(feature, radius);
                            featureAndCenterPoint.Add(feature, new DataPoint(X, Y));
                        }
                        if (counter == 1)
                        {
                            int lineNumber = int.Parse(currentLine.Split(',')[0]);
                            string feature = currentLine.Split(',')[1];

                            if (lineAndFeature.ContainsKey(lineNumber))
                            {
                                lineAndFeature[lineNumber].Add(feature);
                            }
                            else
                            {
                                lineAndFeature.Add(lineNumber, new List<string>());
                                lineAndFeature[lineNumber].Add(feature);
                            }
                        }
                        if (currentLine == "done") counter++;
                    }
                }
            }
            return lineAndFeature;
        }
        public static List<string> getAnomalyListBox(string outputFile)
        {
            int counter = 0;
            string currentLine;
            List<string> anomalyList = new List<string>();

            using (StreamReader sr = new StreamReader(outputFile))
            {
                currentLine = sr.ReadLine();
                if (currentLine == "Line")
                {
                    while ((currentLine = sr.ReadLine()) != "done")
                    {
                        anomalyList.Add(currentLine);
                    }
                }
                if (currentLine == "Circle")
                {
                    while ((currentLine = sr.ReadLine()) != "done" && counter != 2)
                    {
                        if (counter == 1)
                        {
                            anomalyList.Add(currentLine);
                        }
                        if (currentLine == "done") counter++;
                    }
                }
            }
            return anomalyList;
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
                    sw.WriteLine(featuresLine);
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
