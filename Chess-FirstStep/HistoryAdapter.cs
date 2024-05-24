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
using AndroidX.RecyclerView.Widget;

namespace Chess_FirstStep
{
    public class HistoryAdapter : RecyclerView.Adapter
    {
        private List<GameHistory> gameHistoryList;

        public HistoryAdapter(List<GameHistory> gameHistoryList)
        {
            this.gameHistoryList = gameHistoryList;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_game_history, parent, false);
            return new HistoryViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is HistoryViewHolder historyViewHolder)
            {
                GameHistory gameHistory = gameHistoryList[position];
                historyViewHolder.BindData(gameHistory);
            }
        }

        public override int ItemCount => gameHistoryList.Count;
    }

    public class HistoryViewHolder : RecyclerView.ViewHolder
    {
        private TextView textView_black_player_name;
        private TextView textView_white_player_name;

        private TextView textViewResult;

        public HistoryViewHolder(View itemView) : base(itemView)
        {
            textView_black_player_name = itemView.FindViewById<TextView>(Resource.Id.textView_black_player_name);
            textView_white_player_name = itemView.FindViewById<TextView>(Resource.Id.textView_white_player_name);
            textViewResult = itemView.FindViewById<TextView>(Resource.Id.textView_result);
        }

        public void BindData(GameHistory gameHistory)
        {
            textView_black_player_name.Text = "black : " +gameHistory.black_username;
            textView_white_player_name.Text =  "white: " +gameHistory.white_username;
            textViewResult.Text = "won: " + gameHistory.result;
        }
    }
}