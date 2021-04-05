using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInspection
{
    class JoystickViewModel : NotifyPropertyChanged
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
            get { return (0 * 50) + 30; }
            set { }
        }

        //public void NotifyPropertyChanged(string propName) { ...} //means the model has been changed

    }
}
