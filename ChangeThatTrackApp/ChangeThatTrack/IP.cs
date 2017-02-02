using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ChangeThatTrack
{
    class IP
    {
        public string Name
        {
            get;
            set;
        }
        public string IPAdress
        {
            get;
            set;
        }

        public IP(string name, string ip)
        {
            Name = name;
            IPAdress = ip;
        }

        public override string ToString()
        {
            return Name + ": " + IPAdress;
        }
    }
}