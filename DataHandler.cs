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
        public static List<string> featuresList;
        private List<string> linesList;
        public static Dictionary<string, int> featureAndRadius;
        public static Dictionary<string, DataPoint> featureAndCenterPoint;
        public static Dictionary<string, List<DataPoint>> otherFeatureAndDataPoints;

        public static Dictionary<string, string> correlated;
        public static string detectionAlgorithm;

        public DataHandler()
        {

        }

        public float getFeatureValueByLineAndColumn(int line, int column)
        {
            if (line >= linesList.Count)
                line = linesList.Count - 1;
            if (column >= this.linesList[line].Split(',').Length)
                column = this.linesList[line].Split(',').Length - 1;
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

        public static void setCorrelatedMapFromDll()
        {
            correlated = new Dictionary<string, string>();
            if (featuresList != null)
            {
                foreach (string feature in featuresList)
                {
                    if (correlated.ContainsKey(feature)) continue;
                    correlated.Add(feature, null);
                }
            }
        }

        public static Dictionary<int, List<string>> getOutputTxt(string outputFile)
        {
            setCorrelatedMapFromDll();
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
                        string feature1 = currentLine.Split(',')[0];
                        string feature2 = currentLine.Split(',')[1];
                        correlated[feature1] = feature2;
                    }
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
                    while ((currentLine = sr.ReadLine()) != "done")
                    {
                        string feature1 = currentLine.Split(',')[0];
                        string feature2 = currentLine.Split(',')[1];
                        int X = int.Parse(currentLine.Split(',')[2]);
                        int Y = int.Parse(currentLine.Split(',')[3]);
                        int radius = int.Parse(currentLine.Split(',')[4]);
                        featureAndRadius.Add(feature1, radius);
                        featureAndCenterPoint.Add(feature1, new DataPoint(X, Y));
                        correlated[feature1] = feature2;

                    }
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
                if (detectionAlgorithm == "Other")
                {
                    otherFeatureAndDataPoints = new Dictionary<string, List<DataPoint>>();

                    while ((currentLine = sr.ReadLine()) != "done") //get correlated features;
                    {
                        string feature1 = currentLine.Split(',')[0];
                        string feature2 = currentLine.Split(',')[1];
                        correlated[feature1] = feature2;
                        otherFeatureAndDataPoints.Add(feature1, new List<DataPoint>());
                        while ((currentLine = sr.ReadLine()) != "done")
                            otherFeatureAndDataPoints[feature1].Add(new DataPoint(int.Parse(currentLine.Split(',')[0]), int.Parse(currentLine.Split(',')[1])));
                    }

                    currentLine = sr.ReadLine();//'start anomaly' line
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

                foreach (string feature in featuresList)  //if there is no correlation
                {
                    if (!(correlated.ContainsKey(feature))) correlated.Add(feature, null);
                    if (correlated[feature] == null) correlated[feature] = feature;
                }
                return lineAndFeature;
            }
        }

        public static List<string> getAnomalyListBox(string outputFile)
        {
            string currentLine;
            List<string> anomalyList = new List<string>();

            using (StreamReader sr = new StreamReader(outputFile))
            {
                currentLine = sr.ReadLine();
                if (currentLine == "Line")
                {
                    while ((currentLine = sr.ReadLine()) != "done") { }
                    while ((currentLine = sr.ReadLine()) != "done")
                    {
                        anomalyList.Add(currentLine);
                    }
                }
                if (currentLine == "Circle")
                {
                    while ((currentLine = sr.ReadLine()) != "done") { }
                    while ((currentLine = sr.ReadLine()) != "done")
                    {
                        anomalyList.Add(currentLine);
                    }
                }
                if (currentLine == "Other")
                {
                    while ((currentLine = sr.ReadLine()) != "start anomaly") { }
                    while ((currentLine = sr.ReadLine()) != "done")
                    {
                        anomalyList.Add(currentLine);
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

        public void setFeaturesList(List<string> fl)
        {
            featuresList = fl;
        }

        public int getColumnByFeature(string feature)
        {

            if (featuresList != null)
            {
                for (int i = 0; i < featuresList.Count; i++)
                {
                    if (featuresList[i].Equals(feature))
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
