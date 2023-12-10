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
        private int searchDepth;
        private static readonly int[,] PawnSquareTable =
        {
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 50, 50, 50, 50, 50, 50, 50, 50 },
        { 10, 10, 20, 30, 30, 20, 10, 10 },
        { 5,  5, 10, 25, 25, 10,  5,  5 },
        { 0,  0,  0, 20, 20,  0,  0,  0 },
        { 5, -5, -10, 0, 0, -10, -5, 5 },
        { 5, 10, 10, -20, -20, 10, 10, 5 },
        { 0,  0,  0,  0,  0,  0,  0,  0 }};

        private static readonly int[,] KnightSquareTable =
        {
        { -50, -40, -30, -30, -30, -30, -40, -50 },
        { -40, -20,  0,  0,  0,  0, -20, -40 },
        { -30,  0, 10, 15, 15, 10,  0, -30 },
        { -30,  5, 15, 20, 20, 15,  5, -30 },
        { -30,  0, 15, 20, 20, 15,  0, -30 },
        { -30,  5, 10, 15, 15, 10,  5, -30 },
        { -40, -20,  0,  5,  5,  0, -20, -40 },
        { -50, -40, -30, -30, -30, -30, -40, -50 }};

        private static readonly int[,] BishopSquareTable =
        {
        { -20, -10, -10, -10, -10, -10, -10, -20 },
        { -10,  0,  0,  0,  0,  0,  0, -10 },
        { -10,  0,  5, 10, 10,  5,  0, -10 },
        { -10,  5,  5, 10, 10,  5,  5, -10 },
        { -10,  0, 10, 10, 10, 10,  0, -10 },
        { -10, 10, 10, 10, 10, 10, 10, -10 },
        { -10,  5,  0,  0,  0,  0,  5, -10 },
        { -20, -10, -10, -10, -10, -10, -10, -20 }};

        private static readonly int[,] RookSquareTable =
        {
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 5, 10, 10, 10, 10, 10, 10, 5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { 0, 0, 0, 5, 5, 0, 0, 0 }};

        private static readonly int[,] QueenSquareTable =
        {
        { -20, -10, -10, -5, -5, -10, -10, -20 },
        { -10,  0,  0,  0,  0,  0,  0, -10 },
        { -10,  0,  5,  5,  5,  5,  0, -10 },
        { -5,  0,  5,  5,  5,  5,  0, -5 },
        { 0,  0,  5,  5,  5,  5,  0, -5 },
        { -10,  5,  5,  5,  5,  5,  0, -10 },
        { -10,  0,  5,  0,  0,  0,  0, -10 },
        { -20, -10, -10, -5, -5, -10, -10, -20 }};

        private static readonly int[,] KingSquareTable =
        {
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -20, -30, -30, -40, -40, -30, -30, -20 },
        { -10, -20, -20, -20, -20, -20, -20, -10 },
        { 20, 20, 0, 0, 0, 0, 20, 20 },
        { 20, 30, 10, 0, 0, 10, 30, 20 }};
        public ChessAI(bool AIisWhite, int searchDepth)
        {
            this.AIisWhite = AIisWhite;
            this.searchDepth = searchDepth;
        }

        public ChessMove GetBestMove(Chessboard board)
        {
            List<ChessMove> moves = GenerateLegalMoves(board);
            moves.Sort((move1, move2) => MoveValue(move1, board).CompareTo(MoveValue(move2, board)));

            ChessMove bestMove = null;
            int maxScore = int.MinValue;

            foreach(ChessMove move in moves)
            {
                Chessboard cloneBoard = new Chessboard(board);   
                cloneBoard.ApplyMove(move);

                int eval = MiniMax(cloneBoard, searchDepth, int.MinValue, int.MaxValue, false);

                if( eval >= maxScore)
                {
                    maxScore = Math.Max(maxScore, eval);
                    bestMove = move;
                }
                

            }
            return bestMove;
        }

        private int MiniMax(Chessboard board, int searchDepth, int alpha, int beta, bool maximizingPlayer)
        {
            if(searchDepth == 0)
            {
                return EvaluateBoard(board);
            }

            board.SwitchPlayerTurn();
            List<ChessMove> moves = GenerateLegalMoves(board);
            moves.Sort((move1, move2) => MoveValue(move1, board).CompareTo(MoveValue(move2, board)));


            if (moves.Count == 0)
            {
                if (board.IsCheckmate())
                {
                    return int.MinValue;
                }
                return 0;
            }
            if(maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (ChessMove move in moves)
                {
                    Chessboard cloneboard = new Chessboard(board);
                    cloneboard.ApplyMove(move);

                    
                    int eval = MiniMax(cloneboard, searchDepth - 1, alpha, beta, false);
                    
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if(beta <= alpha)
                    {
                        break;
                    }
                }
                return maxEval;
            } else
            {
                int minEval = int.MaxValue;
                foreach (ChessMove move in moves)
                {
                    Chessboard cloneboard = new Chessboard(board);
                    cloneboard.ApplyMove(move);

                    int eval = MiniMax(cloneboard, searchDepth - 1, alpha, beta, true);

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return minEval;
            }
           
        }

        private int MoveValue(ChessMove move, Chessboard board)
        {
            int moveScore = 0;
            ChessPiece currPiece = board.GetChessPieceAt(move.StartRow, move.StartCol);
            ChessPiece capturedPiece = board.GetChessPieceAt(move.EndRow, move.EndCol);

            if (move.IsCapture)
            {
                moveScore = 10 * GetPieceValue(capturedPiece) - GetPieceValue(currPiece); // Fix here
            }

            if (move.IsPromotion)
            {
                moveScore += 900; // the promotion is always a queen which equals 900
            }

            return moveScore;
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

            int perspective = (AIisWhite) ? 1 : -1;
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
        

        private int GetPieceSquareValue(ChessPiece piece)
        {
            // Piece square tables (you can adjust these values based on your strategy)
            int row = piece.Y;
            int col = piece.X;

            switch(piece)
            {
                case Pawn _:
                    return (!piece.IsWhite ? PawnSquareTable[row, col] : PawnSquareTable[7 - row, col]);
                case Bishop _:
                    return (!piece.IsWhite ? BishopSquareTable[row, col] : BishopSquareTable[7 - row, col]);
                case Knight _:
                    return (!piece.IsWhite ? KnightSquareTable[row, col] : KnightSquareTable[7 - row, col]);
                case Rook _:
                    return (!piece.IsWhite ? RookSquareTable[row, col] : RookSquareTable[7 - row, col]);
                case Queen _:
                    return (!piece.IsWhite ? QueenSquareTable[row, col] : QueenSquareTable[7 - row, col]);
                case King _:
                    return (!piece.IsWhite ? KingSquareTable[row, col] : KingSquareTable[7 - row, col]);
                default:
                    return 0;

            }
               
            // Add more tables for other piece types if needed

           
        }
    }
}   