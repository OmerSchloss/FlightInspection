using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FlightInspection
{
    class TelnetClient
    {
        private Socket client = null;
/*        private IPHostEntry ipHost;
        private IPAddress ipAddr;
        private IPEndPoint localEndPoint;*/
        public bool connect(string ip, int port)
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(ip);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
                this.client = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    this.client.Connect(localEndPoint);
                    return true;
                }
                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            return false;
        }


        public void disconnect()
        {
            if (this.client != null)
            {
                this.client.Close();
            }
        }
        public void write(string command)
        {
            if (this.client != null)
            {
                byte[] messageSent = Encoding.ASCII.GetBytes("Test Client<EOF>");
                int byteSent = this.client.Send(messageSent);
            }
        }
        public string read()
        {
            if (this.client != null)
            {
                // Data buffer
                byte[] messageReceived = new byte[1024];

                // We receive the messagge using
                // the method Receive(). This
                // method returns number of bytes
                // received, that we'll use to
                // convert them to string
                int byteRecv = this.client.Receive(messageReceived);
                return Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
            }
            return null;
        }

    }
}