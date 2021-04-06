namespace FlightInspection
{
    class JoystickViewModel
    {
        private FlightgearModel model;
        //public event PropertyChangedEventHandler PropertyChanged;
        public JoystickViewModel(FlightgearModel model)
        {
            this.model = model;
            //model.PropertyChanged +=
            //    delegate (Object sender, PropertyChangedEventArgs e) {
            //        NotifyPropertyChanged("VM_" + e.PropertyName);
            //    };
        }


        public float elevator
        {
            get { return (model.elevator * 25) + 55; }
            set { }
        }
        public float aileron
        {
            get { return (model.aileron * 25) + 90; }
            set { }
        }

        public float rudder
        {
            get { return model.rudder; }
            set { }
        }

        public float throttle
        {
            get { return model.throttle; }
            set { }
        }

        //public void NotifyPropertyChanged(string propName) { ...} //means the model has been changed

    }
}
