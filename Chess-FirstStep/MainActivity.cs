using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using Android.Graphics.Drawables;
using System.Collections.Generic;
using Android.Content;
using Chess_FirstStep.Data_Classes;
using System.Threading.Tasks;
using System.Threading;

namespace Chess_FirstStep
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private static int SplashTimeOut = 3000;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.openning_screen);

         

            new Handler(Looper.MainLooper).PostDelayed(() =>
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
                Finish();
            }, SplashTimeOut);
        }

       
    }
}