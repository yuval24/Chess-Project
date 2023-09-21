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
    public class Bishop : ChessPiece
    {
        public Bishop(int x, int y, bool isWhite) : base("Bishop", x, y, isWhite, false)
        {
        }
        public Bishop(Bishop other, int newX, int newY) : base(other, newX, newY)
        {
        }
        public override bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard)
        {
            int deltaX = newX - X;
            int deltaY = newY - Y;

            // Bishop can move diagonally
            if (Math.Abs(deltaX) == Math.Abs(deltaY))
            {
                int startX = -1;
                int startY = -1;
                if(deltaX > 0 && deltaY > 0) {
                    startX = X + 1;
                    startY = Y + 1;

                    while (startX < newX)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        startX++;
                        startY++;
                    }
                }
                else if (deltaX < 0 && deltaY > 0)
                {
                    startX = X - 1;
                    startY = Y + 1;

                    while (startX > newX)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        startX--;
                        startY++;
                    }
                }
                else if (deltaX > 0 && deltaY < 0)
                {
                    startX = X + 1;
                    startY = Y - 1;

                    while (startX < newX)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        startX++;
                        startY--;
                    }
                }
                else if (deltaX < 0 && deltaY < 0)
                {
                    startX = X - 1;
                    startY = Y - 1;

                    while (startX > newX)
                    {
                        if (chessboard.GetChessPieceAt(startY, startX) != null)
                        {
                            return false;
                        }
                        startX--;
                        startY--;
                    }
                }
                return true;
            }

            return false; // Invalid move
        }
    }
}