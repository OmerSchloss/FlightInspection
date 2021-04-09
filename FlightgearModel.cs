using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
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
        private List<DataPoint> anomalyPoints;

        //#############################################//

        private TelnetClient telnetClient;
        private DataHandler dataHandler;
        private AnomalyUtil anomalyUtil;
        private List<string> featuresList;
        private Dictionary<int, int> correlations;
        private Dictionary<int, List<string>> anomalyDict;
        private string learnCsv;
        private string detectCsv;
        private string xmlPath;
        private float speed;
        private bool threatStarted;
        private bool isConnect;
        volatile bool stop;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightgearModel(string learnCsv, string detectCsv, string xmlPath, string dllPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.learnCsv = learnCsv;
            this.detectCsv = detectCsv;
            this.xmlPath = xmlPath;
            dataHandler = new DataHandler(learnCsv, detectCsv, xmlPath);
            correlations = new Dictionary<int, int>();
            anomalyUtil = new AnomalyUtil();
            dataHandler.csvParser(learnCsv);
            FeaturesList = dataHandler.getFeaturesFromXml();
            setCorrelatedFeatures();
            dataHandler.csvParser(detectCsv);
            dataHandler.createNewTxtFileFromTwoFiles(learnCsv, detectCsv);
            CurrentLineNumber = 0;
            threatStarted = false;
            stop = false;
            Speed = 1;
            //string dllpath = @"C:\Users\רון אליאב\source\repos\implement";
            //string dllFile = dllpath + "\\";
            var assembly = Assembly.LoadFile(dllPath);
            var type = assembly.GetType("implement.Program");
            var obj = Activator.CreateInstance(type);
            var method = type.GetMethod("Main");
            method.Invoke(obj, new object[] { });
            anomalyDict = dataHandler.getOutputTxt("output.txt");
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
                            this.telnetClient.write(this.dataHandler.getDetectionCsvLine(CurrentLineNumber) + "\r\n");
                        }
                        if ((!stop) && (CurrentLineNumber < this.dataHandler.getNumOfLines() - 1))
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
            elevator = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("elevator"));
            aileron = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("aileron"));
            rudder = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("rudder"));
            throttle = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("throttle"));
            airspeed = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("airspeed-kt"));
            direction = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("heading-deg"));
            roll = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("roll-deg"));
            pitch = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("pitch-deg"));
            yaw = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature("side-slip-deg"));
            altimeter = dataHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                dataHandler.getColumnByFeature(
                                                                           "altimeter_indicated-altitude-ft"));
            Points = getPointsFromStart(CurrentLineNumber, featureToDisplay);
            CorrelatedFeaturePoints = getCorrelatedFeaturePointsFromStart(CurrentLineNumber, featureToDisplay);
            PointsForRegression = getPointsFromCorrelatedFeatures(featureToDisplay, CorrelatedFeature);
            RegressionLine = getRegLineFromPoints(featureToDisplay, CorrelatedFeature);
            AnomalyPoints = getAnomalyPoints(featureToDisplay, CorrelatedFeature);


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
            CurrentLineNumber = this.dataHandler.getNumOfLines() - 1;
        }

        internal void forwardTenSec()
        {
            if (CurrentLineNumber < this.dataHandler.getNumOfLines() - 101)
            {
                CurrentLineNumber = CurrentLineNumber + 100;
            }
            else
            {
                CurrentLineNumber = this.dataHandler.getNumOfLines() - 1;
            }
        }

        internal int getNumberOfLines()
        {
            return this.dataHandler.getNumOfLines();
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
                valuesOfFeaturI = dataHandler.getValuesOfFeature(i);
                if (i != 0)
                {
                    max = Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, dataHandler.getValuesOfFeature(0)));
                    maxIndex = 0;
                }
                for (int j = 1; j < size; j++)
                {
                    valuesOfFeaturJ = dataHandler.getValuesOfFeature(j);
                    if (i != j && Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, valuesOfFeaturJ)) > max)
                    {
                        max = Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, valuesOfFeaturJ));
                        maxIndex = j;
                    }
                }
                correlations.Add(i, maxIndex);
            }
        }

        public List<DataPoint> getPointsFromStart(int currentLine, string feature)
        {
            List<DataPoint> points = new List<DataPoint>();
            int column = dataHandler.getColumnByFeature(feature);
            for (int i = 0; i < currentLine; i++)
            {
                int line = i;
                float value = dataHandler.getFeatureValueByLineAndColumn(line, column);
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
            int firstFeatureColumn = dataHandler.getColumnByFeature(firstFeature);
            int secondFeatureColumn = dataHandler.getColumnByFeature(secondFeature);
            int startLine;

            if (CurrentLineNumber >= 300) startLine = CurrentLineNumber - 300;
            else startLine = 0;

            for (int i = startLine; i < CurrentLineNumber; i++)
            {
                float firstFeatureValue = dataHandler.getFeatureValueByLineAndColumn(i, firstFeatureColumn);
                float secondFeatureValue = dataHandler.getFeatureValueByLineAndColumn(i, secondFeatureColumn);
                correlatedList.Add(new DataPoint(firstFeatureValue, secondFeatureValue));
            }
            return correlatedList;
        }
        public List<DataPoint> getAnomalyPoints(string firstFeature, string secondFeature)
        {
            List<DataPoint> anomalyList = new List<DataPoint>();
            int firstFeatureColumn = dataHandler.getColumnByFeature(firstFeature);
            int secondFeatureColumn = dataHandler.getColumnByFeature(secondFeature);
            int startLine;
            if (CurrentLineNumber >= 300) startLine = CurrentLineNumber - 300;
            else startLine = 0;

            for (int i = startLine; i < CurrentLineNumber; i++)
            {
                if (anomalyDict.ContainsKey(i) && anomalyDict[i].Contains(firstFeature))
                {
                    float firstFeatureValue = dataHandler.getFeatureValueByLineAndColumn(i, firstFeatureColumn);
                    float secondFeatureValue = dataHandler.getFeatureValueByLineAndColumn(i, secondFeatureColumn);
                    anomalyList.Add(new DataPoint(firstFeatureValue, secondFeatureValue));

                }
            }
            return anomalyList;
        }
        public List<DataPoint> getRegLineFromPoints(string firstFeature, string secondFeature)
        {
            List<DataPoint> pointsOfLinearReg = new List<DataPoint>();
            List<float> firstFeatureValues = dataHandler.getValuesOfFeature(firstFeature);
            List<float> secondFeatureValues = dataHandler.getValuesOfFeature(secondFeature);
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
            int column = dataHandler.getColumnByFeature(feature);
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

        public List<DataPoint> AnomalyPoints
        {
            get { return anomalyPoints; }
            set
            {
                anomalyPoints = value;
                NotifyPropertyChanged(nameof(AnomalyPoints));
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
