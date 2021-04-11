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
                while ((currentLine = sr.ReadLine()) != null)
                {
                    csvLines.Add(currentLine);
                }
            }
            this.linesList = csvLines;
        }

        public static Dictionary<int, List<string>> getOutputTxt(string outputFile, List<string> featuresList)
        {
            string currentLine;
            Dictionary<int, List<string>> lineAndFeature = new Dictionary<int, List<string>>();

            using (StreamReader sr = new StreamReader(outputFile))
            {
                currentLine = sr.ReadLine();

                currentLine = sr.ReadLine();

                while ((currentLine = sr.ReadLine()) != "done")
                {
                    int lineNumber = int.Parse(currentLine.Split(',')[0]);
                    string feature = currentLine.Split(',')[1];

                    if (lineAndFeature.ContainsKey(lineNumber))
                    {
                        lineAndFeature[lineNumber].Add(featuresList[Convert.ToInt32(feature)-1]);
                    }

                    else
                    {
                        lineAndFeature.Add(lineNumber, new List<string>());
                        lineAndFeature[lineNumber].Add(featuresList[Convert.ToInt32(feature) - 1]);

                    }

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
                //string featuresLine = String.Join(",", featuresList.Select(x => x.ToString()).ToArray());

                string col = "";
                int i = 0;
                for (i = 1; i < featuresList.Count - 1; i++) {
                    col += i.ToString() + ",";
                }
                col += i.ToString();


                string newTxtFile = "input.txt";
                using (StreamWriter sw = new StreamWriter(newTxtFile, true))
                {
                    sw.WriteLine(col);
                    using (StreamReader sr = new StreamReader(learnCsv))
                    {
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(currentLine);
                        }
                        sw.WriteLine("done");
                    }
                    sw.WriteLine(col);
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
