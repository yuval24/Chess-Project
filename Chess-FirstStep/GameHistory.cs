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

namespace Chess_FirstStep
{
    public class GameHistory
    {
        public string black_username { get; set; }
        public string white_username { get; set; }
        public string result { get; set; }
        public string gameDate { get; set; }
        public List<string> moves { get; set; } 

        public GameHistory(string black_username, string white_username, string result, string gameDate, List<string> moves)
        {
            this.black_username = black_username;
            this.white_username = white_username;
            this.result = result;
            this.moves = moves;
            this.gameDate = gameDate;
        }
    }
}