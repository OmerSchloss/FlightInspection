
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FlightInspection
{
    class FeaturesViewModel : INotifyPropertyChanged
    {
        private FlightgearModel model;


        public FeaturesViewModel(FlightgearModel model)
        {
            this.model = model;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public int VM_CurrentLineNumber
        {
            get { return model.CurrentLineNumber; }

        }

        public List<string> VM_FeaturesList
        {
            get { return model.FeaturesList; }

        }


        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
