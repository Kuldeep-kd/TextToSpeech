using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.TextToSpeech;
using System.Threading;
using Plugin.TextToSpeech.Abstractions;
using System.Linq;
using System.Collections.Generic;


namespace TextToSpeech
{
    [Activity(Label = "TextToSpeech", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        static float pitch = 1, volume = 1, speed = 1;
        static Plugin.TextToSpeech.TextToSpeech tts;
        static TextView textbox;
        static Button button;
        static SeekBar spitch, svolume, sspeed;
        static TextView pitchtext, speedtext, volumetext;
        static List<string> lis;
        static Spinner languageSpinner;
        static ArrayAdapter adapter;
        static CrossLocale? crosslocale;
        static IEnumerable<CrossLocale> locale;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            


            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Main);
            PopulateLanguagesAsync();

            #region ControlsInitialisation

            tts = new Plugin.TextToSpeech.TextToSpeech();
            textbox = FindViewById<TextView>(Resource.Id.textBox);
            button = FindViewById<Button>(Resource.Id.btnPlay);
            spitch = FindViewById<SeekBar>(Resource.Id.seekerPitch);
            svolume = FindViewById<SeekBar>(Resource.Id.seekerVolume);
            sspeed = FindViewById<SeekBar>(Resource.Id.seekerSpeed);
            pitchtext = FindViewById<TextView>(Resource.Id.textPitch);
            speedtext = FindViewById<TextView>(Resource.Id.textSpeed);
            volumetext = FindViewById<TextView>(Resource.Id.textVolume);
            languageSpinner = FindViewById<Spinner>(Resource.Id.langspinner);
            crosslocale = new CrossLocale?();
            


            #endregion

            #region seekCalculation

            spitch.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
            {
                if (e.FromUser)
                {
                    pitch = e.Progress;
                    pitch /= 100;
                    pitchtext.Text = string.Format("Pitch {0:0.00}", pitch);
                }
            };
            sspeed.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
            {
                if (e.FromUser)
                {
                    speed = e.Progress;
                    speed /= 50;
                    speedtext.Text = string.Format("Speed {0:0.00}", speed);
                }
            };
            svolume.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
            {
                if (e.FromUser)
                {
                    volume = e.Progress;
                    volume /= 100;
                    volumetext.Text = string.Format("Volume {0:0.00}", volume);
                }
            };
            #endregion

            languageSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs m) =>
                {
                    crosslocale = locale.ElementAt(int.Parse( m.Id.ToString() ));
                };
            
            

            button.Click += async delegate {
                button.Enabled = false;
                await tts.Speak(textbox.Text, crosslocale, pitch, speed, volume, CancellationToken.None);
                button.Enabled = true;
                
           };
        }

        private async void PopulateLanguagesAsync()
        {
            locale = await CrossTextToSpeech.Current.GetInstalledLanguages();
            lis = new List<string>();
            lis = (locale.Select(a => a.ToString()).ToList());
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lis);
            languageSpinner.Adapter = adapter;
            crosslocale = locale.FirstOrDefault();
            Toast.MakeText(this," Populated ",ToastLength.Short);
        }
    }

}