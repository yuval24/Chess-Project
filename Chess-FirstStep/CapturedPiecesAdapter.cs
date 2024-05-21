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
    public class CapturedPiecesAdapter : RecyclerView.Adapter
    {
        private readonly Context context;
        private readonly List<int> capturedPieceIds;

        public CapturedPiecesAdapter(Context context, List<int> capturedPieceIds)
        {
            this.context = context;
            this.capturedPieceIds = capturedPieceIds;
        }

        public override int ItemCount => capturedPieceIds.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is CapturedPieceViewHolder viewHolder)
            {
                viewHolder.ImageView.SetImageResource(capturedPieceIds[position]);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(context).Inflate(Resource.Layout.captured_piece_item, parent, false);
            return new CapturedPieceViewHolder(itemView);
        }

        // Method to add a new piece and notify the adapter
        public void AddCapturedPiece(int pieceId)
        {
            capturedPieceIds.Add(pieceId);
            NotifyItemInserted(capturedPieceIds.Count - 1);
        }
    }

}