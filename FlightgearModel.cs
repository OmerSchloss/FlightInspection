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
        private List<DataPoint> correlatedFeaturePointsCsv;
        private List<DataPoint> correlatedFeaturePointsDll;

        internal void updateCurrentLine(int v)
        {
            stop = true;
            CurrentLineNumber = v;
            updateProperties();
        }

        private List<DataPoint> pointsOfCorrelatedFeaturesCsv;
        private List<DataPoint> pointsOfCorrelatedFeaturesDll;
        private List<DataPoint> regressionLine;
        private List<DataPoint> anomalyPoints;
        private List<DataPoint> minCircle;
        private List<DataPoint> lineAlgo;
        private List<DataPoint> otherAlgo;

        //#############################################//

        private TelnetClient telnetClient;
        private DataHandler trainHandler;
        private DataHandler detectHandler;
        private AnomalyUtil anomalyUtil;
        private List<string> featuresList;
        private List<string> anomalyListBox;
        private Dictionary<int, int> correlations;
        private Dictionary<int, List<string>> anomalyDict;

        private string trainCsv;
        private string detectCsv;
        private string xmlPath;
        private string detectionAlgorithm;
        private float speed;
        private bool threatStarted;
        private bool isConnect;
        volatile bool stop;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightgearModel(string trainCsv, string detectCsv, string xmlPath, string dllPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.trainCsv = trainCsv;
            this.detectCsv = detectCsv;
            this.xmlPath = xmlPath;
            trainHandler = new DataHandler();
            detectHandler = new DataHandler();
            correlations = new Dictionary<int, int>();
            anomalyUtil = new AnomalyUtil();
            FeaturesList = DataHandler.getFeaturesFromXml(xmlPath);
            DataHandler.createTxtFileFromTwoFiles(trainCsv, detectCsv, FeaturesList);
            trainHandler.csvParser(trainCsv);
            detectHandler.csvParser(detectCsv);
            trainHandler.setFeaturesList(FeaturesList);
            detectHandler.setFeaturesList(FeaturesList);
            CurrentLineNumber = 0;
            threatStarted = false;
            stop = false;
            Speed = 1;
            new Thread(delegate ()
            {
                var assembly = Assembly.LoadFile(dllPath);
                var type = assembly.GetType("regression_line_dll.anomaly_detector");
                var obj = Activator.CreateInstance(type);
                var method = type.GetMethod("detector");
                method.Invoke(obj, null);
                anomalyDict = DataHandler.getOutputTxt("output.txt");
                AnomalyListBox = DataHandler.getAnomalyListBox("output.txt");
                detectionAlgorithm = DataHandler.detectionAlgorithm;
            }).Start();
            setCorrelatedFeaturesFromTrainCsv(this.correlations);
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
                            this.telnetClient.write(this.detectHandler.getDetectionCsvLine(CurrentLineNumber) + "\r\n");
                        }
                        if ((!stop) && (CurrentLineNumber < this.detectHandler.getNumOfLines() - 1))
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
            if (s == 0)
            {
                stop = true;
                return;
            }
            Speed = s;
        }

        internal void setCurrentLineNumber(int value)
        {
            CurrentLineNumber = value;
            updateProperties();
        }

        private void updateProperties()
        {
            elevator = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("elevator"));
            aileron = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("aileron"));
            rudder = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("rudder"));
            throttle = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("throttle"));
            airspeed = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("airspeed-kt"));
            direction = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("heading-deg"));
            roll = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("roll-deg"));
            pitch = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("pitch-deg"));
            yaw = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature("side-slip-deg"));
            altimeter = detectHandler.getFeatureValueByLineAndColumn(CurrentLineNumber,
                                                                detectHandler.getColumnByFeature(
                                                                           "altimeter_indicated-altitude-ft"));
            Points = getPointsFromStart(CurrentLineNumber, featureToDisplay);

            CorrelatedFeaturePointsCsv = getCorrelatedFeaturePointsCsv(CurrentLineNumber, featureToDisplay);

            PointsOfCorrelatedFeaturesCsv = getPointsOfCorrelatedFeatures(featureToDisplay, CorrelatedFeatureCsv);

            RegressionLine = getRegLine(featureToDisplay, CorrelatedFeatureCsv);

            AnomalyPoints = getAnomalyPoints(featureToDisplay, DataHandler.correlated[featureToDisplay]);

            try
            {
                if (detectionAlgorithm == "Line")
                {
                    if (DataHandler.correlated[featureToDisplay] != featureToDisplay)
                    {
                        PointsOfCorrelatedFeaturesDll = getPointsOfCorrelatedFeatures(
                                                    featureToDisplay, DataHandler.correlated[featureToDisplay]
                                                    );
                        LineAlgo = getRegLine(featureToDisplay, DataHandler.correlated[featureToDisplay]);
                    }
                }

                if (detectionAlgorithm == "Circle")
                {
                    if (DataHandler.correlated[featureToDisplay] != featureToDisplay)
                    {
                        PointsOfCorrelatedFeaturesDll = getPointsOfCorrelatedFeatures(
                                                        featureToDisplay, DataHandler.correlated[featureToDisplay]
                                                        );
                        MinCircleAlgo = getCircularPoints(DataHandler.featureAndRadius[featureToDisplay],
                                                          DataHandler.featureAndCenterPoint[featureToDisplay], 0.01);
                    }
                }
                if (detectionAlgorithm == "Other")
                {
                    if (DataHandler.correlated[featureToDisplay] != featureToDisplay)
                    {
                        PointsOfCorrelatedFeaturesDll = getPointsOfCorrelatedFeatures(
                                                        featureToDisplay, DataHandler.correlated[featureToDisplay]
                                                        );
                        OtherAlgo = DataHandler.otherFeatureAndDataPoints[featureToDisplay];
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
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
            CurrentLineNumber = this.detectHandler.getNumOfLines() - 1;
        }

        internal void forwardTenSec()
        {
            if (CurrentLineNumber < this.detectHandler.getNumOfLines() - 101)
            {
                CurrentLineNumber = CurrentLineNumber + 100;
            }
            else
            {
                CurrentLineNumber = this.detectHandler.getNumOfLines() - 1;
            }
        }

        internal int getNumberOfLines()
        {
            return this.detectHandler.getNumOfLines();
        }

        public void setFeatureToDisplay(string feature)
        {
            FeatureToDisplay = feature;
        }

        public void setCorrelatedFeaturesFromTrainCsv(Dictionary<int, int> correlations)
        {
            int size = FeaturesList.Count;
            float max = 0;
            int maxIndex = 0;
            List<float> valuesOfFeaturI;
            List<float> valuesOfFeaturJ;

            for (int i = 0; i < size; i++)
            {
                valuesOfFeaturI = trainHandler.getValuesOfFeature(i);
                if (i != 0)
                {
                    max = Math.Abs(anomalyUtil.pearson(valuesOfFeaturI, trainHandler.getValuesOfFeature(0)));
                    maxIndex = 0;
                }
                for (int j = 1; j < size; j++)
                {
                    valuesOfFeaturJ = trainHandler.getValuesOfFeature(j);
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
            int column = detectHandler.getColumnByFeature(feature);
            for (int i = 0; i < currentLine; i++)
            {
                int line = i;
                float value = detectHandler.getFeatureValueByLineAndColumn(line, column);
                points.Add(new DataPoint(line, value));
            }
            return points;
        }

        public List<DataPoint> getCorrelatedFeaturePointsCsv(int currentLine, string featurToDisplay)
        {
            CorrelatedFeatureCsv = getCorrelativeFeature(featureToDisplay);
            return getPointsFromStart(currentLine, CorrelatedFeatureCsv);
        }

        public List<DataPoint> getPointsOfCorrelatedFeatures(string firstFeature, string secondFeature)
        {
            List<DataPoint> correlatedList = new List<DataPoint>();
            int firstFeatureColumn = detectHandler.getColumnByFeature(firstFeature);
            int secondFeatureColumn = detectHandler.getColumnByFeature(secondFeature);
            int startLine;

            if (CurrentLineNumber >= 300) startLine = CurrentLineNumber - 300;
            else startLine = 0;

            for (int i = startLine; i < CurrentLineNumber; i++)
            {
                float firstFeatureValue = detectHandler.getFeatureValueByLineAndColumn(i, firstFeatureColumn);
                float secondFeatureValue = detectHandler.getFeatureValueByLineAndColumn(i, secondFeatureColumn);
                correlatedList.Add(new DataPoint(firstFeatureValue, secondFeatureValue));
            }
            return correlatedList;
        }

        public List<DataPoint> getRegLine(string firstFeature, string secondFeature)
        {
            List<DataPoint> pointsOfLinearReg = new List<DataPoint>();
            List<float> firstFeatureValues = trainHandler.getValuesOfFeature(firstFeature);
            List<float> secondFeatureValues = trainHandler.getValuesOfFeature(secondFeature);
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

        public List<DataPoint> getAnomalyPoints(string firstFeature, string secondFeature)
        {
            List<DataPoint> anomalyList = new List<DataPoint>();
            int firstFeatureColumn = detectHandler.getColumnByFeature(firstFeature);
            int secondFeatureColumn = detectHandler.getColumnByFeature(secondFeature);
            int startLine;
            if (CurrentLineNumber >= 300) startLine = CurrentLineNumber - 300;
            else startLine = 0;

            for (int i = startLine; i < CurrentLineNumber; i++)
            {
                if (anomalyDict.ContainsKey(i) && anomalyDict[i].Contains(firstFeature))
                {
                    float firstFeatureValue = detectHandler.getFeatureValueByLineAndColumn(i, firstFeatureColumn);
                    float secondFeatureValue = detectHandler.getFeatureValueByLineAndColumn(i, secondFeatureColumn);
                    anomalyList.Add(new DataPoint(firstFeatureValue, secondFeatureValue));

                }
            }
            return anomalyList;
        }

        public static List<DataPoint> getCircularPoints(double radius, DataPoint center, double angleInterval)
        {
            List<DataPoint> points = new List<DataPoint>();

            for (double interval = angleInterval; interval < 2 * Math.PI; interval += angleInterval)
            {
                double X = center.X + (radius * Math.Cos(interval));
                double Y = center.Y + (radius * Math.Sin(interval));

                points.Add(new DataPoint((float)X, (float)Y));
            }

            return points;
        }

        public string getCorrelativeFeature(string feature)
        {
            int column = detectHandler.getColumnByFeature(feature);
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

        public List<string> AnomalyListBox
        {
            get { return anomalyListBox; }
            set
            {
                anomalyListBox = value;
                NotifyPropertyChanged(nameof(AnomalyListBox));
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
                if (value > 0)
                {
                    speed = value;
                    NotifyPropertyChanged(nameof(Speed));
                }

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

        public List<DataPoint> CorrelatedFeaturePointsCsv
        {
            get { return correlatedFeaturePointsCsv; }
            set
            {
                correlatedFeaturePointsCsv = value;
                NotifyPropertyChanged(nameof(CorrelatedFeaturePointsCsv));
            }
        }

        public List<DataPoint> CorrelatedFeaturePointsDll
        {
            get { return correlatedFeaturePointsDll; }
            set
            {
                correlatedFeaturePointsDll = value;
                NotifyPropertyChanged(nameof(CorrelatedFeaturePointsCsv));
            }
        }

        public string CorrelatedFeatureCsv
        {
            get { return correlatedFeatureToDisplay; }
            set
            {
                correlatedFeatureToDisplay = value;
                NotifyPropertyChanged(nameof(CorrelatedFeatureCsv));
            }
        }

        public string FeatureToDisplay
        {
            get { return featureToDisplay; }
            set
            {
                featureToDisplay = value;
                NotifyPropertyChanged(nameof(FeatureToDisplay));
            }
        }

        public string CorrelatedFeatureDll
        {
            get { return correlatedFeatureToDisplay; }
            set
            {
                correlatedFeatureToDisplay = value;
                NotifyPropertyChanged(nameof(CorrelatedFeatureDll));
            }
        }

        public List<DataPoint> PointsOfCorrelatedFeaturesCsv
        {
            get { return pointsOfCorrelatedFeaturesCsv; }
            set
            {
                pointsOfCorrelatedFeaturesCsv = value;
                NotifyPropertyChanged(nameof(PointsOfCorrelatedFeaturesCsv));
            }
        }

        public List<DataPoint> PointsOfCorrelatedFeaturesDll
        {
            get { return pointsOfCorrelatedFeaturesDll; }
            set
            {
                pointsOfCorrelatedFeaturesDll = value;
                NotifyPropertyChanged(nameof(PointsOfCorrelatedFeaturesDll));
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

        public List<DataPoint> LineAlgo
        {
            get { return lineAlgo; }
            set
            {
                lineAlgo = value;
                NotifyPropertyChanged(nameof(LineAlgo));
            }
        }

        public List<DataPoint> MinCircleAlgo
        {
            get { return minCircle; }
            set
            {
                minCircle = value;
                NotifyPropertyChanged(nameof(MinCircleAlgo));
            }
        }
        public List<DataPoint> OtherAlgo
        {
            get { return otherAlgo; }
            set
            {
                otherAlgo = value;
                NotifyPropertyChanged(nameof(otherAlgo));
            }
        }

    }

}
