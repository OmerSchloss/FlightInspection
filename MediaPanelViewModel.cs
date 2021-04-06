

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInspection
{
    class MediaPanelViewModel : INotifyPropertyChanged
    {
        private FlightgearModel fgModel;
        public MediaPanelViewModel(FlightgearModel fg)
        {
            this.fgModel = fg;
        }

        public bool connectToFG()
        {
            if(fgModel.connect("127.0.0.1", 5400))
            {
                return true;
            }
            return false;
        }

        public void playFg()
        {
            this.fgModel.play();
        }

        public void disconnectFromFG()
        {
            fgModel.disconnect();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}