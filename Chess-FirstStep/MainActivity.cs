using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using Android.Graphics.Drawables;
using System.Collections.Generic;
using Android.Content;

namespace Chess_FirstStep
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        Button btnTwoPlayerGame;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            btnTwoPlayerGame = FindViewById<Button>(Resource.Id.btnTwoPlayerGame);

            btnTwoPlayerGame.Click += BtnTwoPlayerGame_Click;
        }

        private void BtnTwoPlayerGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TwoPlayerGameActivity));

            StartActivity(intent);
        }
    }

    
}