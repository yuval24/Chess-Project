using System;
using System.Collections.Generic;

using Android.App;
using Android.Bluetooth;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Chess_FirstStep
{
    [Activity(Label = "AIGameActivity")]
    public class AIGameActivity : AppCompatActivity
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
        private List<ChessPiece> whiteCapturedPieces = new List<ChessPiece>();
        private List<ChessPiece> blackCapturedPieces = new List<ChessPiece>();
        private Android.Graphics.Color[,] originalSquareColors = new Android.Graphics.Color[8, 8];
        private bool whiteWon = false;
        private bool blackWon = false;
        Dialog d;
        TextView tvWinner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_two_player_game);

            // Initialize your chessboard and views
            InitializeChessboard();

            // Initialize the UI elements and attach click event handlers
            InitializeUI();
        }

        // Initialize the chessboard and views
        private void InitializeChessboard()
        {
            // Initialize your chessboard
            string[] columnLabels = { "a", "b", "c", "d", "e", "f", "g", "h" };
            chessboard = new Chessboard();
            chessPieceViews = new ImageView[8, 8];
            chessboard.InitializeChessboard(this);
        }

        // Initialize the UI elements and attach click event handlers
        private void InitializeUI()
        {
            // Find the TableLayout in your XML layout
            TableLayout chessboardLayout = FindViewById<TableLayout>(Resource.Id.chessboardLayout);

            // Define the number of rows and columns on the chessboard
            int numRows = 8;
            int numCols = 8;

            // Create ImageView elements and add them to the TableLayout
            for (int row = 0; row < numRows; row++)
            {
                TableRow tableRow = new TableRow(this);
                for (int col = 0; col < numCols; col++)
                {
                    ImageView imageView = new ImageView(this);

                    // Set the attributes of the ImageView (e.g., background color, size)
                    if ((row + col) % 2 == 0)
                    {
                        imageView.SetBackgroundColor(Android.Graphics.Color.White);
                    }
                    else
                    {
                        imageView.SetBackgroundColor(Android.Graphics.Color.Gray);
                    }

                    imageView.LayoutParameters = new TableRow.LayoutParams(
                        135,
                        135);

                    // Add an ID or tag to identify the square (e.g., a1, a2, b1, b2, etc.)
                    imageView.Tag = $"{(char)('a' + col)}{(8 - row)}";

                    // Add a click listener to handle square clicks
                    chessPieceViews[7 - row, col] = imageView;
                    chessPieceViews[7 - row, col].Clickable = true;
                    tableRow.AddView(imageView);
                }
                chessboardLayout.AddView(tableRow);
            }

            // Initialize the chess pieces on the chessboard views
            InitializeChessboardViews();

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

        // Initialize the chess pieces on the chessboard views
        public void InitializeChessboardViews()
        {
            // Set Rooks
            chessPieceViews[0, 0].SetImageResource(Resource.Drawable.Chess_rlt60);
            chessPieceViews[0, 7].SetImageResource(Resource.Drawable.Chess_rlt60);
            chessPieceViews[7, 0].SetImageResource(Resource.Drawable.Chess_rdt60);
            chessPieceViews[7, 7].SetImageResource(Resource.Drawable.Chess_rdt60);

            // Set Knights
            chessPieceViews[0, 1].SetImageResource(Resource.Drawable.Chess_nlt60);
            chessPieceViews[0, 6].SetImageResource(Resource.Drawable.Chess_nlt60);
            chessPieceViews[7, 1].SetImageResource(Resource.Drawable.Chess_ndt60);
            chessPieceViews[7, 6].SetImageResource(Resource.Drawable.Chess_ndt60);

            // Set Bishops
            chessPieceViews[0, 2].SetImageResource(Resource.Drawable.Chess_blt60);
            chessPieceViews[0, 5].SetImageResource(Resource.Drawable.Chess_blt60);
            chessPieceViews[7, 2].SetImageResource(Resource.Drawable.Chess_bdt60);
            chessPieceViews[7, 5].SetImageResource(Resource.Drawable.Chess_bdt60);

            // Set Queens
            chessPieceViews[0, 3].SetImageResource(Resource.Drawable.Chess_qlt60);
            chessPieceViews[7, 3].SetImageResource(Resource.Drawable.Chess_qdt60);

            // Set Kings
            chessPieceViews[0, 4].SetImageResource(Resource.Drawable.Chess_klt60);
            chessPieceViews[7, 4].SetImageResource(Resource.Drawable.Chess_kdt60);

            // Set Pawns
            for (int i = 0; i < 8; i++)
            {
                chessPieceViews[1, i].SetImageResource(Resource.Drawable.Chess_plt60);
                chessPieceViews[6, i].SetImageResource(Resource.Drawable.Chess_pdt60);
            }
        }

        // Get the background color of a square
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



        private void HandleTargetSquareClick(ImageView imageView)
        {
            if (selectedPiece != null)
            {
                ChessMove chessMove = new ChessMove(selectedRow, selectedCol, targetRow, targetCol);
                if (chessboard.IsMoveValid(chessMove))
                {
                    chessboard.ApplyMove(chessMove);
                    if (chessMove.IsEnPassantCapture)
                    {

                        ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                        targetImageView.SetImageDrawable(selectedImageView.Drawable);
                        selectedImageView.SetImageDrawable(null);

                        targetImageView = chessPieceViews[selectedRow, targetCol];
                        targetImageView.SetImageDrawable(null);
                    }
                    else if (chessMove.IsKingsideCastle || chessMove.IsQueensideCastle)
                    {
                        MoveKingAndRookForCastle();
                    }
                    else if (chessMove.IsCapture)
                    {
                        // Update the UI
                        ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                        targetImageView.SetImageDrawable(selectedImageView.Drawable);
                        selectedImageView.SetImageDrawable(null);

                        if (chessboard.piecesOnTheBoard < 5)
                        {
                            if (chessboard.insufficientMatrielDraw())
                            {
                                createEndGameDialog();
                            }
                        }
                    }
                    else
                    {
                        // Update the UI
                        ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                        targetImageView.SetImageDrawable(selectedImageView.Drawable);
                        selectedImageView.SetImageDrawable(null);


                        if (chessboard.achieved50Moves())
                        {
                            createEndGameDialog();
                        }
                    }

                   
                    if (chessboard.drawByRepitition())
                    {
                        createEndGameDialog();
                    }

                    chessboard.SwitchPlayerTurn();
                }
                else if (chessMove.IsIllegalMove)
                {
                    Toast.MakeText(this, "You will lose your king moron", ToastLength.Short).Show();
                }

                // Check for game over conditions
                if (chessboard.IsCheckmate())
                {
                    if (chessboard.IsInCheck())
                    {
                        Toast.MakeText(this, "Game Over", ToastLength.Short).Show();
                        if (chessboard.isWhiteTurn)
                        {
                            blackWon = true;
                        }
                        else
                        {
                            whiteWon = true;
                        }

                    }
                    createEndGameDialog();
                }

            }



            ResetSelection();
        }
        // Reset the selected piece and related variables
        private void ResetSelection()
        {
            if (selectedRow != -1 && selectedCol != -1)
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

            ImageView targetImageView = chessPieceViews[targetRow, targetCol];
            targetImageView.SetImageDrawable(selectedImageView.Drawable);
            selectedImageView.SetImageDrawable(null);


            targetImageView = chessPieceViews[selectedRow, selectedCol + direction];
            ImageView rookImageView = chessPieceViews[rookPosition.Item1, rookPosition.Item2];
            targetImageView.SetImageDrawable(rookImageView.Drawable);
            rookImageView.SetImageDrawable(null);
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
            else if (blackWon)
            {
                tvWinner.Text = "Black Won!";
            }
            else
            {
                tvWinner.Text = "Draw";
            }
        }
    }
}