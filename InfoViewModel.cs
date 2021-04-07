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
            get { return (float)(Math.Floor(model.altimeter * 100) / 100); }
            set { }
        }

        //airspeed-kt    
        public float VM_airspeed
        {
            get { return (float)(Math.Floor(model.airspeed * 100) / 100); }
            set { }
        }

        public float VM_rudder
        {
            get { return (float)(Math.Floor(model.rudder * 100) / 100); }
            set { }
        }

        public float VM_throttle
        {
            get { return model.throttle; }
            set { }
        }

        public float VM_direction
        {
            get { return (float)(Math.Floor(model.direction * 100) / 100); }
            set { }
        }

        public float VM_roll
        {
            get { return (float)(Math.Floor(model.roll * 100) / 100); }
            set { }
        }

        public float VM_pitch
        {
            get { return (float)(Math.Floor(model.pitch * 100) / 100); }
            set { }
        }

        public float VM_yaw
        {
            get { return (float)(Math.Floor(model.yaw * 100) / 100); }
            set { }
        }

        //public void NotifyPropertyChanged(string propName) { ...} //means the model has been changed


    }
}
