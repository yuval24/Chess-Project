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
    public class Queen : ChessPiece
    {
        public Queen(int x, int y, bool isWhite) : base("Queen", x, y, isWhite, false)
        {
        }
        public Queen(Queen other, int newX, int newY) : base(other, newX, newY)
        {
        }
        

        public override bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard)
        {
            ChessPiece Bishop = new Bishop(Y, X, this.IsWhite);
            ChessPiece Rook = new Rook(Y, X, this.IsWhite);
            // Queen can move vertically, horizontally, or diagonally
            if (Bishop.Move(newX, newY, isCaptured, chessboard) || Rook.Move(newX, newY, isCaptured, chessboard))
            {

                return true;
            }

            return false; // Invalid move
        }
    }
}