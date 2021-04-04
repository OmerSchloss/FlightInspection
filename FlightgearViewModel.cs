using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInspection
{
    class FlightgearViewModel : NotifyPropertyChanged
    {
        private FlightgearModel model;
        //public event PropertyChangedEventHandler PropertyChanged;
        public FlightgearViewModel(FlightgearModel model)
        {
            this.model = model;
            //model.PropertyChanged +=
            //    delegate (Object sender, PropertyChangedEventArgs e) {
            //        NotifyPropertyChanged("VM_" + e.PropertyName);
            //    };
        }


        //public void NotifyPropertyChanged(string propName) { ...} //means the model has been changed

    }
}
