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
        private string fullcsvPath;
        private string xmlPath;


        public FlightgearModel(string csvPath, string xmlPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.csvPath = csvPath;
            this.csvHandler = new CSVHandler(csvPath);
            this.xmlPath = xmlPath;
            FeaturesList = getFeaturesFromXml();
            csvHandler.createNewCSV();
            fullcsvPath = "new_reg_flight.csv";
            this.currentLineNumber = 0;
            //this.featuresList = new List<string>();
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

        public void play(bool isConnected)
        {
            new Thread(delegate ()
            {
                while (!stop)
                {
                    if (this.currentLineNumber < this.csvHandler.getNumOfLines())
                    {
                        if (isConnected)
                        {
                            this.telnetClient.write(this.csvHandler.getCSVLine(this.currentLineNumber) + "\r\n");
                        }
                        this.currentLineNumber++;
                        System.Threading.Thread.Sleep(100);
                    }
                }

            }).Start();
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


        private int currentLineNumber;
        public int CurrentLineNumber
        {
            get { return currentLineNumber; }
            set
            {
                currentLineNumber = value;
                NotifyPropertyChanged(nameof(CurrentLineNumber));
            }
        }


        private List<string> featuresList;
        public List<string> FeaturesList
        {
            get { return featuresList; }
            set
            {
                featuresList = value;
                NotifyPropertyChanged(nameof(FeaturesList));

            }
        }



        private List<string> getFeaturesFromXml()
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.xmlPath);
            XmlNodeList featuresNames = xmlDoc.GetElementsByTagName("name");
            List<string> features = new List<string>();


            int i = 0;

            while (i < featuresNames.Count)
            {
                features.Add(featuresNames[i].InnerText);
                i++;
                if (featuresNames[i].InnerText.Equals("aileron")) break;

            }
            features.TrimExcess();
            return features;

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


    }
}
