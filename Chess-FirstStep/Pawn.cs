using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;

namespace Chess_FirstStep
{
    public class Pawn : ChessPiece
    {
        

        public Pawn(int y, int x, bool isWhite) : base("Pawn", y, x, isWhite, false)
        {
            
        }
        public Pawn(Pawn other, int newX, int newY) : base(other, newX, newY)
        {
            
        }

        public override bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard)
        {
            
            int deltaX = newX - X;
            int deltaY = newY - Y;

            // Determine the valid moves for a pawn (assuming standard chess rules)
            if (IsWhite)
            {
                if (!HasMoved && deltaY == 2 && deltaX == 0 && chessboard.GetChessPieceAt(Y + 1, X) == null && !isCaptured)
                {
                    // Pawn can move two squares forward on its first move
                    this.HasMoved = true;
                    return true;
                }
                else if (deltaY == 1 && deltaX == 0 && !isCaptured)
                {
                    // Pawn can move one square forward
                    HasMoved = true;
                    return true;
                }
                else if (deltaY == 1 && Math.Abs(deltaX) == 1 && isCaptured)
                {
                    // Pawn can capture diagonally
                    HasMoved = true;
                    return true;
                }
            }
            else // Black pawn
            {
                if (!HasMoved && deltaY == -2 && deltaX == 0 && chessboard.GetChessPieceAt(Y - 1, X) == null && !isCaptured)
                {
                    // Pawn can move two squares forward on its first move
                    HasMoved = true;
                    return true;
                }
                else if (deltaY == -1 && deltaX == 0 && !isCaptured)
                {
                    // Pawn can move one square forward
                    HasMoved = true;
                    return true;
                }
                else if (deltaY == -1 && Math.Abs(deltaX) == 1 && isCaptured)
                {
                    // Pawn can capture diagonally
                    HasMoved = true;
                    return true;
                }
            }

            

            return false; // Invalid move
        }

        public bool enPassant(int newX, int newY, Chessboard chessboard)
        {
            int deltaX = newX - X;
            int deltaY = newY - Y;

            if (IsWhite)
            {
                if(Y == 4 && deltaY == 1)
                {
                    if(deltaX == -1 && chessboard.GetChessPieceAt(Y,X-1) is Pawn)
                    {
                        return true;
                    }
                    else if (deltaX == 1 && chessboard.GetChessPieceAt(Y, X + 1) is Pawn)
                    {
                        return true;
                    }

                }
            } else
            {
                if (Y == 3 && deltaY == -1)
                {
                    if (deltaX == -1 && chessboard.GetChessPieceAt(Y, X - 1) is Pawn)
                    {
                        return true;
                    }
                    else if (deltaX == 1 && chessboard.GetChessPieceAt(Y, X + 1) is Pawn)
                    {
                        return true;
                    }

                }
            }
            return false;

        }

        public bool validPromtion(int newY)
        {

            if (IsWhite)
            {
                if (newY == 7)
                {
                    return true;
                }
            } else
            {
                if (newY == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}