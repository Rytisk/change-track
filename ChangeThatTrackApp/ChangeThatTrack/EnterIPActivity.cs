using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.IO;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;

namespace ChangeThatTrack
{
    [Activity(Label = "EnterIPActivity", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EnterIPActivity : Activity
    {

        EditText editTxtIP;
        Button btnEnterIP;
        Button btnAddToList;
        ListView listViewIP;
        List<IP> ips;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnterIP);
            

            editTxtIP = FindViewById<EditText>(Resource.Id.editTxtIP);
            btnEnterIP = FindViewById<Button>(Resource.Id.btnEnterIP);
            btnAddToList = FindViewById<Button>(Resource.Id.btnAddToList);
            listViewIP = FindViewById<ListView>(Resource.Id.listViewIP);

            UploadIPList();

            btnEnterIP.Click += BtnEnterIP_Click;
            btnAddToList.Click += OpenDialog;
            
            listViewIP.ItemClick += ListViewIP_ItemClick;
            listViewIP.ItemLongClick += ListViewIP_ItemLongClick;
        }

        public void UploadIPList()
        {
            ips = new List<IP>();
            foreach (var ip in GetIPList())
            {
                ips.Add(new IP(ip.Key, ip.Value));
            }
            ArrayAdapter<IP> adapter = new ArrayAdapter<IP>(this, Android.Resource.Layout.SimpleListItem1, ips);
            listViewIP.Adapter = adapter;
        }

        private void ListViewIP_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            DeleteIP(ips[e.Position].Name, ips[e.Position].IPAdress);
            UploadIPList();
        }

        private void ListViewIP_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var activity2 = new Intent(this, typeof(MainActivity));
            activity2.PutExtra("MyData", ips[e.Position].IPAdress);
            StartActivity(activity2);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(editTxtIP.WindowToken, 0);
            return base.OnTouchEvent(e);
        }

        private void BtnEnterIP_Click(object sender, EventArgs e)
        {
            var activity2 = new Intent(this, typeof(MainActivity));
            activity2.PutExtra("MyData", editTxtIP.Text.ToString());
            StartActivity(activity2);
        }

        private void OpenDialog(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            DialogRegister logInDialog = new DialogRegister();
            logInDialog.Show(transaction, "dialog fragment");
        }

        public Dictionary<string, string> GetIPList()
        {
            Dictionary<string, string> ipList = new Dictionary<string, string>();
            string key = "";
            string value = "";
            string path = Application.Context.FilesDir.Path;
            var filename = Path.Combine(path.ToString(), "iplist.txt");

            try
            {
                using (StreamReader streamReader = new StreamReader(filename, true))
                {
                    while (!streamReader.EndOfStream)
                    {
                        key = streamReader.ReadLine();
                        value = streamReader.ReadLine();
                        if (key != null && value != null)
                            ipList.Add(key, value);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error writing to file: " + ex.StackTrace);
            }
            return ipList;
        }

        public void DeleteIP(string key, string value)
        {
            string path = Application.Context.FilesDir.Path;
            var filename = Path.Combine(path.ToString(), "iplist.txt");

            var tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(filename).Where(id => id != value  && id != key);

            File.WriteAllLines(tempFile, linesToKeep);

            File.Delete(filename);
            File.Move(tempFile, filename);

            Console.WriteLine("I SHOULD DELETE");
        }

        public void DeleteElement(string el)
        {
            string path = Application.Context.FilesDir.Path;
            var filename = Path.Combine(path.ToString(), "iplist.txt");

            var tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(filename).Where(id => id != el);

            File.WriteAllLines(tempFile, linesToKeep);

            File.Delete(filename);
            File.Move(tempFile, filename);

        }
    }
}