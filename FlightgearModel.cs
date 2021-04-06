using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml;

namespace FlightInspection
{
    class FlightgearModel : INotifyPropertyChanged
    {

        //public event PropertyChangedEventHandler PropertyChanged;
        TelnetClient telnetClient;
        volatile bool stop;
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
            this.featuresList = new List<string>();
            this.csvHandler = new CSVHandler(csvPath);
            this.xmlPath = xmlPath;
            this.setFeaturesFromXml();
            csvHandler.createNewCSV();
            fullcsvPath = "new_reg_flight.csv";
            this.currentLineNumber = 1490;

            stop = false;
        }

        public bool connect(string ip, int port)
        {
            if (telnetClient.connect(ip, port))
            {
                return true;
            }
            return false;
        }

        public void play()
        {
            new Thread(delegate ()
            {
                while (!stop)
                {
                    if (this.currentLineNumber < this.csvHandler.getNumOfLines())
                    {
                        this.telnetClient.write(this.csvHandler.getCSVLine(this.currentLineNumber) + "\r\n");
                        this.currentLineNumber++;
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
                  ).Start();
        }

        public void disconnect()
        {
            stop = true;
            telnetClient.disconnect();
        }



        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public float elevator
        {
            get
            {
                return this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("elevator"));
            }
            set { }
        }

        public float aileron
        {
            get
            {
                return this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("aileron"));
            }
            set { }
        }

        public float rudder
        {
            get
            {
                return this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("rudder"));
            }
            set { }
        }

        public float throttle
        {
            get
            {
                return this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("throttle"));
            }
            set { }
        }

        public int CurrentLineNumber
        {
            get { return currentLineNumber; }
            set
            {
                currentLineNumber = value;
                NotifyPropertyChanged(nameof(CurrentLineNumber));
            }
        }







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

        private void setFeaturesFromXml()
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.xmlPath);
            XmlNodeList featuresNames = xmlDoc.GetElementsByTagName("name");


            int i = 0;

            while (i < featuresNames.Count)
            {
                this.featuresList.Add(featuresNames[i].InnerText);
                i++;
                if (featuresNames[i].InnerText.Equals("aileron")) break;

            }
            this.featuresList.TrimExcess();
        }

        private int getColumnByFeature(string feature)
        {
            int i = 0;
            for (; i < this.featuresList.Count; i++)
            {
                if (this.featuresList[i].Equals(feature))
                {
                    //Console.WriteLine(feature);

                    //Console.WriteLine(i);
                    return i;
                }
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
