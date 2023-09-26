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

                int score = -Minimax(cloneBoard, depth - 1, int.MinValue, int.MaxValue, true);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(Chessboard board, int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0 || IsGameOver(board))
            {
                return EvaluateBoard(board);
            }

            Stack<ChessMove> legalMoves = GenerateLegalMoves(board);

            if (isMaximizing)
            {
                int maxScore = int.MinValue;
                foreach (ChessMove move in legalMoves)
                {
                    Chessboard cloneBoard = new Chessboard(board);
                    cloneBoard.ApplyMove(move);

                    int score = Minimax(cloneBoard, depth - 1, alpha, beta, false);
                    maxScore = Math.Max(maxScore, score);
                    alpha = Math.Max(alpha, score);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return maxScore;
            }
            else
            {
                int minScore = int.MaxValue;
                foreach (ChessMove move in legalMoves)
                {
                    Chessboard cloneBoard = new Chessboard(board);
                    cloneBoard.ApplyMove(move);

                    int score = Minimax(cloneBoard, depth - 1, alpha, beta, true);
                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, score);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return minScore;
            }
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

        private bool IsGameOver(Chessboard board)
        {
            // Implement logic to check if the game is over (e.g., checkmate or stalemate).
            return false;
        }

        public int EvaluateBoard(Chessboard board)
        {
            int score = 0;

            // Evaluate pieces on the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = board.GetChessPieceAt(row, col);
                    if(piece != null)
                    {
                        if (piece.IsWhite == AIisWhite)
                        {
                            score += GetPieceValue(piece);
                            score += GetPieceSquareValue(piece);
                        }
                        else
                        {
                            score -= GetPieceValue(piece);
                            score -= GetPieceSquareValue(piece);
                        }
                    }
                   
                }
            }
           

            return score;
        }

        private int GetPieceValue(ChessPiece piece)
        {
            if (piece is Pawn)
            {
                return 1;
            }
            else if (piece is Bishop)
            {
                return 3;
            }
            else if (piece is Knight)
            {
                return 3;
            }
            else if (piece is Rook)
            {
                return 5;
            }
            else if (piece is Queen)
            {
                return 9;
            }
            else if (piece is King)
            {
                return 1000;
            }
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
                    return pawnSquareTable[7 - row, col];
            }

            // Add more tables for other piece types if needed

            return 0;
        }
    }
}   