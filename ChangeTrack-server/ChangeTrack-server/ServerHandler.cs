using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeTrack_server
{
    class ServerHandler
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        private static Socket s;
        private static TcpListener myList;

        private static int[] availableCodes = new int[] { 174,     //Volume down
                                                          175,     //Volume up
                                                          176,     //Next track
                                                          177,     //Previous track
                                                          178,     //Stop track
                                                          179 };   //Play/pause track

        public static string GetLocalIP()
        {
            string ip = "127.0.0.1";
            long max = 0;
            try
            {   
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    foreach (var x in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if (x.Address.AddressFamily == AddressFamily.InterNetwork && x.IsDnsEligible && adapter.OperationalStatus == OperationalStatus.Up && adapter.GetIPStatistics().BytesReceived > max)
                        {
                            max = adapter.GetIPStatistics().BytesReceived;
                            ip = x.Address.ToString();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return ip;
        }

        public void Start()
        {
            IPAddress ipAd = IPAddress.Parse(GetLocalIP());
            myList = new TcpListener(ipAd, 8001);
            myList.Start();

            while (true)
            {
                RunTheServer();
                OnProcessExit();
            }
        }


        public void RunTheServer()
        {
            try
            {
                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" +
                                  myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
                s = myList.AcceptSocket();
                WorkOnConnection();
            }
            catch (Exception)
            {
                Console.WriteLine("Connection terminated.");
            }
        }

        private string GetVolume()
        {
            CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            return defaultPlaybackDevice.Volume.ToString();
        }

        private void WorkOnConnection()
        {
            while (s.Connected)
            {

                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                byte[] b = new byte[100];
                int k = s.Receive(b);
                int code = BitConverter.ToInt32(b, 0);
                Console.WriteLine("Recieved... Code: " + code);
                if (IsAvailableCommand(code))
                    CompleteCommand(code);
                else
                    Console.WriteLine("Command not available");
                if (code == 174 || code == 175)
                    SendAcknowledgement(GetVolume());
                else
                    SendAcknowledgement("Works fine");
                
            }
        }

        private void SendAcknowledgement(string message)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes(message));
            Console.WriteLine("\nSent Acknowledgement");
        }

        private bool IsAvailableCommand(int code)
        {
            foreach (var num in availableCodes)
            {
                if (num == code)
                    return true;
            }
            return false;
        }

        private void CompleteCommand(int code)
        {
            keybd_event(Convert.ToByte(code), 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(Convert.ToByte(code), 0, KEYEVENTF_KEYUP, 0);
            Console.WriteLine("Command completed");
        }

        private void OnProcessExit()
        {
            if (s.Connected)
                s.Close();
        }

        public void OnProgramExit(object sender, EventArgs args)
        {
            try
            {
                OnProcessExit();
                myList.Stop();
            }
            catch(Exception)
            {
                Console.WriteLine("Couldnt close connection...");
            }
        }
    }
}
