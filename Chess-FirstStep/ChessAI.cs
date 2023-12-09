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

        public ChessMove GetBestMove(Chessboard board, int depth)
        {
            Stack<ChessMove> legalMoves = GenerateLegalMoves(board);
            ChessMove bestMove = null;
            int bestScore = int.MinValue;

            foreach (ChessMove move in legalMoves)
            {
                Chessboard cloneBoard = new Chessboard(board); // Create a copy of the board to simulate moves
                
                cloneBoard.ApplyMove(move);
                


                int score = -Minimax(cloneBoard, depth - 1, int.MinValue, int.MaxValue);

                
                

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(Chessboard board, int depth, int alpha, int beta)
        {
            if (depth == 0)
            {
                return EvaluateBoard(board);
            }


            board.SwitchPlayerTurn();
            AIisWhite = !AIisWhite;
            Stack<ChessMove> legalMoves = GenerateLegalMoves(board);
            

            if(legalMoves.Count == 0)
            {
                if (board.IsInCheck())
                {
                    return int.MinValue;
                }
                return 0;
            }
            int bestScore = int.MinValue;
            foreach (ChessMove move in legalMoves)
            {
                Chessboard cloneBoard = new Chessboard(board);
                cloneBoard.ApplyMove(move);
                
                

                int score = -Minimax(cloneBoard, depth - 1, -beta, -alpha);


                if(score >= beta) 
                {
                    return score; 
                }

                alpha = Math.Max(alpha, score);

                
            }

            return alpha;
        
        }

        private Stack<ChessMove> GenerateLegalMoves(Chessboard board)
        {
            // Implement logic to generate a list of legal moves based on the current board state.
            // This involves considering the positions and possible moves of all pieces.
            // Return a list of ChessMove objects representing legal moves.
            Stack<ChessMove> legalMoves = new Stack<ChessMove>();

            // Iterate through all the player's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = board.GetChessPieceAt(row, col);

                    // Check if the piece belongs to the player
                    if (piece != null && piece.IsWhite == AIisWhite)
                    {
                        // Try all possible moves for the piece
                        for (int newRow = 0; newRow < 8; newRow++)
                        {
                            for (int newCol = 0; newCol < 8; newCol++)
                            {
                                ChessMove move = new ChessMove(row, col, newRow, newCol);

                                if (board.IsMoveValid(move))
                                {
                                    legalMoves.Push(move);
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
            int evaluation = whiteScore -blackScore;

            int perspective = (board.isWhiteTurn) ? 1 : -1;
            return evaluation * perspective;
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