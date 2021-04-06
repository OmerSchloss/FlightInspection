using System;
using System.Diagnostics;
using System.ComponentModel;

namespace FlightInspection
{
    class InfoViewModel : INotifyPropertyChanged
    {


        private FlightgearModel model;
        public event PropertyChangedEventHandler PropertyChanged;


        public InfoViewModel(FlightgearModel model)
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

        //altimeter_indicated-altitude-ft
        public float VM_altimeter
        {
            get { return model.altimeter; }
            set { }
        }

        //airspeed-kt    
        public float VM_airspeed
        {
            get { return model.airspeed; }
            set { }
        }

        public float VM_rudder
        {
            get { return model.rudder; }
            set { }
        }

        public float VM_throttle
        {
            get { return model.throttle; }
            set { }
        }

        public float VM_direction
        {
            get { return model.direction; }
            set { }
        }

        public float VM_roll
        {
            get { return model.roll; }
            set { }
        }

        public float VM_pitch
        {
            get { return model.pitch; }
            set { }
        }

        public float VM_yaw
        {
            get { return model.yaw; }
            set { }
        }

        //public void NotifyPropertyChanged(string propName) { ...} //means the model has been changed

    }
}
