using System.Collections.Generic;
using System.IO;


namespace FlightInspection
{
    class CSVHandler
    {
        private List<string> featuresList;
        private List<string> linesList;
        private string csvPath;
        private int numOfLines;
        private int numOfColumns;
        private float[,] flightTable;


        public CSVHandler(string csvPath)
        {
            this.csvPath = csvPath;
            this.linesList = this.csvParser();
            this.tableSize();
        }

        public float getFeatureValueByLineAndColumn(int line, int column)
        {
            return float.Parse(this.linesList[line].Split(',')[column]);
        }

        public void tableSize()
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
            return this.numOfLines;
        }

        public string getCSVLine(int line)
        {
            return linesList[line];
        }

        public void createNewCSV()
        {
            //string currentLine;
            //string featuresLine = String.Join(",", this.featuresList.Select(x => x.ToString()).ToArray());
            //string newCsv = "new_reg_flight.csv";
            //using (StreamWriter sw = new StreamWriter(newCsv, true))
            //{
            //    sw.WriteLine(featuresLine);
            //    using (StreamReader sr = new StreamReader(this.csvPath))
            //    {
            //        while ((currentLine = sr.ReadLine()) != null)
            //        {
            //            sw.WriteLine(currentLine);
            //        }
            //    }
            //}
        }


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
