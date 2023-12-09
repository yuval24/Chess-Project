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
    public class King : ChessPiece
    {
        
        public King(int x, int y, bool isWhite) : base("King", x, y, isWhite, false)
        {
            
        }
        public King(King other, int newY, int newX) : base(other, newY, newX)
        {
            
        }
        public override bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard)
        {
            int deltaX = Math.Abs(newX - X);
            int deltaY = Math.Abs(newY - Y);

            // King can move one square in any direction
            if (deltaX <= 1 && deltaY <= 1 )
            {
               
                return true;
            }
            

            return false; // Invalid move
        }

        public bool castle(int newX, int newY, Chessboard chessboard)
        {
            int deltaX = newX - X;
            int deltaY = newY - Y;
            //check for castle
            if (deltaY == 0 && !HasMoved)
            {

                if (IsWhite)
                {
                    if (deltaX == 2 && chessboard.GetChessPieceAt(0, 5) == null)
                    {
                        if (chessboard.GetChessPieceAt(0, 7) is Rook && !chessboard.GetChessPieceAt(0, 7).HasMoved)
                        {
                            return true;
                        }

                    }

                    if (deltaX == -2 && chessboard.GetChessPieceAt(0, 3) == null && chessboard.GetChessPieceAt(0, 1) == null)
                    {

                        if (chessboard.GetChessPieceAt(0, 0) is Rook && !chessboard.GetChessPieceAt(0, 0).HasMoved)
                        {
                            return true;
                        }

                    }
                }
                else
                {
                    if (deltaX == 2 && chessboard.GetChessPieceAt(7, 5) == null)
                    {
                        if (chessboard.GetChessPieceAt(07, 7) is Rook && !chessboard.GetChessPieceAt(7, 7).HasMoved)
                        {
                            return true;
                        }

                    }

                    if (deltaX == -2 && chessboard.GetChessPieceAt(7, 3) == null && chessboard.GetChessPieceAt(7, 1) == null)
                    {
                        if (chessboard.GetChessPieceAt(7, 0) is Rook && !chessboard.GetChessPieceAt(7, 0).HasMoved)
                        {
                            return true;
                        }

                    }
                }

            }
            return false;
        }
    }
}