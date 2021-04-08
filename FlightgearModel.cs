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
        private string featureToDisplay;
        private string correlatedFeatureToDisplay;
        private List<DataPoint> points;
        private List<DataPoint> correlatedFeaturePoints;
        private List<DataPoint> pointsForRegression;
        private List<DataPoint> regressionLine;

        volatile bool stop;
        private TelnetClient telnetClient;
        private CSVHandler csvHandler;
        private AnomalyUtil anomalyUtil;
        private List<string> featuresList;
        private Dictionary<int, int> correlations;
        private string csvPath;
        private string fullcsvPath;
        private string xmlPath;
        private bool threatStarted;
        private bool isConnect;
        private float speed;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightgearModel(string csvPath, string xmlPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.csvPath = csvPath;
            this.xmlPath = xmlPath;
            this.csvHandler = new CSVHandler(csvPath);
            anomalyUtil = new AnomalyUtil();
            correlations = new Dictionary<int, int>();
            FeaturesList = getFeaturesFromXml();
            csvHandler.createNewCSV();
            fullcsvPath = "new_reg_flight.csv";
            CurrentLineNumber = 0;
            threatStarted = false;
            stop = false;
            setCorrelatedFeatures();
            Speed = 1;
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
                        if ((!stop) && (CurrentLineNumber < this.csvHandler.getNumOfLines() - 1))
                        {
                            this.updateProperties();
                            CurrentLineNumber = CurrentLineNumber + 1;
                        }
                        System.Threading.Thread.Sleep((int)(100 / Speed));

                    }
                }
                      ).Start();
            }

        }

        internal int getCurrentLineNumber()
        {
            return CurrentLineNumber;
        }

        internal void updateSpeed(float s)
        {
            Speed = s;
        }

        internal void setCurrentLineNumber(int value)
        {
            CurrentLineNumber = value;
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
            CorrelatedFeaturePoints = getCorrelatedFeaturePointsFromStart(CurrentLineNumber, featureToDisplay);
            PointsForRegression = getPointsFromCorrelatedFeatures(featureToDisplay, CorrelatedFeature);
            RegressionLine = getRegLineFromPoints(featureToDisplay, CorrelatedFeature);
        }

        internal void closeThread()
        {
            threatStarted = false;
        }

        internal string GetTimeString()
        {
            TimeSpan ts = TimeSpan.FromSeconds((double)(CurrentLineNumber / 10));
            return new DateTime(ts.Ticks).ToString("HH:mm:ss");
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
            CurrentLineNumber = this.csvHandler.getNumOfLines() - 1;
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
                    max = Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, getValuesOfFeature(0)));
                    maxIndex = 0;
                }
                for (int j = 1; j < size; j++)
                {
                    valuesOfFeaturJ = getValuesOfFeature(j);
                    if (i != j && Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, valuesOfFeaturJ)) > max)
                    {
                        max = Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, valuesOfFeaturJ));
                        maxIndex = j;
                    }
                }
                correlations.Add(i, maxIndex);
            }
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
        List<float> getValuesOfFeature(string feature)
        {
            List<float> listOfValues = new List<float>();
            int column = getColumnByFeature(feature);
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

        public List<DataPoint> getCorrelatedFeaturePointsFromStart(int currentLine, string featurToDisplay)
        {
            CorrelatedFeature = getCorrelativeFeature(featureToDisplay);
            return getPointsFromStart(currentLine, CorrelatedFeature);
        }

        public List<DataPoint> getPointsFromCorrelatedFeatures(string firstFeature, string secondFeature)
        {
            List<DataPoint> correlatedList = new List<DataPoint>();
            int firstFeatureColumn = getColumnByFeature(firstFeature);
            int secondFeatureColumn = getColumnByFeature(secondFeature);
            int startLine;

            if (CurrentLineNumber >= 300) startLine = CurrentLineNumber - 300;
            else startLine = 0;

            for (int i = startLine; i < CurrentLineNumber; i++)
            {
                float firstFeatureValue = csvHandler.getFeatureValueByLineAndColumn(i, firstFeatureColumn);
                float secondFeatureValue = csvHandler.getFeatureValueByLineAndColumn(i, secondFeatureColumn);
                correlatedList.Add(new DataPoint(firstFeatureValue, secondFeatureValue));
            }
            return correlatedList;
        }

        public List<DataPoint> getRegLineFromPoints(string firstFeature, string secondFeature)
        {
            List<DataPoint> pointsOfLinearReg = new List<DataPoint>();
            List<float> firstFeatureValues = getValuesOfFeature(firstFeature);
            List<float> secondFeatureValues = getValuesOfFeature(secondFeature);
            Line line = anomalyUtil.linear_reg(firstFeatureValues, secondFeatureValues);
            int startLine;

            if (CurrentLineNumber >= 300) startLine = CurrentLineNumber - 300;
            else startLine = 0;

            for (int i = startLine; i < CurrentLineNumber; i++)
            {
                pointsOfLinearReg.Add(new DataPoint(firstFeatureValues[i], line.f(firstFeatureValues[i])));
            }

            return pointsOfLinearReg;
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
                NotifyPropertyChanged("TimeString");
            }
        }

        public float Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                NotifyPropertyChanged(nameof(Speed));
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

        public List<DataPoint> CorrelatedFeaturePoints
        {
            get { return correlatedFeaturePoints; }
            set
            {
                correlatedFeaturePoints = value;
                NotifyPropertyChanged(nameof(CorrelatedFeaturePoints));
            }

        }

        public string CorrelatedFeature
        {
            get { return correlatedFeatureToDisplay; }
            set
            {
                correlatedFeatureToDisplay = value;
                NotifyPropertyChanged(nameof(CorrelatedFeature));
            }
        }
        public List<DataPoint> PointsForRegression
        {
            get { return pointsForRegression; }
            set
            {
                pointsForRegression = value;
                NotifyPropertyChanged(nameof(PointsForRegression));
            }
        }
        public List<DataPoint> RegressionLine
        {
            get { return regressionLine; }
            set
            {
                regressionLine = value;
                NotifyPropertyChanged(nameof(RegressionLine));
            }
        }
    }
}
