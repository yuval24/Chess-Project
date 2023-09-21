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
    public class Rook : ChessPiece
    {

        public Rook(int x, int y, bool isWhite) : base("Rook", x, y, isWhite, false)
        {
            HasMoved = false;
        }
        public Rook(Rook other, int newX, int newY) : base(other, newX, newY)
        {
            
        }
        public override bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard)
        {
            int deltaX = newX - X;
            int deltaY = newY - Y;

            // Rook can move vertically or horizontally
            if (deltaX == 0 || deltaY == 0)
            {
                int startX = -1;
                int startY = -1;
                if (deltaX == 0 && deltaY > 0)
                {
                    startX = X;
                    startY = Y + 1;
                    
                    while (startY < newY)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        
                        startY++;
                    }
                }
                else if (deltaX == 0 && deltaY < 0)
                {
                    startX = X;
                    startY = Y - 1;

                    while (startY > newY)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        
                        startY--;
                    }
                }
                else if (deltaX > 0 && deltaY == 0)
                {
                    startX = X + 1;
                    startY = Y;

                    while (startX < newX)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        startX++;
                    }
                }
                else if (deltaX < 0 && deltaY == 0)
                {
                    startX = X - 1;
                    startY = Y;

                    while (startX > newX)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        startX--;
                    }
                }
                HasMoved = true;
                return true;
            }

            return false; // Invalid move
        }
    }
}