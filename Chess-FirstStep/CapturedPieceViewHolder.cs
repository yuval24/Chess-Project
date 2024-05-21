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
    public class CapturedPieceViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageView { get; }

        public CapturedPieceViewHolder(View itemView) : base(itemView)
        {
            ImageView = itemView.FindViewById<ImageView>(Resource.Id.capturedPieceImage);
        }
    }
}