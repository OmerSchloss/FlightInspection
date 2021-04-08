using System;
using System.Collections.Generic;
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
        private List<DataPoint> correlated_points;
        private List<DataPoint> correlated_line;

        volatile bool stop;
        private TelnetClient telnetClient;
        private CSVHandler csvHandler;
        private List<string> featuresList;
        private Dictionary<int, int> correlations;
        private string csvPath;
        private string fullcsvPath;
        private string xmlPath;
        private string featureToDisplay;
        private string correlativeFeatureToDisplay;
        private bool threatStarted;
        private bool isConnect;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightgearModel(string csvPath, string xmlPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.csvPath = csvPath;
            this.csvHandler = new CSVHandler(csvPath);
            this.xmlPath = xmlPath;
            correlations = new Dictionary<int, int>();
            FeaturesList = getFeaturesFromXml();
            csvHandler.createNewCSV();
            fullcsvPath = "new_reg_flight.csv";
            CurrentLineNumber = 0;
            threatStarted = false;
            stop = false;
            setCorrelatedFeatures();
        }

        public bool connect(string ip, int port)
        {
            if (telnetClient.connect(ip, port))
            {
                return true;
            }
            return false;
        }

        public void disconnect()
        {
            stop = true;
            telnetClient.disconnect();
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
                            this.updateProperties();
                            CurrentLineNumber = CurrentLineNumber + 1;
                        }
                        System.Threading.Thread.Sleep(100);

                    }
                }
                      ).Start();
            }

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
            CorrelativePoints = getCorrelativePointsFromStart(CurrentLineNumber, featureToDisplay);
            Correlated_points = getCorrelatedPointsFromTwoList(Points, CorrelativePoints);
            Correlated_line = getRegLineFromPoints(Correlated_points);
        }
        public List<DataPoint> getCorrelatedPointsFromTwoList(List<DataPoint> first, List<DataPoint> second)
        {
            return null;
        }

        public List<DataPoint> getRegLineFromPoints(List<DataPoint> points)
        {
            return null;
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

        List<float> getValuesOfFeature(int column)
        {
            List<float> listOfValues = new List<float>();
            for (int i = 0; i < getNumberOfLines(); i++)
            {
                listOfValues.Add(csvHandler.getFeatureValueByLineAndColumn(i, column));
            }
            return listOfValues;
        }

        public List<DataPoint> getPointsFromStart(int currentLine, string feature)
        {
            List<DataPoint> points = new List<DataPoint>();
            int column = getColumnByFeature(feature);
            for (int i = 0; i < currentLine; i++)
            {
                int line = i;
                float value = csvHandler.getFeatureValueByLineAndColumn(line, column);
                points.Add(new DataPoint(line, value));
            }
            return points;
        }

        public List<DataPoint> getCorrelativePointsFromStart(int currentLine, string featurToDisplay)
        {
            CorrelativeFeature = getCorrelativeFeature(featureToDisplay);
            return getPointsFromStart(currentLine, CorrelativeFeature);
        }

        public void setCorrelatedFeatures()
        {
            int size = featuresList.Count;
            float max = 0;
            int maxIndex = 0;
            List<float> valuesOfFeaturI;
            List<float> valuesOfFeaturJ;

            for (int i = 0; i < size; i++)
            {
                valuesOfFeaturI = getValuesOfFeature(i);
                if (i != 0)
                {
                    max = Math.Abs(csvHandler.pearson(valuesOfFeaturI, getValuesOfFeature(0)));
                    maxIndex = 0;
                }
                for (int j = 1; j < size; j++)
                {
                    valuesOfFeaturJ = getValuesOfFeature(j);
                    if (i != j && Math.Abs(csvHandler.pearson(valuesOfFeaturI, valuesOfFeaturJ)) > max)
                    {
                        max = Math.Abs(csvHandler.pearson(valuesOfFeaturI, valuesOfFeaturJ));
                        maxIndex = j;
                    }
                }
                correlations.Add(i, maxIndex);
            }
        }

        public string getCorrelativeFeature(string feature)
        {
            int column = getColumnByFeature(feature);
            return featuresList[correlations[column]];
        }

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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
                correlativePoints = value;
                NotifyPropertyChanged(nameof(CorrelativePoints));
            }

        }

        public string CorrelativeFeature
        {
            get { return correlativeFeatureToDisplay; }
            set
            {
                correlativeFeatureToDisplay = value;
                NotifyPropertyChanged(nameof(CorrelativeFeature));
            }
        }
        public List<DataPoint> Correlated_points
        {
            get { return correlated_points; }
            set
            {
                correlated_points = value;
                NotifyPropertyChanged("Correlated_points");
            }
        }
        public List<DataPoint> Correlated_line
        {
            get { return correlated_line; }
            set
            {
                correlated_line = value;
                NotifyPropertyChanged("Correlated_points");
            }
        }
    }
}
