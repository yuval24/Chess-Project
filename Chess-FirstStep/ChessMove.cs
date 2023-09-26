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
    public class ChessMove
    {
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public int EndRow { get; set; }
        public int EndCol { get; set; }

        public bool IsIllegalMove { get; set; }
        public bool IsCapture { get; set; }
        public bool IsPromotion { get; set; }
        public char PromotedTo { get; set; }
        public bool IsEnPassantCapture { get; set; }
        public bool IsKingsideCastle { get; set; }
        public bool IsQueensideCastle { get; set; }

        public ChessMove(int startRow, int startCol, int endRow, int endCol)
        {
            IsIllegalMove = false;
            StartRow = startRow;
            StartCol = startCol;
            EndRow = endRow;
            EndCol = endCol;
            IsCapture = false;
            IsPromotion = false;
            PromotedTo = ' ';
            IsEnPassantCapture = false;
            IsKingsideCastle = false;
            IsQueensideCastle = false;
        }

        public override string ToString()
        {
            // Convert the move to standard chess notation.
            char startFile = (char)('a' + StartCol);
            char endFile = (char)('a' + EndCol);
            int startRank = 8 - StartRow;
            int endRank = 8 - EndRow;
            string moveString = $"{startFile}{startRank}{endFile}{endRank}";

            if (IsPromotion)
            {
                moveString += PromotedTo;
            }
            else if (IsKingsideCastle)
            {
                moveString = "O-O";
            }
            else if (IsQueensideCastle)
            {
                moveString = "O-O-O";
            }

            return moveString;
        }
    }
}