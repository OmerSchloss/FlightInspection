namespace FlightInspection
{
    class FlightgearModel : NotifyPropertyChanged
    {

        //public event PropertyChangedEventHandler PropertyChanged;
        TelnetClient telnetClient;
        //volatile Boolean stop;
        private CSVHandler csvHandler;
        private string csvPath;

        public FlightgearModel(string csvPath, TelnetClient telnetClient)
        {
            this.telnetClient = telnetClient;
            this.csvPath = csvPath;
            this.csvHandler = new CSVHandler(csvPath);
            //stop = false;
        }
        //public void connect(string ip, int port)
        //{
        //    telnetClient.connect(ip, port);
        //}

        //public void disconnect()
        //{
        //    stop = true;
        //    telnetClient.disconnect();
        //}


        //public void start()
        //{
        //    newThread(delegate () {
        //        while (!stop)
        //        {
        //            telnetClient.write("get left sonar");
        //            LeftSonar = Double.Parse(telnetClient.read());
        //            // the same for the other sensors propertiesThread.Sleep(250);
        //            // read the data in 4Hz
        //        }
        //    }).Start();
        //}

    }
}
