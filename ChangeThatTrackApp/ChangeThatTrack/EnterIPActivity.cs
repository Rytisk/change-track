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
    [Activity(Label = "EnterIPActivity", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EnterIPActivity : Activity
    {

        EditText editTxtIP;
        Button btnEnterIP;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnterIP);

            editTxtIP = FindViewById<EditText>(Resource.Id.editTxtIP);
            btnEnterIP = FindViewById<Button>(Resource.Id.btnEnterIP);

            btnEnterIP.Click += BtnEnterIP_Click;

            // Create your application here
        }

        private void BtnEnterIP_Click(object sender, EventArgs e)
        {
            var activity2 = new Intent(this, typeof(MainActivity));
            activity2.PutExtra("MyData", editTxtIP.Text.ToString());
            StartActivity(activity2);
        }
    }
}