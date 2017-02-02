using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Views;
using Android.Content;

namespace ChangeThatTrack
{
    [Activity(Label = "ChangeThatTrack", MainLauncher = false, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class MainActivity : Activity
    {
        private ImageButton btnPlayPause;
        private ImageButton btnStop;
        private ImageButton btnNext;
        private ImageButton btnPrev;
        private ImageButton btnVolumeUp;
        private ImageButton btnVolumeDown;

        private ProgressBar barVolume;

        private ConnectionHandler conHandler;

        private string ipAddress;

        private const int VK_MEDIA_NEXT_TRACK = 0xB0;
        private const int VK_MEDIA_PREV_TRACK = 0xB1;
        private const int VK_MEDIA_STOP = 0xB2;
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const int VK_VOLUME_DOWN = 0xAE;
        private const int VK_VOLUME_UP = 0xAF;

        private int currentPlayPauseTag = Resource.Drawable.PlayButton;

        protected override void OnDestroy()
        {
            conHandler.CloseConnection();
            Console.WriteLine("Closing app");
            base.OnDestroy();
        }
         

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView (Resource.Layout.Main);

            ipAddress = Intent.GetStringExtra("MyData") ?? "Data not available";

            try {
                conHandler = new ConnectionHandler(ipAddress);
            }
            catch(Exception)
            {
                Console.WriteLine("FAIL");
            }
            btnPlayPause = FindViewById<ImageButton>(Resource.Id.btnPlay);
        //  btnStop = FindViewById<Button>(Resource.Id.btnStop);
            btnNext = FindViewById<ImageButton>(Resource.Id.btnNext);
            btnPrev = FindViewById<ImageButton>(Resource.Id.btnPrev);
            btnVolumeUp = FindViewById<ImageButton>(Resource.Id.btnVolumeUp);
            btnVolumeDown = FindViewById<ImageButton>(Resource.Id.btnVolumeDown);

            barVolume = FindViewById<ProgressBar>(Resource.Id.barVolume);
            barVolume.Max = 100;


            btnVolumeDown.Click += BtnVolumeDown_Click;
            btnVolumeUp.Click += BtnVolumeUp_Click;
            btnPlayPause.Click += BtnPlay_Click;
            btnPlayPause.Click += ChangePlayPauseImage;
          //  btnStop.Click += BtnStop_Click;
            btnNext.Click += BtnNext_Click;
            btnPrev.Click += BtnPrev_Click;
            

        }
        
        
        private void BtnVolumeUp_Click(object sender, EventArgs e)
        {
            conHandler.Send(VK_VOLUME_UP);
            barVolume.Progress = Convert.ToInt32(conHandler.Answer);
        }

        private void BtnVolumeDown_Click(object sender, EventArgs e)
        {
            conHandler.Send(VK_VOLUME_DOWN);
            barVolume.Progress = Convert.ToInt32(conHandler.Answer);
        }

        private void BtnPrev_Click(object sender, System.EventArgs e)
        {
            conHandler.Send(VK_MEDIA_PREV_TRACK);
        }

        private void BtnNext_Click(object sender, System.EventArgs e)
        {
            conHandler.Send(VK_MEDIA_NEXT_TRACK);
        }

        private void BtnStop_Click(object sender, System.EventArgs e)
        {
            conHandler.Send(VK_MEDIA_STOP);
        }

        private void BtnPlay_Click(object sender, System.EventArgs e)
        {
            conHandler.Send(VK_MEDIA_PLAY_PAUSE);
        }

        private void ChangePlayPauseImage(object sender, System.EventArgs e)
        {
            if(currentPlayPauseTag == Resource.Drawable.PlayButton)
            {
                btnPlayPause.SetImageResource(Resource.Drawable.PauseButton);
                currentPlayPauseTag = Resource.Drawable.PauseButton;
            }
            else
            {
                btnPlayPause.SetImageResource(Resource.Drawable.PlayButton);
                currentPlayPauseTag = Resource.Drawable.PlayButton;
            }
        }
    }
}

