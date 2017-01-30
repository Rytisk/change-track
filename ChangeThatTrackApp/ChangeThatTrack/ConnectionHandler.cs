using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace ChangeThatTrack
{
    public class ConnectionHandler
    {
        TcpClient tcpclnt;
        
        public string SongInfo
        {
            get;
            set;
        }
       

        public ConnectionHandler(string ipAddress)
        {
            tcpclnt = new TcpClient();
            try
            {

                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(ipAddress, 8001);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error..... " + ex.StackTrace);
            }
        }
        public void Send(int code)
        {
            Stream stm = tcpclnt.GetStream();

            byte[] ba = BitConverter.GetBytes(code);
            Console.WriteLine("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);

            Console.WriteLine("**************************");
            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(bb[i]));
            Console.WriteLine("");
            Console.WriteLine("**************************");
        }

        public void CloseConnection()
        {
            tcpclnt.Close();
        }
    }
}
