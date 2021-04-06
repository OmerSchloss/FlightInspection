using System;
using System.Diagnostics;
using System.ComponentModel;

namespace FlightInspection
{
    class JoystickViewModel : INotifyPropertyChanged
    {
        
       
        private FlightgearModel model;
        public event PropertyChangedEventHandler PropertyChanged;


        public JoystickViewModel(FlightgearModel model)
        {
            this.model = model;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e) {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


        public float VM_elevator
        {
            get {
                return (model.elevator * 25) + 55; }
            set {}
        }
        
             
        public float VM_aileron
        {
            get {
                return (model.aileron * 25) + 100; }
            set {}
        }

        public float VM_rudder
        {
            get { 
                return model.rudder; }
            set {}
        }

        public float VM_throttle
        {
            get { 
                return model.throttle; }
            set {}
        }

        //public void NotifyPropertyChanged(string propName) { ...} //means the model has been changed

    }
}
