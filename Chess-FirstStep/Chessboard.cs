using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Lights;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Provider.DocumentsContract;

namespace Chess_FirstStep
{
    public class Chessboard
    {
        public bool isWhiteTurn { get; set; }
        private ChessPiece[,] chessPieces;
        private ImageView[,] chessPieceViews;

        public Chessboard()
        {
            // Initialize the arrays
            isWhiteTurn = true;
            chessPieces = new ChessPiece[8, 8];
            
        }

        public Chessboard(Chessboard other)
        {
            // Initialize the arrays
            isWhiteTurn = other.isWhiteTurn;
            chessPieces = new ChessPiece[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    
                    chessPieces[row,col] = other.GetChessPieceAt(row,col);
                }
            }
        }
        public void InitializeChessboard(Context context)
        {
            // Populate the chessboard with pieces and ImageViews
            

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Create and set ChessPiece objects
                    // Populate the chessboard with specific chess pieces
                    // White pieces
                    
                    // White pieces
                    chessPieces[0, 0] = new Rook(0, 0, true);
                    chessPieces[0, 1] = new Knight(0, 1, true);
                    chessPieces[0, 2] = new Bishop(0, 2, true);
                    chessPieces[0, 3] = new Queen(0, 3, true);
                    chessPieces[0, 4] = new King(0, 4, true);
                    chessPieces[0, 5] = new Bishop(0, 5, true);
                    chessPieces[0, 6] = new Knight(0, 6, true);
                    chessPieces[0, 7] = new Rook(0, 7, true);

                    // Black pieces
                    chessPieces[7, 0] = new Rook(7, 0, false);
                    chessPieces[7, 1] = new Knight(7, 1, false);
                    chessPieces[7, 2] = new Bishop(7, 2, false);
                    chessPieces[7, 3] = new Queen(7, 3, false);
                    chessPieces[7, 4] = new King(7, 4, false);
                    chessPieces[7, 5] = new Bishop(7, 5, false);
                    chessPieces[7, 6] = new Knight(7, 6, false);
                    chessPieces[7, 7] = new Rook(7, 7, false);

                    // Pawns

                    for (int i = 0; i < 8; i++)
                    {
                        chessPieces[1, i] = new Pawn(1, i, true);
                        chessPieces[6, i] = new Pawn(6, i, false);
                    }

                    // Empty squares (initialize the rest of the board with null)
                    for (int i = 2; i < 6; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            chessPieces[i, j] = null;
                        }
                    }

                    // Create and set ImageView elements
                    
                }
            }
        }

        // You can add methods to move pieces, update the UI, and handle game logic here

        public ChessPiece GetChessPieceAt(int row, int col)
        {
            return chessPieces[row, col];
        }
        public void SwitchPlayerTurn()
        {
            isWhiteTurn = !isWhiteTurn; // Toggle the player's turn
        }

        public void SetChessPiece(ChessPiece piece, int row, int col)
        {

            
            if (piece != null)
            {

                if (piece.Name == "Pawn")
                {
                    if (piece.IsWhite)
                    {

                        chessPieces[row, col] = new Pawn((Pawn)piece, col, row);
                    }
                    else
                    {
                        chessPieces[row, col] = new Pawn((Pawn)piece, col, row);
                    }

                }
                else if (piece.Name == "King")
                {
                    if (piece.IsWhite)
                    {
                        chessPieces[row, col] = new King((King)piece, col, row);
                    }
                    else
                    {
                        chessPieces[row, col] = new King((King)piece, col, row);
                    }
                }
                else if (piece.Name == "Queen")
                {
                    if (piece.IsWhite)
                    {
                        chessPieces[row, col] = new Queen((Queen)piece, col, row);
                    }
                    else
                    {
                        chessPieces[row, col] = new Queen((Queen)piece, col, row);
                    }
                }
                else if (piece.Name == "Rook")
                {
                    if (piece.IsWhite)
                    {
                        chessPieces[row, col] = new Rook((Rook)piece, col, row);
                    }
                    else
                    {
                        chessPieces[row, col] = new Rook((Rook)piece, col, row);
                    }
                }
                else if (piece.Name == "Knight")
                {
                    if (piece.IsWhite)
                    {
                        chessPieces[row, col] = new Knight((Knight)piece, col, row);
                    }
                    else
                    {
                        chessPieces[row, col] = new Knight((Knight)piece, col, row);
                    }
                }
                else if (piece.Name == "Bishop")
                {
                    if (piece.IsWhite)
                    {
                        chessPieces[row, col] = new Bishop((Bishop)piece, col, row);
                    }
                    else
                    {
                        chessPieces[row, col] = new Bishop((Bishop)piece, col, row);
                    }
                }
            } else
            {
                chessPieces[row, col] = null;
            }

        }

        public Tuple<int, int> FindKingPosition()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {

                    if (this.chessPieces[row, col] is King && this.chessPieces[row, col].IsWhite == isWhiteTurn)
                    {
                        return Tuple.Create(row, col);
                    }
                }
            }
            return Tuple.Create(-1, -1);
        }

        public bool IsInCheck()
        {
            // Find the position of the player's king
            Tuple<int, int> kingPosition = FindKingPosition();

            // Opponent's color (the color that can put the king in check)
            bool opponentColor = !isWhiteTurn;

            // Iterate through the opponent's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessPieces[row, col];

                    // Check if the piece belongs to the opponent
                    if (piece != null && piece.IsWhite == opponentColor)
                    {
                        // Check if the opponent's piece can legally move to capture the king
                        if (piece.Move(kingPosition.Item2, kingPosition.Item1, true, this))
                        {
                            // The player's king is in check
                            return true;
                        }
                    }
                }
            }

            // The player's king is not in check
            return false;
        }

        public bool IsInCheckCertianLocation(int targtedRow, int targtedCol)
        {
            // Find the position of the player's king
            Tuple<int, int> kingPosition = Tuple.Create(targtedRow, targtedCol); 
            



            // Opponent's color (the color that can put the king in check)
            bool opponentColor = !isWhiteTurn;

            // Iterate through the opponent's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessPieces[row, col];

                    // Check if the piece belongs to the opponent
                    if (piece != null && piece.IsWhite == opponentColor)
                    {
                        // Check if the opponent's piece can legally move to capture the king
                        if (piece.Move(kingPosition.Item2, kingPosition.Item1, true, this))
                        {
                            // The player's king is in check
                            return true;
                        }
                    }
                }
            }

            // The player's king is not in check
            return false;
        }

        

       


    }

   
}