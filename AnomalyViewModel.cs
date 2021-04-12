using System;
using System.Collections.Generic;
using System.ComponentModel;
using OxyPlot;

namespace FlightInspection
{
    class AnomalyViewModel : INotifyPropertyChanged
    {
        private FlightgearModel model;

        public AnomalyViewModel(FlightgearModel model)
        {
            this.model = model;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public List<DataPoint> VM_AnomalyPoints
        {
            get { return model.AnomalyPoints; }
        }

        internal void updateCurrentLine(int v)
        {
            model.updateCurrentLine(v);
        }

        public List<DataPoint> VM_PointsOfCorrelatedFeatures
        {
            get { return model.PointsOfCorrelatedFeatures; }
        }

        public List<DataPoint> VM_LineAlgo
        {
            get { return model.LineAlgo; }
        }

        public List<DataPoint> VM_RegressionLine
        {
            get { return model.RegressionLine; }
        }

        public List<DataPoint> VM_MinCircleAlgo
        {
            get { return model.MinCircleAlgo; }
        }

        public List<string> VM_AnomalyListBox
        {
            get { return model.AnomalyListBox; }
        }

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
