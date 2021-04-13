
using System;
using System.Collections.Generic;
using System.ComponentModel;
using OxyPlot;

namespace FlightInspection
{
    class FeaturesViewModel : INotifyPropertyChanged
    {
        private FlightgearModel model;
        private string featureToDisplay;

        public FeaturesViewModel(FlightgearModel model)
        {
            this.model = model;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> VM_FeaturesList
        {
            get { return model.FeaturesList; }
        }

        public int VM_CurrentLineNumber
        {
            get { return model.CurrentLineNumber; }
        }

        public List<DataPoint> VM_Points
        {
            get { return model.Points; }
        }

        public List<DataPoint> VM_CorrelatedFeaturePointsCsv
        {
            get { return model.CorrelatedFeaturePointsCsv; }
        }

        public List<DataPoint> VM_PointsOfCorrelatedFeaturesCsv
        {
            get { return model.PointsOfCorrelatedFeaturesCsv; }
        }

        public List<DataPoint> VM_RegressionLine
        {
            get { return model.RegressionLine; }
        }

        public List<DataPoint> VM_LineAlgo
        {
            get { return model.LineAlgo; }
        }

        public string VM_FeatureToDisplay
        {
            get { return model.FeatureToDisplay; }
            set
            {
                featureToDisplay = value;
                model.setFeatureToDisplay(featureToDisplay);
            }
        }

        public string VM_CorrelatedFeatureCsv
        {
            get { return model.CorrelatedFeatureCsv; }
        }



        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
