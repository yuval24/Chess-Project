using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using static Chess_FirstStep.HistoryAdapter;

namespace Chess_FirstStep
{
    public class HistoryAdapter : RecyclerView.Adapter
    {
        private OnItemClickListener listener;

        public interface OnItemClickListener
        {
            void OnItemClick(int position);
        }

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
                historyViewHolder.SetItemClickListener(listener);
            }
        }

        public void SetOnItemClickListener(OnItemClickListener listener)
        {
            this.listener = listener;
        }

        public override int ItemCount => gameHistoryList.Count;
    }

    public class HistoryViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        private TextView textView_black_player_name;
        private TextView textView_white_player_name;
        public TextView GameDate { get; private set; }
        public ImageView Logo { get; private set; }
        private TextView textViewResult;
        private OnItemClickListener listener;

        public HistoryViewHolder(View itemView) : base(itemView)
        {
            textView_black_player_name = itemView.FindViewById<TextView>(Resource.Id.textView_black_player_name);
            textView_white_player_name = itemView.FindViewById<TextView>(Resource.Id.textView_white_player_name);
            textViewResult = itemView.FindViewById<TextView>(Resource.Id.textView_result);
            GameDate = itemView.FindViewById<TextView>(Resource.Id.textView_game_date);
            Logo = itemView.FindViewById<ImageView>(Resource.Id.imageView_logo);

            // Set click listener
            itemView.SetOnClickListener(this);
        }

        public void BindData(GameHistory gameHistory)
        {
            textView_black_player_name.Text = "black : " + gameHistory.black_username;
            textView_white_player_name.Text = "white: " + gameHistory.white_username;
            textViewResult.Text = "won: " + gameHistory.result;
            GameDate.Text = gameHistory.gameDate;
        }

        public void SetItemClickListener(OnItemClickListener listener)
        {
            this.listener = listener;
        }

        public void OnClick(View v)
        {
            // Check if the listener is set and notify the listener about the item click
            listener?.OnItemClick(AdapterPosition);
        }
    }
}
