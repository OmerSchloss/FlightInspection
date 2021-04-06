using System;
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
        private int currentLineNumber;
        private bool threatStarted;
        private bool isConnect;

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
            CurrentLineNumber = 0;
            threatStarted = false;
            stop = false;
        }


        public void play(bool isConnected)
        {
            isConnect = isConnected;
            stop = false;
            if (!threatStarted)
            {
                new Thread(delegate ()
                {
                    threatStarted = true;
                    while (threatStarted)
                    {
                        if (isConnect)
                        {
                            this.telnetClient.write(this.csvHandler.getCSVLine(this.currentLineNumber) + "\r\n");
                        }
                        if ((!stop) && (this.currentLineNumber < this.csvHandler.getNumOfLines()-2))
                        {
                            CurrentLineNumber = CurrentLineNumber + 1;

                        }
                        System.Threading.Thread.Sleep(10);

                    }
                }
                      ).Start();
            }

        }

        internal void closeThread()
        {
            threatStarted = false;
        }

        internal void resetCurrent()
        {
            CurrentLineNumber = 0;
        }

        internal void backwardTenSec()
        {
            if (this.currentLineNumber < 100)
            {
                CurrentLineNumber = 0;
            }
            else
            {
                CurrentLineNumber = CurrentLineNumber - 100;
            }
        }

        internal int getNumberOfLines()
        {
            return this.csvHandler.getNumOfLines();
        }

        internal void endCurrentLine()
        {
            stop = true;
            CurrentLineNumber = this.csvHandler.getNumOfLines() - 2;
        }

        internal void forwardTenSec()
        {

            if(this.currentLineNumber < this.csvHandler.getNumOfLines()-101)
            {
                CurrentLineNumber = CurrentLineNumber + 100;
            }
            else
            {
                CurrentLineNumber = this.csvHandler.getNumOfLines() - 1;
            }
        }

        public bool connect(string ip, int port)
        {
            if (telnetClient.connect(ip, port))
            {
                return true;
            }
            return false;
        }

        public void pause()
        {
            stop = true;
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
