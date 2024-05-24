using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Chess_FirstStep
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class HomePageFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_home_page, container, false);

            Button btnTwoPlayerGame = view.FindViewById<Button>(Resource.Id.btnTwoPlayerGame);
            Button btnAiGame = view.FindViewById<Button>(Resource.Id.btnAiGame);
            Button btnOnlineGame = view.FindViewById<Button>(Resource.Id.btnOnlineGame);

            btnOnlineGame.Click += BtnOnlineGame_Click;
            btnTwoPlayerGame.Click += BtnTwoPlayerGame_Click;
            btnAiGame.Click += BtnAiGame_Click;

            return view;
        }

        private void BtnOnlineGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Activity, typeof(WaitingForOnlineGameActivity));
            StartActivity(intent);
            Activity.Finish();
        }

        private void BtnAiGame_Click(object sender, EventArgs e)
        {
            ShowSelectAiDialog();
        }

        private void BtnTwoPlayerGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Activity, typeof(TwoPlayerGameActivity));
            StartActivity(intent);
        }

        private void ShowSelectAiDialog()
        {
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Activity);
            LayoutInflater inflater = Activity.LayoutInflater;
            View dialogView = inflater.Inflate(Resource.Layout.chooseAiDialog, null);
            dialogBuilder.SetView(dialogView);

            Button myBotButton = dialogView.FindViewById<Button>(Resource.Id.myBotButton);
            Button stockfishBotButton = dialogView.FindViewById<Button>(Resource.Id.stockfishBotButton);

            AlertDialog alertDialog = dialogBuilder.Create();

            myBotButton.Click += (sender, e) => {
                Intent intent = new Intent(Activity, typeof(AIGameActivity));
                StartActivity(intent);
            };

            stockfishBotButton.Click += (sender, e) => {
                Intent intent = new Intent(Activity, typeof(StockfishGameActivity));
                StartActivity(intent);
            };

            alertDialog.Show();
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

}