using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Chess_FirstStep
{
    public abstract class ChessPiece
    {
        public string Name { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWhite { get; protected set; }
        public bool HasMoved { get; set; }
        public ChessPiece(string name, int y, int x, bool isWhite, bool hasMoved)
        {
            Name = name;
            X = x;
            Y = y;
            IsWhite = isWhite;
            HasMoved = hasMoved;
        }

        
        public ChessPiece(ChessPiece other, int newX, int newY)
        {
            Name = other.Name;
            X = newX;
            Y = newY;
            IsWhite = other.IsWhite;
            HasMoved = other.HasMoved;
        }

        public abstract bool Move(int newX, int newY, bool isCaptured, Chessboard chessboard);
    }
}