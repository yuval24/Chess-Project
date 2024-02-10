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
    public class CapturedPiecesAdapter : BaseAdapter<int>
    {
        private readonly Context context;
        private readonly List<int> capturedPieceIds;

        public CapturedPiecesAdapter(Context context, List<int> capturedPieceIds)
        {
            this.context = context;
            this.capturedPieceIds = capturedPieceIds;
        }

        public override int Count => capturedPieceIds.Count;

        public override long GetItemId(int position) => position;

        public override int this[int position] => capturedPieceIds[position];

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;
            if (convertView == null)
            {
                imageView = new ImageView(context);
                imageView.LayoutParameters = new AbsListView.LayoutParams(150, 150); // Adjust size as needed
                imageView.SetScaleType(ImageView.ScaleType.FitCenter);
            }
            else
            {
                imageView = (ImageView)convertView;
            }

            // Load image from resources using its ID
            imageView.SetImageResource(capturedPieceIds[position]);

            return imageView;
        }
    }

}