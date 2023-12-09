using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Nio.Channels;
using Org.Apache.Http.Conn.Routing;

namespace Chess_FirstStep
{
    public class ChessAI
    {
        private bool AIisWhite; // Specify the color of the AI
        private readonly int searchDepth;

        public ChessAI(bool AIisWhite, int searchDepth)
        {
            this.AIisWhite = AIisWhite;
            this.searchDepth = searchDepth;
        }

        public ChessMove GetBestMove(Chessboard board)
        {
            List<ChessMove> moves = GenerateLegalMoves(board);
            ChessMove bestMove = null;
            int maxScore = int.MinValue;

            foreach(ChessMove move in moves)
            {
                Chessboard cloneBoard = new Chessboard(board);   
                cloneBoard.ApplyMove(move);

                int eval = -MiniMax(cloneBoard, searchDepth -1);

                if( eval > maxScore)
                {
                    maxScore = Math.Max(maxScore, eval);
                    bestMove = move;
                }
                

            }
            return bestMove;
        }

        private int MiniMax(Chessboard board, int searchDepth)
        {
            if(searchDepth == 0)
            {
                return EvaluateBoard(board);
            }

            board.SwitchPlayerTurn();
            List<ChessMove> moves = GenerateLegalMoves(board);
            int maxScore = int.MinValue;

            if(moves.Count == 0)
            {
                if (board.IsCheckmate())
                {
                    return int.MinValue;
                }
                return 0;
            }

            foreach(ChessMove move in moves)
            {
                Chessboard cloneboard = new Chessboard(board);
                cloneboard.ApplyMove(move);

                int eval = -MiniMax(cloneboard, searchDepth -1);

                maxScore = Math.Max(maxScore, eval);
            }
            return maxScore;
        }

        private List<ChessMove> GenerateLegalMoves(Chessboard board)
        {
            // Implement logic to generate a list of legal moves based on the current board state.
            // This involves considering the positions and possible moves of all pieces.
            // Return a list of ChessMove objects representing legal moves.
            List<ChessMove> legalMoves = new List<ChessMove>();

            // Iterate through all the player's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = board.GetChessPieceAt(row, col);

                    // Check if the piece belongs to the player
                    if (piece != null && piece.IsWhite == board.isWhiteTurn)
                    {
                        // Try all possible moves for the piece
                        for (int newRow = 0; newRow < 8; newRow++)
                        {
                            for (int newCol = 0; newCol < 8; newCol++)
                            {
                                ChessMove move = new ChessMove(row, col, newRow, newCol);

                                if (board.IsMoveValid(move))
                                {
                                    legalMoves.Add(move);
                                }
                            }
                        }
                    }
                }
            }


            return legalMoves;

        }

      
        public int EvaluateBoard(Chessboard board)
        {
            int whiteScore = 0;
            int blackScore = 0;

            // Evaluate pieces on the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = board.GetChessPieceAt(row, col);
                    if(piece != null)
                    {
                        if (piece.IsWhite)
                        {
                            whiteScore += GetPieceValue(piece);
                            whiteScore += GetPieceSquareValue(piece);
                        }
                        else
                        {
                            blackScore += GetPieceValue(piece);
                            blackScore += GetPieceSquareValue(piece);
                        }
                    }
                   
                }
            }
            int evaluation = whiteScore - blackScore;

            //int perspective = (board.isWhiteTurn) ? 1 : -1;
            return evaluation;
        }

        private int GetPieceValue(ChessPiece piece)
        {
            if (piece is Pawn)
            {
                return 100;
            }
            else if (piece is Bishop)
            {
                return 310;
            }
            else if (piece is Knight)
            {
                return 300;
            }
            else if (piece is Rook)
            {
                return 500;
            }
            else if (piece is Queen)
            {
                return 900;
            }
            /*else if (piece is King)
            {
                return 20000;
            }*/
            return 0;
            
        }
        private int[,] pawnSquareTable =
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            {  5,  5, 10, 25, 25, 10,  5,  5 },
            {  0,  0,  0, 20, 20,  0,  0,  0 },
            {  5, -5,-10,  0,  0,-10, -5,  5 },
            {  5, 10, 10,-20,-20, 10, 10,  5 },
            {  0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private int GetPieceSquareValue(ChessPiece piece)
        {
            // Piece square tables (you can adjust these values based on your strategy)
            int row = piece.Y;
            int col = piece.X;

            if (piece is Pawn)
            {
                // Use the pawn square table
                if (piece.IsWhite)
                    return pawnSquareTable[row, col];
                else
                    return pawnSquareTable[7-row, col];
            }

            // Add more tables for other piece types if needed

            return 0;
        }
    }
}   