

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInspection
{
    class MediaPanelViewModel : NotifyPropertyChanged
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

        public void disconnectFromFG()
        {
            fgModel.disconnect();
        }
    }
}