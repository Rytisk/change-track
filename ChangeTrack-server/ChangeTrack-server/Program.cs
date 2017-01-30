using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    class Program
    {
        public static void Main()
        {
            //ServerHandler server = new ServerHandler();
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(server.OnProgramExit);
            //server.Start();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                foreach (var x in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (x.Address.AddressFamily == AddressFamily.InterNetwork && x.IsDnsEligible)
                    {
                        Console.WriteLine(" IPAddress ........ : {0:x}", x.Address.ToString());
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
