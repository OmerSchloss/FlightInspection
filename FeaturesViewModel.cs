
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

        public List<DataPoint> VM_CorrelativePoints
        {
            get { return model.CorrelativePoints; }
        }

        public string VM_FeatureToDisplay
        {
            get { return featureToDisplay; }
            set
            {
                featureToDisplay = value;
                model.setFeatureToDisplay(featureToDisplay);
            }
        }

        public string VM_CorrelativeFeature
        {
            get { return model.CorrelativeFeature; }
        }


        public List<DataPoint> VM_Correlated_points
        {
            get { return model.Correlated_points; }
        }

        public List<DataPoint> VM_Correlated_line
        {
            get { return model.Correlated_points; }
        }


        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
