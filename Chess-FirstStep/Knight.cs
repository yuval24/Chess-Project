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
    public class Knight : ChessPiece
    {
        public Knight(int x, int y, bool isWhite) : base("Knight", x, y, isWhite, false)
        {
        }
        public Knight(Knight other, int newX, int newY) : base(other, newX, newY)
        {
        }
        public override bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard)
        {
            int deltaX = Math.Abs(newX - X);
            int deltaY = Math.Abs(newY - Y);

            // Knight can move in an L-shape: 2 squares in one direction and 1 square perpendicular
            if ((deltaX == 2 && deltaY == 1) || (deltaX == 1 && deltaY == 2))
            {
                return true;
            }

            return false; // Invalid move
        }
    }
}