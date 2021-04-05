using System.Collections.Generic;
using System.Xml;


namespace FlightInspection
{
    class FlightgearModel : NotifyPropertyChanged
    {

        //public event PropertyChangedEventHandler PropertyChanged;
        TelnetClient telnetClient;
        //volatile Boolean stop;
        private CSVHandler csvHandler;
        private string csvPath;
        private List<string> featuresList;
        private string fullcsvPath;
        private string xmlPath;
        int currentLineNumber;


        public FlightgearModel(string csvPath, string xmlPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.csvPath = csvPath;
            this.csvHandler = new CSVHandler(csvPath);
            this.xmlPath = xmlPath;
            this.setFeaturesFromXml();
            csvHandler.createNewCSV();
            fullcsvPath = "new_reg_flight.csv";
            this.currentLineNumber = 0;
            
            //stop = false;
        }
             
        public float elevator
        {
            get
            {
                return this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("elevator"));
            }
            set { }
        }

        private void setFeaturesFromXml()
        {

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(this.xmlPath);
            //XmlNodeList featuresNames = xmlDoc.GetElementsByTagName("name");


            //int i = 0;

            //while (i < featuresNames.Count)
            //{
            //    this.featuresList.Add(featuresNames[i].InnerText);
            //    i++;
            //    if (featuresNames[i].InnerText.Equals("aileron")) break;

            //}
            //this.featuresList.TrimExcess();
        }

        private int getColumnByFeature(string feature)
        {
            int i = 0;
            for (; i < this.featuresList.Count; i++)
            {
                if (this.featuresList[i].Equals(feature))
                    return i;
            }
            return -1;
        }
        //public void connect(string ip, int port)
        //{
        //    telnetClient.connect(ip, port);
        //}

        //public void disconnect()
        //{
        //    stop = true;
        //    telnetClient.disconnect();
        //}


        //public void start()
        //{
        //    newThread(delegate () {
        //        while (!stop)
        //        {
        //            telnetClient.write("get left sonar");
        //            LeftSonar = Double.Parse(telnetClient.read());
        //            // the same for the other sensors propertiesThread.Sleep(250);
        //            // read the data in 4Hz
        //        }
        //    }).Start();
        //}
        
    }
}
