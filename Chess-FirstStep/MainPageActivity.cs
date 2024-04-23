using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Chess_FirstStep.Data_Classes;

namespace Chess_FirstStep
{
    [Activity(Label = "LoginPageActivity")]
    public class MainPageActivity : Activity
    {
        Button btnTwoPlayerGame;
        Button btnAiGame;
        Button btnOnlineGame;
        NetworkManager networkManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            networkManager = NetworkManager.Instance;
            Task.Run(() => networkManager.ReconnectAsync());

            btnTwoPlayerGame = FindViewById<Button>(Resource.Id.btnTwoPlayerGame);
            btnAiGame = FindViewById<Button>(Resource.Id.btnAiGame);
            btnOnlineGame = FindViewById<Button>(Resource.Id.btnOnlineGame);

            btnOnlineGame.Click += BtnOnlineGame_Click;
            btnTwoPlayerGame.Click += BtnTwoPlayerGame_Click;
            btnAiGame.Click += BtnAiGame_Click;
        }

        private void BtnOnlineGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(WaitingForOnlineGameActivity));

            StartActivity(intent);
            Finish();
        }

        private void BtnAiGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AIGameActivity));

            StartActivity(intent);
        }

        private void BtnTwoPlayerGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TwoPlayerGameActivity));

            StartActivity(intent);
        }
    }

}