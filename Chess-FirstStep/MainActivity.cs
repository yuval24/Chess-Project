using System.ComponentModel;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Content;
using Android.Graphics;
using static Android.InputMethodServices.Keyboard;
using System;
using Android.Util;
using Java.Security.Cert;
using System.Runtime.Remoting.Contexts;
using Android.Graphics.Drawables;
using Android.Hardware;
using Java.Nio.Channels;
using System.Collections.Generic;
using AndroidX.RecyclerView.Widget;
using System.Text;
using Android.Media;

namespace Chess_FirstStep
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        
        private Chessboard chessboard;
        private ImageView selectedImageView;
        private ChessPiece selectedPiece = null;
        private ChessPiece lastPiecePlayed = null;
        private ImageView[,] chessPieceViews;
        private int selectedRow = -1; // Initialize with an invalid value
        private int selectedCol = -1; // Initialize with an invalid value
        private int targetCol = -1;
        private int targetRow = -1;
        private int countFor50Moves = 0;
        private int piecesOnTheBoard = 32;
        private List<ChessPiece> whiteCapturedPieces = new List<ChessPiece>();
        private List<ChessPiece> blackCapturedPieces = new List<ChessPiece>();
        private List<string> positionHistory = new List<string>();
        private Dictionary<string, int> positionCount = new Dictionary<string, int>();
        private Android.Graphics.Color[,] originalSquareColors = new Android.Graphics.Color[8, 8];
        private const int SomeThreshold = 1000;
        private bool whiteWon = false;
        private bool blackWon = false;
        Dialog d;
        TextView tvWinner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            string[] columnLabels = { "a", "b", "c", "d", "e", "f", "g", "h" };
            chessboard = new Chessboard();
            chessPieceViews = new ImageView[8, 8];
            chessboard.InitializeChessboard(this);


           

            // Initialize chessPieceViews array
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string id = $"{columnLabels[col]}{8 - row}";
                    int resId = Resources.GetIdentifier(id, "id", PackageName);
                    ImageView imageView = FindViewById<ImageView>(resId);
                    chessPieceViews[7 - row, col] = imageView;
                    chessPieceViews[7 - row, col].Clickable = true;
                }
            }

            // Store the original background colors
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    originalSquareColors[row, col] = GetSquareBackgroundColor(row, col);
                }
            }

            // Attach click event handlers to chessPieceViews
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    chessPieceViews[row, col].Click += HandleChessPieceClick;
                }
            }
        }

        private Android.Graphics.Color GetSquareBackgroundColor(int row, int col)
        {
            return ((ColorDrawable)chessPieceViews[row, col].Background).Color;
        }

        private void HandleChessPieceClick(object sender, EventArgs e)
        {
            ImageView clickedImageView = (ImageView)sender;

            // Find the position (row and col) of the clicked ImageView in the array
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chessPieceViews[row, col] == clickedImageView)
                    {
                        if (selectedPiece == null && chessboard.GetChessPieceAt(row, col) != null && (chessboard.GetChessPieceAt(row, col).IsWhite == chessboard.isWhiteTurn))
                        {
                            // Store the selected piece and its position
                            selectedPiece = chessboard.GetChessPieceAt(row, col);
                            selectedImageView = clickedImageView;
                            selectedRow = row;
                            selectedCol = col;
                            chessPieceViews[selectedRow, selectedCol].SetBackgroundColor(Android.Graphics.Color.Yellow);
                        }
                        else
                        {
                            // Store the target square and handle the move
                            targetRow = row;
                            targetCol = col;
                            HandleTargetSquareClick(clickedImageView);
                        }
                    }
                }
            }
        }

        // Handle target square click event
        private void HandleTargetSquareClick(ImageView imageView)
        {
            if (selectedPiece != null && selectedImageView != null)
            {
                // Check if the target square is empty
                if (chessboard.GetChessPieceAt(targetRow, targetCol) == null)
                {
                    if (selectedPiece.Move(targetCol, targetRow, false, chessboard))
                    {
                        // Update the chessboard model
                        chessboard.SetChessPiece(selectedPiece, targetRow, targetCol);
                        chessboard.SetChessPiece(null, selectedRow, selectedCol);

                        // Check if the move puts the player's king in check
                        if (chessboard.IsInCheck())
                        {
                            // Revert the move and show an error message
                            chessboard.SetChessPiece(null, targetRow, targetCol);
                            chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol);
                            Toast.MakeText(this, "Illegal Move", ToastLength.Short).Show();
                        }
                        else
                        {
                            

                            // Store the last piece played
                            lastPiecePlayed = selectedPiece;
                            countFor50Moves++;
                            

                            // Update the UI
                            ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                            targetImageView.SetImageDrawable(selectedImageView.Drawable);

                            // Check for pawn promotion
                            if (selectedPiece is Pawn)
                            {
                                Pawn pawn = new Pawn((Pawn)selectedPiece, selectedCol, selectedRow);
                                if (pawn.validPromtion(targetRow))
                                {
                                    Queen queen = new Queen(selectedCol, selectedRow, selectedPiece.IsWhite);
                                    chessboard.SetChessPiece(queen, targetRow, targetCol);
                                    int queenResource = selectedPiece.IsWhite ? Resource.Drawable.Chess_qlt60 : Resource.Drawable.Chess_qdt60;
                                    targetImageView.SetImageResource(queenResource);
                                }
                            }

                            // Capture the opponent's piece and move to the target square
                            selectedImageView.SetImageDrawable(null);
                            chessboard.SwitchPlayerTurn();
                            drawByRepitition();

                           
                        }
                    }
                    else if (selectedPiece is King && !chessboard.IsInCheck())
                    {
                        King castlePiece = new King((King)selectedPiece, selectedCol, selectedRow);
                        if (castlePiece.castle(targetCol, targetRow, chessboard))
                        {
                            MoveKingAndRookForCastle();
                        }
                    }
                    else if (selectedPiece is Pawn)
                    {
                        Pawn pawn = new Pawn((Pawn)selectedPiece, selectedCol, selectedRow);
                        if (pawn.enPassant(targetCol, targetRow, chessboard))
                        {
                            if (lastPiecePlayed is Pawn)
                            {
                                if (lastPiecePlayed.X == selectedCol - 1 || lastPiecePlayed.X == selectedCol + 1)
                                {
                                    setEnPassant();
                                    Toast.MakeText(this, "en passant", ToastLength.Short).Show();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (selectedPiece.IsWhite != chessboard.GetChessPieceAt(targetRow, targetCol).IsWhite)
                    {
                        if (selectedPiece.Move(targetCol, targetRow, true, chessboard))
                        {
                            // Update the chessboard model
                            ChessPiece chesspiece = chessboard.GetChessPieceAt(targetRow, targetCol);
                            chessboard.SetChessPiece(selectedPiece, targetRow, targetCol);
                            chessboard.SetChessPiece(null, selectedRow, selectedCol);

                            // Check if the move puts the player's king in check
                            if (chessboard.IsInCheck())
                            {
                                // Revert the move and show an error message
                                chessboard.SetChessPiece(chesspiece, targetRow, targetCol);
                                chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol);
                                Toast.MakeText(this, "Illegal Move", ToastLength.Short).Show();
                            }
                            else
                            {
                                
                                // Store the last piece played
                                lastPiecePlayed = selectedPiece;
                                piecesOnTheBoard--;
                                countFor50Moves = 0;

                                // Update the UI
                                ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                                targetImageView.SetImageDrawable(selectedImageView.Drawable);

                                // Check for pawn promotion
                                if (selectedPiece is Pawn)
                                {
                                    Pawn pawn = new Pawn((Pawn)selectedPiece, selectedCol, selectedRow);
                                    if (pawn.validPromtion(targetRow))
                                    {
                                        Queen queen = new Queen(selectedCol, selectedRow, selectedPiece.IsWhite);
                                        chessboard.SetChessPiece(queen, targetRow, targetCol);
                                        int queenResource = selectedPiece.IsWhite ? Resource.Drawable.Chess_qlt60 : Resource.Drawable.Chess_qdt60;
                                        targetImageView.SetImageResource(queenResource);
                                    }
                                }
                                if (piecesOnTheBoard < 5)
                                {
                                    if(insufficientMatrielDraw()){
                                        Toast.MakeText(this, "Draw", ToastLength.Short).Show();
                                    }
                                }

                                selectedImageView.SetImageDrawable(null);
                                chessboard.SwitchPlayerTurn();
                                drawByRepitition();
                            }
                        }
                    }
                }
            }

            // Check for game over conditions
            if (IsCheckmate())
            {
                if (chessboard.IsInCheck())
                {
                    Toast.MakeText(this, "Game Over", ToastLength.Short).Show();
                    if (chessboard.isWhiteTurn)
                    {
                        whiteWon = true;
                    } else
                    {
                        blackWon= true;
                    }
                    createEndGameDialog();
                }
                else
                {
                    Toast.MakeText(this, "Draw", ToastLength.Short).Show();
                }
            }

            if (countFor50Moves == 50)
            {
                Toast.MakeText(this, "Draw", ToastLength.Short).Show();
            }
           
            ResetSelection();
        }

        // Reset the selected piece and related variables
        private void ResetSelection()
        {
            if(selectedRow != -1 && selectedCol != -1)
            {
                chessPieceViews[selectedRow, selectedCol].SetBackgroundColor(originalSquareColors[selectedRow, selectedCol]);
            }
            selectedPiece = null;
            selectedImageView = null;
            selectedRow = -1;
            selectedCol = -1;
            targetRow = -1;
            targetCol = -1;
        }
        // Check if the current player is in checkmate
        public bool IsCheckmate()
        {
            // Iterate through all the player's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessboard.GetChessPieceAt(row, col);

                    // Check if the piece belongs to the player
                    if (piece != null && piece.IsWhite == chessboard.isWhiteTurn)
                    {
                        // Try all possible moves for the piece
                        for (int newRow = 0; newRow < 8; newRow++)
                        {
                            for (int newCol = 0; newCol < 8; newCol++)
                            {
                                if (chessboard.GetChessPieceAt(newRow, newCol) == null)
                                {
                                    Chessboard newChessBoard = CloneChessBoard(chessboard);
                                    ChessPiece currPiece = newChessBoard.GetChessPieceAt(row, col);

                                    if (currPiece.Move(newCol, newRow, false, newChessBoard))
                                    {
                                        newChessBoard.SetChessPiece(currPiece, newRow, newCol);
                                        newChessBoard.SetChessPiece(null, row, col);

                                        // Check if the king is still in check after the move
                                        if (!newChessBoard.IsInCheck())
                                        {
                                            // The player can make a move that gets them out of check
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (piece.IsWhite != chessboard.GetChessPieceAt(newRow, newCol).IsWhite)
                                    {
                                        Chessboard newChessBoard = CloneChessBoard(chessboard);
                                        ChessPiece currPiece = newChessBoard.GetChessPieceAt(row, col);

                                        if (currPiece.Move(newCol, newRow, true, chessboard))
                                        {
                                            newChessBoard.SetChessPiece(currPiece, newRow, newCol);
                                            newChessBoard.SetChessPiece(null, row, col);

                                            // Check if the king is still in check after the move
                                            if (!newChessBoard.IsInCheck())
                                            {
                                                // The player can make a move that gets them out of check
                                                return false;
                                            }
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

        // Move the king and rook for castling
        private void MoveKingAndRookForCastle()
        {
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
            Chessboard newChessboard = new Chessboard(chessboard);
            newChessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol + direction);
            newChessboard.SetChessPiece(null, selectedRow, selectedCol);

            chessboard.SetChessPiece(selectedPiece, targetRow, targetCol);
            chessboard.SetChessPiece(null, selectedRow, selectedCol);

            if (chessboard.IsInCheck() || newChessboard.IsInCheck())
            {
                chessboard.SetChessPiece(null, targetRow, targetCol);
                chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol);
                Toast.MakeText(this, "Illegal Move", ToastLength.Short).Show();
            }
            else
            {
                drawByRepitition();
                if (piecesOnTheBoard < 5)
                {
                    if (insufficientMatrielDraw())
                    {
                        Toast.MakeText(this, "Draw", ToastLength.Short).Show();
                    }
                }
                ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                targetImageView.SetImageDrawable(selectedImageView.Drawable);
                selectedImageView.SetImageDrawable(null);
                chessboard.SwitchPlayerTurn();

                chessboard.SetChessPiece(chessboard.GetChessPieceAt(rookPosition.Item1, rookPosition.Item2), selectedRow, selectedCol + direction);
                chessboard.SetChessPiece(null, rookPosition.Item1, rookPosition.Item2);
                targetImageView = chessPieceViews[selectedRow, selectedCol + direction];
                ImageView rookImageView = chessPieceViews[rookPosition.Item1, rookPosition.Item2];
                targetImageView.SetImageDrawable(rookImageView.Drawable);
                rookImageView.SetImageDrawable(null);
            }
        }

        // Handle en passant
        private void setEnPassant()
        {
            chessboard.SetChessPiece(selectedPiece, targetRow, targetCol);
            chessboard.SetChessPiece(null, selectedRow, selectedCol);
            ChessPiece chesspieceEaten = chessboard.GetChessPieceAt(selectedRow, targetCol);
            chessboard.SetChessPiece(null, selectedRow, targetCol);

            if (chessboard.IsInCheck())
            {
                chessboard.SetChessPiece(null, targetRow, targetCol);
                chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol);
                chessboard.SetChessPiece(chesspieceEaten, selectedRow, targetCol);
                Toast.MakeText(this, "Illegal Move", ToastLength.Short).Show();
            }
            else
            {
                drawByRepitition();
                if (piecesOnTheBoard < 5)
                {
                    if (insufficientMatrielDraw())
                    {
                        Toast.MakeText(this, "Draw", ToastLength.Short).Show();
                    }
                }
                ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                targetImageView.SetImageDrawable(selectedImageView.Drawable);
                selectedImageView.SetImageDrawable(null);

                targetImageView = chessPieceViews[selectedRow, targetCol];
                targetImageView.SetImageDrawable(null);

                chessboard.SwitchPlayerTurn();
            }
        }

        // Clone the chessboard for checking possible moves
        public Chessboard CloneChessBoard(Chessboard other)
        {
            Chessboard newChessBoard = new Chessboard();
            newChessBoard.isWhiteTurn = other.isWhiteTurn;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    newChessBoard.SetChessPiece(other.GetChessPieceAt(row, col), row, col);
                }
            }
            return newChessBoard;
        }

        public void drawByRepitition()
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
                Toast.MakeText(this, "Draw by repetition", ToastLength.Short).Show();
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
       
        // ... (your existing code)


        private string EncodeBoardPosition()
        {
            StringBuilder encodedPosition = new StringBuilder();

            // Loop through the entire board and encode each piece
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessboard.GetChessPieceAt(row, col);

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

        public bool insufficientMatrielDraw()
        {
            int usefullBlackPieces = 0;
            int usefullWhitePieces = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessPiece piece = chessboard.GetChessPieceAt(row, col);

                    if (piece != null && !(piece is King))
                    {
                        if(piece is Bishop || piece is Knight)
                        {
                            if (piece.IsWhite)
                            {
                                usefullWhitePieces++;
                            }
                            else
                            {
                                usefullBlackPieces++;
                            }
                        } else
                        {
                            return false;
                        }
                    }
                }
            }
            if(usefullBlackPieces > 1 || usefullWhitePieces > 1)
            {
                return false;
            }
            return true;
        }
        public void createEndGameDialog()
        {
            d = new Dialog(this);

            d.SetContentView(Resource.Layout.a);

            d.SetTitle("Reset");

            d.SetCancelable(true);

            tvWinner = d.FindViewById<TextView>(Resource.Id.tvWinner);


            d.Show();
            if (whiteWon)
            {
                tvWinner.Text = "White Won!";
            }
            else if(blackWon)
            {
                tvWinner.Text = "Black Won!";
            } else
            {
                tvWinner.Text = "Draw";
            }
        }





    }

    
}