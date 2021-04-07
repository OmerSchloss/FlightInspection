﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using OxyPlot;

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
        private int currentLineNumber;
        private List<DataPoint> points;
        private List<DataPoint> correlativePoints;



        TelnetClient telnetClient;
        volatile bool stop;
        private CSVHandler csvHandler;
        private List<string> featuresList;
        private string csvPath;
        private string fullcsvPath;
        private string xmlPath;
        private string featureToDisplay;
        private string correlativeFeatureToDisplay;
        private bool threatStarted;
        private bool isConnect;


        public FlightgearModel(string csvPath, string xmlPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.csvPath = csvPath;
            this.csvHandler = new CSVHandler(csvPath);
            this.xmlPath = xmlPath;
            FeaturesList = getFeaturesFromXml();
            csvHandler.createNewCSV();
            fullcsvPath = "new_reg_flight.csv";
            CurrentLineNumber = 0;
            threatStarted = false;
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
                            this.telnetClient.write(this.csvHandler.getCSVLine(CurrentLineNumber) + "\r\n");
                        }
                        if ((!stop) && (CurrentLineNumber < this.csvHandler.getNumOfLines() - 2))
                        {
                            CorrelativePoints = getCorrelativePointsFromStart(CurrentLineNumber, featureToDisplay);
                            this.updateProperties();
                            CurrentLineNumber = CurrentLineNumber + 1;
                        }
                        System.Threading.Thread.Sleep(100);

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
        public void pause()
        {
            stop = true;
        }
        internal void backwardTenSec()
        {
            if (CurrentLineNumber < 100)
            {
                CurrentLineNumber = 0;
            }
            else
            {
                CurrentLineNumber = CurrentLineNumber - 100;
            }
        }

        internal void endCurrentLine()
        {
            stop = true;
            CurrentLineNumber = this.csvHandler.getNumOfLines() - 2;
        }
        internal void forwardTenSec()
        {

            if (CurrentLineNumber < this.csvHandler.getNumOfLines() - 101)
            {
                CurrentLineNumber = CurrentLineNumber + 100;
            }
            else
            {
                CurrentLineNumber = this.csvHandler.getNumOfLines() - 1;
            }
        }

        public void disconnect()
        {
            stop = true;
            telnetClient.disconnect();
        }




        internal int getNumberOfLines()
        {
            return this.csvHandler.getNumOfLines();
        }



        private List<string> getFeaturesFromXml()
        {
            XDocument xml = XDocument.Load(this.xmlPath);
            IEnumerable<string> temp = xml.Descendants("output").Descendants("name").Select(name => (string)name);
            return temp.ToList();
        }

        private int getColumnByFeature(string feature)
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



        public void setFeatureToDisplay(string feature)
        {
            this.featureToDisplay = feature;
        }


        public List<DataPoint> getPointsFromStart(int currentLine, string featurToDisplay)
        {
            List<DataPoint> points = new List<DataPoint>();
            int column = getColumnByFeature(featureToDisplay);
            for (int i = 0; i < currentLine; i++)
            {
                int line = i;
                float value = csvHandler.getFeatureValueByLineAndColumn(line, column);
                points.Add(new DataPoint(line, value));
            }
            return points;

        }



        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void updateProperties()
        {
            elevator = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("elevator"));
            aileron = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("aileron"));
            rudder = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("rudder"));
            throttle = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("throttle"));
            altimeter = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("altimeter_indicated-altitude-ft"));
            airspeed = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("airspeed-kt"));
            direction = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("heading-deg"));
            roll = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("roll-deg"));
            pitch = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("pitch-deg"));
            yaw = this.csvHandler.getFeatureValueByLineAndColumn(CurrentLineNumber, getColumnByFeature("side-slip-deg"));
            Points = getPointsFromStart(CurrentLineNumber, featureToDisplay);

        }

        public float elevator
        {
            get { return _elevator; }
            set
            {
                _elevator = value;
                NotifyPropertyChanged("elevator");
            }
        }

        public float aileron
        {
            get { return _aileron; }
            set
            {
                _aileron = value;
                NotifyPropertyChanged("aileron");
            }
        }

        public float rudder
        {
            get { return _rudder; }
            set
            {
                _rudder = value;
                NotifyPropertyChanged("rudder");
            }
        }

        public float throttle
        {
            get { return _throttle; }
            set
            {
                _throttle = value;
                NotifyPropertyChanged("throttle");
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

        public float roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                NotifyPropertyChanged("roll");
            }
        }


        // get a list of all the features in the csv
        public List<string> FeaturesList
        {
            get { return featuresList; }
            set
            {
                featuresList = value;
                NotifyPropertyChanged(nameof(FeaturesList));

            }
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

        public string getCorrelativeFeature(string feature)
        {
            return null;
        }

        public List<DataPoint> getCorrelativePointsFromStart(int currentLine, string featurToDisplay)
        {
            return null;
        }

        public List<DataPoint> Points
        {
            get { return points; }
            set
            {
                points = value;
                NotifyPropertyChanged(nameof(Points));
            }
        }

        public List<DataPoint> CorrelativePoints
        {
            get { return correlativePoints; }
            set
            {
                points = value;
                NotifyPropertyChanged(nameof(CorrelativePoints));
            }

        }






        // private float currentFeatureValue;

        /*public float getCurrentFeatureValue(string featureToDisplay)
      {
          this.featureToDisplay = featureToDisplay;
          int column = getColumnByFeature(featureToDisplay);
          int line = currentLineNumber;
          return csvHandler.getFeatureValueByLineAndColumn(line, column);
      }*/

        //   CurrentFeatureValue = getCurrentFeatureValue(featureToDisplay);



        // get the current value of the given feature 
        /*  public float CurrentFeatureValue
          {
              get { return currentFeatureValue; }

              set
              {
                  currentFeatureValue = value;
                  NotifyPropertyChanged(nameof(CurrentFeatureValue));
              }
          }*/





    }
}
