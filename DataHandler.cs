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
        private string learnCsv;
        private string detectCsv;
        private string xml;


        public DataHandler(string learnCsv, string detectCsv, string xml)
        {
            this.learnCsv = learnCsv;
            this.detectCsv = detectCsv;
            this.xml = xml;
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
                if (csv == detectCsv) currentLine = sr.ReadLine();
                while ((currentLine = sr.ReadLine()) != null)
                {
                    csvLines.Add(currentLine);
                }
            }
            this.linesList = csvLines;
        }

        public Dictionary<int, List<string>> getOutputTxt(string outputFile)
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

        public void createNewTxtFileFromTwoFiles(string learnCsv, string detectCsv)
        {
            string currentLine;
            if (featuresList != null)
            {
                string featuresLine = String.Join(",", this.featuresList.Select(x => x.ToString()).ToArray());
                string newTxtFile = "intput.txt";
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

        public List<string> getFeaturesFromXml()
        {
            XDocument xml = XDocument.Load(this.xml);
            IEnumerable<string> temp = xml.Descendants("output").Descendants("name").Select(name => (string)name);
            featuresList = temp.ToList();
            return temp.ToList();
        }

        public int getColumnByFeature(string feature)
        {
            int i = 0;
            for (; i < this.featuresList.Count; i++)
            {
                if (this.featuresList[i].Equals(feature))
                {
                    return i;
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



        /* public void tableSize()
   {
       int lineCounter = 0;
       string currentLine;

       using (StreamReader sr = new StreamReader(this.csvPath))
       {
           while ((currentLine = sr.ReadLine()) != null)
           {
               lineCounter++;
           }
       }
       this.numOfLines = lineCounter;
       this.numOfColumns = 1100;
   }*/

        /*
private void create2DArrayFromCsv()
{
    int timeLine = 0;
    FileStream fs = new FileStream(this.csvPath, FileMode.Open, FileAccess.Read); //    open for reading
                                                                                  // the file has opened; can read it now
    StreamReader sr = new StreamReader(fs);
    while (!sr.EndOfStream)
    {
        string line = sr.ReadLine();
        string[] parts = line.Split(',');
        for (int i = 0; i < parts.Length; i++)
        {
            this.flightTable[timeLine,i] = float.Parse(parts[i]);
        }
        timeLine++;
    }
}
*/


    }
}
