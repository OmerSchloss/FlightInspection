
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



        public List<string> VM_FeaturesList
        {
            get { return model.FeaturesList; }

        }


        // TO DO: binding to this property with X
        public int VM_CurrentLineNumber
        {
            get { return model.CurrentLineNumber; }

        }

        // TO DO: binding to this property with Y
        public float VM_CurrentFeatureValue
        {
            get { return model.CurrentFeatureValue; }
        }




        private string featureToDisplay;
        public string VM_FeatureToDisplay
        {
            get { return featureToDisplay; }
            set
            {
                featureToDisplay = value;
                model.getCurrentFeatureValue(featureToDisplay);
            }
        }




        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
