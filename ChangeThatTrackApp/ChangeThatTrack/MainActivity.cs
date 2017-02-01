using Android.App;
using Android.Widget;
using Android.OS;

namespace ChangeThatTrack
{
    [Activity(Label = "ChangeThatTrack", MainLauncher = false, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class MainActivity : Activity
    {
        private ImageButton btnPlayPause;
        private ImageButton btnStop;
        private ImageButton btnNext;
        private ImageButton btnPrev;

        private ConnectionHandler conHandler;

        private string ipAddress;

        private const int VK_MEDIA_NEXT_TRACK = 0xB0;
        private const int VK_MEDIA_PREV_TRACK = 0xB1;
        private const int VK_MEDIA_STOP = 0xB2;
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;

        private int currentPlayPauseTag;

        protected override void OnDestroy()
        {
            conHandler.CloseConnection();
            base.OnDestroy();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            ipAddress = Intent.GetStringExtra("MyData") ?? "Data not available";

            conHandler = new ConnectionHandler(ipAddress);
            btnPlayPause = FindViewById<ImageButton>(Resource.Id.btnPlay);
        //    btnStop = FindViewById<Button>(Resource.Id.btnStop);
            btnNext = FindViewById<ImageButton>(Resource.Id.btnNext);
            btnPrev = FindViewById<ImageButton>(Resource.Id.btnPrev);

            currentPlayPauseTag = Resource.Drawable.PlayButton;    


            btnPlayPause.Click += BtnPlay_Click;
            btnPlayPause.Click += ChangePlayPauseImage;
          //  btnStop.Click += BtnStop_Click;
            btnNext.Click += BtnNext_Click;
            btnPrev.Click += BtnPrev_Click;
            

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
            }
        }
    }
}

