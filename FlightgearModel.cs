using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml;

namespace FlightInspection
{
    class FlightgearModel : INotifyPropertyChanged
    {
        private float _elevator;
        private float _rudder;
        private float _aileron;
        private float _throttle;
        private float _altimeter;
        private float _airspeed;
        private float _direction;
        private float _roll;
        private float _pitch;
        private float _yaw;
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
                        this.updateProperties();
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

        private void updateProperties(){
            elevator = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("elevator"));
            aileron = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("aileron"));
            rudder = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("rudder"));
            throttle = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("throttle"));
            altimeter = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("altimeter_indicated-altitude-ft"));
            airspeed = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("airspeed-kt"));
            direction = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("heading-deg"));
            roll = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("roll-deg"));
            pitch = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("pitch-deg"));
            yaw = this.csvHandler.getFeatureValueByLineAndColumn(this.currentLineNumber, getColumnByFeature("side-slip-deg"));
                        
        }

        public float elevator
        {
            get { return _elevator; }
            set {
                _elevator = value; 
                NotifyPropertyChanged("elevator");
            }
        }

        public float aileron
        {
            get { return _aileron; }
            set { 
                _aileron = value; 
                NotifyPropertyChanged("aileron");
            }
        }

        public float rudder
        {
            get { return _rudder; }
            set { 
                _rudder = value; 
                NotifyPropertyChanged("rudder");
            }
        }

        public float throttle
        {
            get { return _throttle; }
            set { 
                _throttle = value; 
                NotifyPropertyChanged("throttle");
            }
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

        public float altimeter
        {
            get { return _altimeter; }
            set
            {
                _altimeter = value;
                NotifyPropertyChanged("altimeter");
            }
        }

        public float airspeed
        {
            get { return _airspeed; }
            set
            {
                _airspeed = value;
                NotifyPropertyChanged("airspeed");
            }
        }

        
        public float direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                NotifyPropertyChanged("direction");
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

        public float roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                NotifyPropertyChanged("roll");
            }
        }

        public float pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                NotifyPropertyChanged("pitch");
            }
        }

        public float yaw
        {
            get { return _yaw; }
            set
            {
                _yaw = value;
                NotifyPropertyChanged("yaw");
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
