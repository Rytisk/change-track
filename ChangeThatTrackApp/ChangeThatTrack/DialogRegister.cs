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
using System.IO;
using Android.Views.InputMethods;

namespace ChangeThatTrack
{
    class DialogRegister : DialogFragment
    {
        private EditText editTxtIPToSave;
        private EditText editTxtName;
        private Button btnSaveIP;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.AddIPDialog, container, false);

            btnSaveIP = view.FindViewById<Button>(Resource.Id.btnSaveIP);
            editTxtIPToSave = view.FindViewById<EditText>(Resource.Id.editTxtIPToSave);
            editTxtName = view.FindViewById<EditText>(Resource.Id.editTxtName);

            btnSaveIP.Click += SaveIPButtonClicked;

            return view;
        }

        private void SaveIPButtonClicked(object sender, EventArgs args)
        {
            SaveString(editTxtName.Text, editTxtIPToSave.Text);
            Dismiss();
            var intent = new Intent(this.Activity, typeof(EnterIPActivity));
            StartActivity(intent);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;
        }

        

        public void SaveString(string key, string value)
        {
            string path = Application.Context.FilesDir.Path;
            var filename = Path.Combine(path.ToString(), "iplist.txt");

            try
            {
                using(StreamWriter streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(key);
                    streamWriter.WriteLine(value);
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine("Error writing to file: " + ex.StackTrace);
            }
            Console.WriteLine("I SHOULD WRITE");
        }
    }
}