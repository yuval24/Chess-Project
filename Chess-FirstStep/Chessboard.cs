﻿using System;
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
        public int piecesOnTheBoard { get; set; }
        public int countFor50Moves { get; set; }
        private List<string> positionHistory;
        private Dictionary<string, int> positionCount;
        private const int SomeThreshold = 1000;

        public Chessboard()
        {
            // Initialize the arrays
            isWhiteTurn = true;
            chessPieces = new ChessPiece[8, 8];
            piecesOnTheBoard = 32;
            countFor50Moves = 0;
            positionHistory = new List<string>();
            positionCount = new Dictionary<string, int>();


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
                    chessPieces[row, col] = new Pawn((Pawn)piece, col, row);

                }
                else if (piece.Name == "King")
                {
                    chessPieces[row, col] = new King((King)piece, col, row);
                }
                else if (piece.Name == "Queen")
                {
                    chessPieces[row, col] = new Queen((Queen)piece, col, row);
                }
                else if (piece.Name == "Rook")
                {
                    chessPieces[row, col] = new Rook((Rook)piece, col, row);
                }
                else if (piece.Name == "Knight")
                {
                    chessPieces[row, col] = new Knight((Knight)piece, col, row);
                }
                else if (piece.Name == "Bishop")
                {
                    chessPieces[row, col] = new Bishop((Bishop)piece, col, row);
                    
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

        public bool insufficientMatrielDraw()
        {
            int usefullBlackPieces = 0;
            int usefullWhitePieces = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessPieces[row,col];

                    if (piece != null && !(piece is King))
                    {
                        if (piece is Bishop || piece is Knight)
                        {
                            if (piece.IsWhite)
                            {
                                usefullWhitePieces++;
                            }
                            else
                            {
                                usefullBlackPieces++;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            if (usefullBlackPieces > 1 || usefullWhitePieces > 1)
            {
                return false;
            }
            return true;
        }

        public bool achieved50Moves()
        {
            if(countFor50Moves == 50)
            {
                return true;
            }
            return false;
        }

        public bool drawByRepitition()
        {
            string currentPosition = EncodeBoardPosition(); // Implement this method

            // Add the current position to the position history
            positionHistory.Add(currentPosition);

            // Check if the current position has occurred before
            if (positionCount.ContainsKey(currentPosition))
            {
                positionCount[currentPosition]++;
            }
            else
            {
                positionCount[currentPosition] = 1;
            }

            // Check if the same position has occurred three times (draw by repetition)
            if (positionCount[currentPosition] >= 3)
            {
                // Declare a draw due to repetition
                return true;
            }

            // Clear old positions from history if needed to prevent excessive memory usage
            if (positionHistory.Count > SomeThreshold)
            {
                string removedPosition = positionHistory[0];
                positionHistory.RemoveAt(0);

                // Update the count of the removed position
                if (positionCount.ContainsKey(removedPosition))
                {
                    int count = positionCount[removedPosition];
                    if (count == 1)
                    {
                        positionCount.Remove(removedPosition);
                    }
                    else
                    {
                        positionCount[removedPosition] = count - 1;
                    }
                }
            }
            return false;
        }

        private string EncodeBoardPosition()
        {
            StringBuilder encodedPosition = new StringBuilder();

            // Loop through the entire board and encode each piece
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessPieces[row,col];

                    if (piece != null)
                    {
                        // Append a unique representation of each piece
                        char pieceChar = GetPieceChar(piece, piece.IsWhite);
                        encodedPosition.Append(pieceChar);
                    }
                    else
                    {
                        // Use a character to represent an empty square
                        encodedPosition.Append('-');
                    }
                }
            }

            // Append additional information if needed (e.g., player turn, castling rights, etc.)

            return encodedPosition.ToString();
        }

        private char GetPieceChar(ChessPiece piece, bool isWhite)
        {
            char pieceChar = '?'; // Default if no mapping is defined

            if (piece is Pawn)
            {
                pieceChar = isWhite ? 'P' : 'p';
            }
            else if (piece is Rook)
            {
                pieceChar = isWhite ? 'R' : 'r';
            }
            else if (piece is Knight)
            {
                pieceChar = isWhite ? 'N' : 'n';
            }
            else if (piece is Bishop)
            {
                pieceChar = isWhite ? 'B' : 'b';
            }
            else if (piece is Queen)
            {
                pieceChar = isWhite ? 'Q' : 'q';
            }
            else if (piece is King)
            {
                pieceChar = isWhite ? 'K' : 'k';
            }

            return pieceChar;
        }

        // Check if the current player is in checkmate
        public bool IsCheckmate()
        {
            // Iterate through all the player's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessPieces[row, col];

                    // Check if the piece belongs to the player
                    if (piece != null && piece.IsWhite == isWhiteTurn)
                    {
                        // Try all possible moves for the piece
                        for (int newRow = 0; newRow < 8; newRow++)
                        {
                            for (int newCol = 0; newCol < 8; newCol++)
                            {
                                if (chessPieces[newRow, newCol] == null)
                                {
                                    ChessPiece currPiece = chessPieces[row, col];

                                    if (currPiece.Move(newCol, newRow, false, this))
                                    {
                                        SetChessPiece(currPiece, newRow, newCol);
                                        SetChessPiece(null, row, col);

                                        // Check if the king is still in check after the move
                                        if (!IsInCheck())
                                        {
                                            // The player can make a move that gets them out of check
                                            SetChessPiece(currPiece, row, col);
                                            SetChessPiece(null, newRow, newCol);
                                            return false;
                                        }
                                        SetChessPiece(currPiece, row, col);
                                        SetChessPiece(null, newRow, newCol);
                                    }
                                }
                                else
                                {
                                    if (piece.IsWhite != chessPieces[newRow, newCol].IsWhite)
                                    {
                                        ChessPiece currPiece = chessPieces[row, col];
                                        ChessPiece otherPiece = chessPieces[newRow, newCol];

                                        if (currPiece.Move(newCol, newRow, true, this))
                                        {
                                            SetChessPiece(currPiece, newRow, newCol);
                                            SetChessPiece(null, row, col);

                                            // Check if the king is still in check after the move
                                            if (!IsInCheck())
                                            {
                                                // The player can make a move that gets them out of check
                                                SetChessPiece(currPiece, row, col);
                                                SetChessPiece(otherPiece, newRow, newCol);
                                                return false;
                                            }
                                            SetChessPiece(currPiece, row, col);
                                            SetChessPiece(otherPiece, newRow, newCol);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // If no legal moves can get the player out of check, it's checkmate
            return true;
        }
    }

   
}