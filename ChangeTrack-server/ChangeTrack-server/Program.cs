using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeTrack_server
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        private static Socket s;
        private static TcpListener myList;

        private static int[] availableCodes = new int[] {176,177,178,179};
        static string songInfo = "";

        public static void Main()
        {    
 
               
            
            IPAddress ipAd = IPAddress.Parse("172.16.12.210");
             
            myList = new TcpListener(ipAd, 8001);

           
            myList.Start();

            while (true)
            {
                Thread t = new Thread(new ThreadStart(RunTheServer));
                t.Start();
                t.Join();
                OnProcessExit();
            }

            myList.Stop();
        }

        static void OnProcessExit()
        {
            s.Close();
        }

        static void RunTheServer()
        {
            
            try
            {

                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" +
                                  myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
                s = myList.AcceptSocket();
                while (s.Connected)
                {

                    Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                    byte[] b = new byte[100];
                    int k = s.Receive(b);
                    Console.WriteLine("Recieved...");
                    int code = BitConverter.ToInt32(b, 0);
                    Console.WriteLine(code);
                    foreach(var num in availableCodes)
                    {
                        if(num == code)
                        {
                            keybd_event(Convert.ToByte(code), 0, KEYEVENTF_EXTENDEDKEY, 0);
                            keybd_event(Convert.ToByte(code), 0, KEYEVENTF_KEYUP, 0);
                            break;
                        }
                    }
                   
                   
                    ASCIIEncoding asen = new ASCIIEncoding();
                    s.Send(asen.GetBytes("Hi"));
                    Console.WriteLine("\nSent Acknowledgement");
                    /* clean up */
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Connection terminated.");
            }
            
        }

        static string GetSongInfo()
        {
            return Process.GetProcessById(7572).MainWindowTitle; 
        }
    }
}
