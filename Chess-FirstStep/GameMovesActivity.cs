using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Chess_FirstStep
{
    [Activity(Label = "GameMovesActivity")]
    public class GameMovesActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_moves_activity_layout);

            IList<string> movesList = Intent.GetStringArrayListExtra("GameMoves");

            // Explicitly convert IList<string> to List<string>
            List<string> moves = new List<string>(movesList);

            // Display the moves in the UI
            TextView movesTextView = FindViewById<TextView>(Resource.Id.movesTextView);
            movesTextView.Text = string.Join("\n", moves);
        }
    }
}