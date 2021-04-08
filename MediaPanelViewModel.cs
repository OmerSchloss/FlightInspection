

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

            fgModel.PropertyChanged +=
        delegate (Object sender, PropertyChangedEventArgs e) {
            NotifyPropertyChanged("VM_" + e.PropertyName);
        };
        }

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public bool connectToFG()
        {
            if(fgModel.connect("127.0.0.1", 5400))
            {
                return true;
            }
            return false;
        }

        public void playFg(bool isConnected)
        {
            this.fgModel.play(isConnected);
        }

        public void pauseFg()
        {
            this.fgModel.pause();
        }


        public void disconnectFromFG()
        {
            fgModel.disconnect();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void startFromZeroFg()
        {
            fgModel.resetCurrent();
        }

        internal void backwardTenSecFg()
        {
            fgModel.backwardTenSec();
        }

        internal int getNumOfLines()
        {
            return fgModel.getNumberOfLines();
        }

        internal void forwardTenSecFg()
        {
            fgModel.forwardTenSec();
        }

        internal void endFg()
        {
            fgModel.endCurrentLine();
        }

        internal void closeThread()
        {
            fgModel.closeThread();
        }

        internal void updateSpeedFg(float s)
        {
            fgModel.updateSpeed(s);
        }

        public int VM_CurrentLineNumber
        {
            get { return fgModel.getCurrentLineNumber(); }
            set
            {
                fgModel.setCurrentLineNumber(value);
            }

        }
        public string VM_TimeString
        {
            get { return fgModel.GetTimeString();
            }
        }
    }
}