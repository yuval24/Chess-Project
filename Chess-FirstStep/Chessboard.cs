using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Hardware.Lights;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Nio.Channels;
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
        private Stack<ChessPiece> capturedChessPiecesWhite;
        private Stack<ChessPiece> capturedChessPiecesBlack;
        public int moveCount {  get; set; }
        public ChessPiece lastPieceCaptured {  get; set; }
        public ChessPiece lastPiecePlayed {  get; set; }

        public Chessboard()
        {
            // Initialize the arrays
            isWhiteTurn = true;
            chessPieces = new ChessPiece[8, 8];
            piecesOnTheBoard = 32;
            countFor50Moves = 0;
            moveCount = 0;
            positionHistory = new List<string>();
            positionCount = new Dictionary<string, int>();
            capturedChessPiecesWhite = new Stack<ChessPiece>();
            capturedChessPiecesBlack = new Stack<ChessPiece>();

        }

        public Chessboard(Chessboard other)
        {
            // Copy the values of all the fields from the other Chessboard object
            this.isWhiteTurn = other.isWhiteTurn;
            this.chessPieces = new ChessPiece[8, 8];

            // Copy the contents of the chessPieces array
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    SetChessPiece(other.chessPieces[row, col], row, col);
                }
            }
           
            this.piecesOnTheBoard = other.piecesOnTheBoard;
            this.countFor50Moves = other.countFor50Moves;
            this.moveCount = other.moveCount;
            this.positionHistory = new List<string>(other.positionHistory);
            capturedChessPiecesWhite = new Stack<ChessPiece>();
            capturedChessPiecesBlack = new Stack<ChessPiece>();
            // Make a deep copy of the positionCount dictionary
            this.positionCount = new Dictionary<string, int>(other.positionCount);
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

        public ChessPiece SetNewChessPiece(ChessPiece piece, int row, int col)
        {


            if (piece != null)
            {

                if (piece.Name == "Pawn")
                {
                    return new Pawn((Pawn)piece, col, row);

                }
                else if (piece.Name == "King")
                {
                    return new King((King)piece, col, row);
                }
                else if (piece.Name == "Queen")
                {
                    return new Queen((Queen)piece, col, row);
                }
                else if (piece.Name == "Rook")
                {
                    return new Rook((Rook)piece, col, row);
                }
                else if (piece.Name == "Knight")
                {
                    return new Knight((Knight)piece, col, row);
                }
                else if (piece.Name == "Bishop")
                {
                    return new Bishop((Bishop)piece, col, row);

                }
            }
            return null;

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
                    ChessPiece piece = chessPieces[row, col];

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
            if (countFor50Moves == 50)
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
                    ChessPiece piece = chessPieces[row, col];

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

        public bool IsMoveValid(ChessMove move)
        {
            int selectedRow = move.StartRow;
            int selectedCol = move.StartCol;
            int targetRow = move.EndRow;
            int targetCol = move.EndCol;

            ChessPiece selectedPiece = chessPieces[selectedRow, selectedCol];
            ChessPiece targetedPlace = chessPieces[targetRow, targetCol];
            if(selectedPiece == null)
            {
                return false;
            }
            if (targetedPlace == null)
            {
                if (selectedPiece.Move(targetCol, targetRow, false, this))
                {
                    // Update the chessboard model
                    SetChessPiece(selectedPiece, targetRow, targetCol);
                    SetChessPiece(null, selectedRow, selectedCol);

                    // Check if the move puts the player's king in check
                    if (IsInCheck())
                    {
                        // Revert the move and show an error message
                        SetChessPiece(null, targetRow, targetCol);
                        SetChessPiece(selectedPiece, selectedRow, selectedCol);
                        move.IsIllegalMove = true;


                        return false;
                    }
                    else
                    {

                        SetChessPiece(null, targetRow, targetCol);
                        SetChessPiece(selectedPiece, selectedRow, selectedCol);



                        // Check for pawn promotion
                        if (selectedPiece is Pawn)
                        {
                            Pawn pawn = new Pawn((Pawn)selectedPiece, selectedRow, selectedCol);
                            if (pawn.validPromtion(targetRow))
                            {

                                move.IsPromotion = true;

                            }
                        }

                        return true;
                    }
                }
                else if (selectedPiece is King && !IsInCheck())
                {
                    King castlePiece = new King((King)selectedPiece, selectedCol, selectedRow);
                    if (castlePiece.castle(targetCol, targetRow, this))
                    {
                        if (castleIsValid(move)) {

                            if (Math.Abs(targetCol - selectedCol) == 2)
                            {
                                move.IsKingsideCastle = true;
                            }
                            else
                            {
                                move.IsQueensideCastle = true;
                            }
                            return true;
                        }

                    }
                }
                else if (selectedPiece is Pawn)
                {
                    Pawn pawn = new Pawn((Pawn)selectedPiece, selectedCol, selectedRow);
                    if (pawn.enPassant(targetCol, targetRow, this))
                    {
                        if (lastPiecePlayed is Pawn)
                        {
                            if (lastPiecePlayed.X == targetCol)
                            {
                                if (EnPassantisValid(move))
                                {
                                    //move.IsCapture = true;
                                    move.IsEnPassantCapture = true;
                                    return true;
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                if (selectedPiece.IsWhite != targetedPlace.IsWhite)
                {
                    if (selectedPiece.Move(targetCol, targetRow, true, this))
                    {
                        // Update the chessboard model
                        ChessPiece chesspiece = chessPieces[targetRow, targetCol];
                        SetChessPiece(selectedPiece, targetRow, targetCol);
                        SetChessPiece(null, selectedRow, selectedCol);

                        // Check if the move puts the player's king in check
                        if (IsInCheck())
                        {
                            // Revert the move and show an error message
                            SetChessPiece(chesspiece, targetRow, targetCol);
                            SetChessPiece(selectedPiece, selectedRow, selectedCol);
                            move.IsIllegalMove = true;
                            return false;
                        }
                        else
                        {
                            
                            SetChessPiece(chesspiece, targetRow, targetCol);
                            SetChessPiece(selectedPiece, selectedRow, selectedCol);
                            move.IsCapture = true;

                            // Check for pawn promotion
                            if (selectedPiece is Pawn)
                            {
                                Pawn pawn = new Pawn((Pawn)selectedPiece, selectedCol, selectedRow);
                                if (pawn.validPromtion(targetRow))
                                {
                                    move.IsPromotion = true;


                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void ApplyMove(ChessMove move)
        {
            int selectedRow = move.StartRow;
            int selectedCol = move.StartCol;
            int targetRow = move.EndRow;
            int targetCol = move.EndCol;
            ChessPiece selectedPiece = chessPieces[selectedRow, selectedCol];
            ChessPiece targetedPlace = chessPieces[targetRow, targetCol];
            chessPieces[selectedRow, selectedCol].HasMoved = true;
            moveCount++;
            if (move.IsEnPassantCapture)
            {
                //(!isWhiteTurn ? capturedChessPiecesWhite : capturedChessPiecesBlack).Push(chessPieces[selectedRow, selectedCol]);
                setEnPassant(move);

            }
            else if (move.IsKingsideCastle || move.IsQueensideCastle)
            {


                MoveKingAndRookForCastle(move);

            }
            else if (move.IsPromotion)
            {
                chessPieces[targetRow, targetCol] = new Queen(targetRow, targetCol, selectedPiece.IsWhite);
                SetChessPiece(null,selectedRow, selectedCol);

                countFor50Moves = 0;

            }
            else if (move.IsCapture)
            {
                // Store the last piece played
                lastPieceCaptured = chessPieces[targetRow, targetCol];

                SetChessPiece(selectedPiece, targetRow, targetCol);
                SetChessPiece(null, selectedRow, selectedCol);


                
                piecesOnTheBoard--;
                countFor50Moves = 0;
            }
            else
            {
                SetChessPiece(selectedPiece, targetRow, targetCol);
                SetChessPiece(null, selectedRow, selectedCol);

                if (!(selectedPiece is Pawn))
                {
                    countFor50Moves++;
                }
            }
            lastPiecePlayed = selectedPiece;
            
        }
        // Move the king and rook for castling
        private bool castleIsValid(ChessMove move)
        {
            int selectedRow = move.StartRow;
            int selectedCol = move.StartCol;
            int targetRow = move.EndRow;
            int targetCol = move.EndCol;
            ChessPiece selectedPiece = chessPieces[selectedRow, selectedCol];
            // Determine the direction of castling (left or right)
            Tuple<int, int> rookPosition;
            if (selectedPiece.IsWhite)
            {
                if (selectedCol > targetCol)
                {
                    rookPosition = Tuple.Create(0, 0);
                }
                else
                {
                    rookPosition = Tuple.Create(0, 7);
                }
            }
            else
            {
                if (selectedCol > targetCol)
                {
                    rookPosition = Tuple.Create(7, 0);
                }
                else
                {
                    rookPosition = Tuple.Create(7, 7);
                }
            }

            int direction = rookPosition.Item2 < selectedCol ? -1 : 1;

            // Move the rook to the square the king crossed


            SetChessPiece(selectedPiece, selectedRow, selectedCol + direction);
            SetChessPiece(null, selectedRow, selectedCol);


            if (IsInCheck())
            {
                SetChessPiece(null, selectedRow, selectedCol + direction);
                SetChessPiece(selectedPiece, selectedRow, selectedCol);

                move.IsIllegalMove = true;
                return false;
            }

            SetChessPiece(null, selectedRow, selectedCol + direction);
            SetChessPiece(selectedPiece, targetRow, targetCol);

            if (IsInCheck())
            {
                SetChessPiece(null, targetRow, targetCol);
                SetChessPiece(selectedPiece, selectedRow, selectedCol);

                move.IsIllegalMove = true;
                return false;
            }


            SetChessPiece(selectedPiece, selectedRow, selectedCol);
            SetChessPiece(null, targetRow, targetCol);


            return true;
        }

        private bool MoveKingAndRookForCastle(ChessMove move)
        {
            int selectedRow = move.StartRow;
            int selectedCol = move.StartCol;
            int targetRow = move.EndRow;
            int targetCol = move.EndCol;
            ChessPiece selectedPiece = chessPieces[selectedRow, selectedCol];
            // Determine the direction of castling (left or right)
            Tuple<int, int> rookPosition;
            if (selectedPiece.IsWhite)
            {
                if (selectedCol > targetCol)
                {
                    rookPosition = Tuple.Create(0, 0);
                }
                else
                {
                    rookPosition = Tuple.Create(0, 7);
                }
            }
            else
            {
                if (selectedCol > targetCol)
                {
                    rookPosition = Tuple.Create(7, 0);
                }
                else
                {
                    rookPosition = Tuple.Create(7, 7);
                }
            }

            int direction = rookPosition.Item2 < selectedCol ? -1 : 1;

            // Move the rook to the square the king crossed


            SetChessPiece(selectedPiece, targetRow, targetCol);
            SetChessPiece(null, selectedRow, selectedCol);
            SetChessPiece(chessPieces[rookPosition.Item1, rookPosition.Item2], selectedRow, selectedCol + direction);
            SetChessPiece(null, rookPosition.Item1, rookPosition.Item2);

            countFor50Moves++;

            return true;
        }

        // Handle en passant
        private bool EnPassantisValid(ChessMove move)
        {
            int selectedRow = move.StartRow;
            int selectedCol = move.StartCol;
            int targetRow = move.EndRow;
            int targetCol = move.EndCol;
            ChessPiece selectedPiece = chessPieces[selectedRow, selectedCol];

            SetChessPiece(selectedPiece, targetRow, targetCol);
            SetChessPiece(null, selectedRow, selectedCol);
            ChessPiece chesspieceEaten = chessPieces[selectedRow, targetCol];
            SetChessPiece(null, selectedRow, targetCol);

            if (IsInCheck())
            {
                SetChessPiece(null, targetRow, targetCol);
                SetChessPiece(selectedPiece, selectedRow, selectedCol);
                SetChessPiece(chesspieceEaten, selectedRow, targetCol);

                move.IsIllegalMove = true;
                return false;

            }

            SetChessPiece(null, targetRow, targetCol);
            SetChessPiece(selectedPiece, selectedRow, selectedCol);
            SetChessPiece(chesspieceEaten, selectedRow, targetCol);


            return true;
        }

        private void setEnPassant(ChessMove move)
        {
            int selectedRow = move.StartRow;
            int selectedCol = move.StartCol;
            int targetRow = move.EndRow;
            int targetCol = move.EndCol;
            ChessPiece selectedPiece = chessPieces[selectedRow, selectedCol];
            SetChessPiece(selectedPiece, targetRow, targetCol);
            SetChessPiece(null, selectedRow, selectedCol);
            SetChessPiece(null, selectedRow, targetCol);



            piecesOnTheBoard--;
            countFor50Moves = 0;

        }

        public int convertChessPieceToImage(ChessPiece piece)
        {
            if (piece is Pawn)
            {
                return piece.IsWhite ? Resource.Drawable.Chess_plt60 : Resource.Drawable.Chess_pdt60;
            }
            else if (piece is Rook)
            {
                return piece.IsWhite ? Resource.Drawable.Chess_rlt60 : Resource.Drawable.Chess_rdt60;
            }
            else if (piece is Knight)
            {
                return piece.IsWhite ? Resource.Drawable.Chess_nlt60 : Resource.Drawable.Chess_ndt60;
            }
            else if (piece is Bishop)
            {
                return piece.IsWhite ? Resource.Drawable.Chess_blt60 : Resource.Drawable.Chess_bdt60;
            }
            else if (piece is Queen)
            {
                return piece.IsWhite ? Resource.Drawable.Chess_qlt60 : Resource.Drawable.Chess_qdt60;
            }
            else if (piece is King)
            {
                return piece.IsWhite ? Resource.Drawable.Chess_klt60 : Resource.Drawable.Chess_kdt60;
            }
            return -1;
        }

        /*public void UndoMove(ChessMove move)
        {
            if (move == null) return;



            if (move.IsPromotion)
            {
                Pawn pawn = new Pawn(move.StartRow, move.StartCol, isWhiteTurn);
                SetChessPiece(pawn, move.StartRow, move.StartCol);


            }

            if (move.IsKingsideCastle || move.IsQueensideCastle)
            {
                
            }
            else if (move.IsCapture)
            {
                
                ChessPiece capturedPiece = lastPieceCaptured;
                if (move.IsPromotion)
                {
                    SetChessPiece(capturedPiece, capturedPiece.Y, capturedPiece.X);
                }
                else
                {
                    SetChessPiece(this.chessPieces[move.EndRow, move.EndCol], move.StartRow, move.StartCol);
                    SetChessPiece(capturedPiece, capturedPiece.Y, capturedPiece.X);
                }
             
            }
            else
            {
                SetChessPiece(this.chessPieces[move.EndRow, move.EndCol], move.StartRow, move.StartCol);
                SetChessPiece(null, move.EndRow, move.EndCol);
            }
            //SwitchPlayerTurn();
        }*/
    }

   



   
}

//move here the checking if a move is valid or not 
