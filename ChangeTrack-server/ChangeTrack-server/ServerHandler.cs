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
        private static TcpListener check;

        Thread t;

        CoreAudioDevice defaultPlaybackDevice;

        private static int[] availableCodes = new int[] { 174,     //Volume down
                                                          175,     //Volume up
                                                          176,     //Next track
                                                          177,     //Previous track
                                                          178,     //Stop track
                                                          179 };   //Play/pause track

        public ServerHandler()
        {
            defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
        }

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
            }
        }


        public void RunTheServer()
        {
            try
            {
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);

                s = myList.AcceptSocket();
                Console.WriteLine("Client connected");
                if (t != null)
                {
                    t.Abort();
                }
                t = new Thread(new ThreadStart(WorkOnConnection));
                t.Start();
            }
            catch (Exception)
            {
                Console.WriteLine("Connection terminated.");
            }
        }

        private string GetVolume()
        {
            return defaultPlaybackDevice.Volume.ToString();
        }

        private void SetVolume(int change)
        {
            defaultPlaybackDevice.Volume += change;    
        }

        private void WorkOnConnection()
        {
            while (s.IsConnected())
            {
                byte[] b = new byte[100];
                int k = s.Receive(b);
                int code = BitConverter.ToInt32(b, 0);
                Console.WriteLine("Recieved... Code: " + code);
                if(code == 9)
                {
                    Console.WriteLine("App closed");
                    break;
                }
                if (IsAvailableCommand(code))
                {
                    if (code == 174 || code == 175)
                        CompleteVolumeCommand(code);
                    else
                        CompleteCommand(code);
                }
                else
                    Console.WriteLine("Command witch code: " + code + " not available");

                if (code == 174 || code == 175)
                    SendAcknowledgement(GetVolume());
                else
                    SendAcknowledgement("Works fine");
                
            }
        }

        private void CompleteVolumeCommand(int code)
        {
            if (code == 174)
                SetVolume(-10);
            else if (code == 175)
                SetVolume(10);
        }

        private void SendAcknowledgement(string message)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes(message));
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
