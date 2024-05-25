using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Security.Cert;
using static Android.InputMethodServices.Keyboard;

namespace Chess_FirstStep
{
    [Activity(Label = "GameMovesActivity")]
    public class GameMovesActivity : Activity
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
        private int currentMoveIndex = 0;
        List<string> moves;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_moves_activity_layout);

            // Initialize your chessboard and views
            InitializeChessboard();


            // Initialize the UI elements and attach click event handlers
            InitializeUI();

            IList<string> movesList = Intent.GetStringArrayListExtra("GameMoves");

            // Explicitly convert IList<string> to List<string>
            moves = new List<string>(movesList);
            ImageButton leftArrowImageButton = FindViewById<ImageButton>(Resource.Id.leftArrowImageButton);
            ImageButton rightArrowImageButton = FindViewById<ImageButton>(Resource.Id.rightArrowImageButton);
            ImageButton btnExitGameMoves = FindViewById<ImageButton>(Resource.Id.btnExitGameMoves);
            

            leftArrowImageButton.Click += LeftArrowImageButton_Click;
            rightArrowImageButton.Click += RightArrowImageButton_Click;
            btnExitGameMoves.Click += BtnExitGameMoves_Click;
        }

        private void BtnExitGameMoves_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void RightArrowImageButton_Click(object sender, EventArgs e)
        {
            // Ensure there are moves available in the list
            if (currentMoveIndex < moves.Count)
            {
                // Apply the next move from the moves list


                updateUIRightArrow(NetworkManager.ConvertStringToMove(moves[currentMoveIndex]));
                // Increment the current move index
                currentMoveIndex++;

              
            }
        }

        private void LeftArrowImageButton_Click(object sender, EventArgs e)
        {
           if(currentMoveIndex > 0)
           {
                currentMoveIndex--;
                updateUILeftArrow(NetworkManager.ConvertStringToMove(moves[currentMoveIndex]));
                // Increment the current move index
                

            }
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

        private void updateUIRightArrow(ChessMove chessMove)
        {
            selectedRow = chessMove.StartRow;
            selectedCol = chessMove.StartCol;
            targetRow = chessMove.EndRow;
            targetCol = chessMove.EndCol;
            selectedPiece = chessboard.GetChessPieceAt(selectedRow, selectedCol);
            selectedImageView = chessPieceViews[selectedRow, selectedCol];
            if (selectedPiece != null)
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
                else if (chessMove.IsPromotion)
                {
                    ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                    if (chessboard.isWhiteTurn)
                    {
                        targetImageView.SetImageResource(Resource.Drawable.Chess_qlt60);
                    }
                    else
                    {
                        targetImageView.SetImageResource(Resource.Drawable.Chess_qdt60);
                    }

                    selectedImageView.SetImageDrawable(null);
                }
                else if (chessMove.IsCapture)
                {

                    ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                    targetImageView.SetImageDrawable(selectedImageView.Drawable);
                    selectedImageView.SetImageDrawable(null);
                }
                else
                {
                    // Update the UI
                    ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                    targetImageView.SetImageDrawable(selectedImageView.Drawable);
                    selectedImageView.SetImageDrawable(null);


                }



                chessboard.SwitchPlayerTurn();

            }



            ResetSelection();
        }


        private void updateUILeftArrow(ChessMove chessMove)
        {
            selectedRow = chessMove.StartRow;
            selectedCol = chessMove.StartCol;
            targetRow = chessMove.EndRow;
            targetCol = chessMove.EndCol;
            ImageView targetImageView = chessPieceViews[targetRow, targetCol];
            selectedPiece = chessboard.GetChessPieceAt(selectedRow, selectedCol);
            selectedImageView = chessPieceViews[selectedRow, selectedCol];
            if (targetImageView != null)
            {
                chessboard.UndoMove(chessMove);
                ChessPiece targetPiece = chessboard.GetChessPieceAt(targetRow, targetCol);
                

                if (chessMove.IsEnPassantCapture)
                {
                    
                    targetImageView = chessPieceViews[targetRow, targetCol];
                    selectedImageView.SetImageDrawable(targetImageView.Drawable);
                    targetImageView.SetImageDrawable(null);

                    targetImageView = chessPieceViews[selectedRow, targetCol];
                    ChessPiece chessPiece = chessboard.GetChessPieceAt(selectedRow, targetCol);
                    targetImageView.SetImageResource(chessboard.convertChessPieceToImage(chessPiece));
                }
                else if (chessMove.IsKingsideCastle || chessMove.IsQueensideCastle)
                {
                    updateViewBoard();
                }
                else if (chessMove.IsPromotion)
                {
                    selectedPiece = chessboard.GetChessPieceAt(selectedRow, selectedCol);
                    targetImageView = chessPieceViews[targetRow, targetCol];
                    selectedImageView.SetImageResource(chessboard.convertChessPieceToImage(selectedPiece));
                    targetImageView.SetImageResource(chessboard.convertChessPieceToImage(targetPiece));
                }
                else if (chessMove.IsCapture)
                {

                    targetImageView = chessPieceViews[targetRow, targetCol];

                    selectedImageView.SetImageDrawable(targetImageView.Drawable);
                    targetImageView.SetImageResource(chessboard.convertChessPieceToImage(targetPiece));

                }
                else
                {
                    // Update the UI
                    
                    selectedImageView.SetImageDrawable(targetImageView.Drawable);
                    targetImageView.SetImageDrawable(null);


                }



                chessboard.SwitchPlayerTurn();


            }



            ResetSelection();
        }

        private void updateViewBoard()
        {
            for (int i = 0; i < 8; i++) {

                for(int j = 0; j < 8; j++)
                {
                    chessPieceViews[i, j].SetImageResource(chessboard.convertChessPieceToImage(chessboard.GetChessPieceAt(i,j)));
                }
            }

        }
        private void ResetSelection()
        {
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

        // Initialize the UI elements and attach click event handlers
        private void InitializeUI()
        {

            /*ImageButton btnExit = FindViewById<ImageButton>(Resource.Id.btnExitTwoPlayer);
            btnExit.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(MainPageActivity));
                StartActivity(intent);
                Finish();
            };*/

            // Find the TableLayout in your XML layout
            TableLayout chessboardLayout = FindViewById<TableLayout>(Resource.Id.chessboardLayoutGameMoves);

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
    }
}