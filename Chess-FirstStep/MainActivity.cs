using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using Android.Graphics.Drawables;
using System.Collections.Generic;


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
                            chessboard.countFor50Moves++;
                            

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
                            if (chessboard.drawByRepitition())
                            {
                                createEndGameDialog();
                            }

                           
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
                                chessboard.piecesOnTheBoard--;
                                chessboard.countFor50Moves = 0;

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
                                if (chessboard.piecesOnTheBoard < 5)
                                {
                                    if(chessboard.insufficientMatrielDraw()){
                                        createEndGameDialog();
                                    }
                                }

                                selectedImageView.SetImageDrawable(null);
                                chessboard.SwitchPlayerTurn();
                                if (chessboard.drawByRepitition())
                                {
                                    createEndGameDialog();
                                }
                            }
                        }
                    }
                }
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
                    } else
                    {
                        whiteWon= true;
                    }
                    
                }
                createEndGameDialog();
            }

            if (chessboard.achieved50Moves())
            {
                createEndGameDialog();
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
            
            chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol + direction);
            chessboard.SetChessPiece(null, selectedRow, selectedCol);

            chessboard.SetChessPiece(selectedPiece, targetRow, targetCol);
            chessboard.SetChessPiece(null, selectedRow, selectedCol);

            if (chessboard.IsInCheck())
            {
                chessboard.SetChessPiece(null, selectedRow, selectedCol + direction);
                chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol);
                chessboard.SetChessPiece(null, targetRow, targetCol);
                chessboard.SetChessPiece(selectedPiece, selectedRow, selectedCol);
                Toast.MakeText(this, "Illegal Move", ToastLength.Short).Show();
            }
            else
            {
                if (chessboard.drawByRepitition())
                {
                    createEndGameDialog();
                }
                if (chessboard.piecesOnTheBoard < 5)
                {
                    if (chessboard.insufficientMatrielDraw())
                    {
                        createEndGameDialog();
                    }
                }
                ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                targetImageView.SetImageDrawable(selectedImageView.Drawable);
                selectedImageView.SetImageDrawable(null);

                chessboard.SetChessPiece(chessboard.GetChessPieceAt(rookPosition.Item1, rookPosition.Item2), selectedRow, selectedCol + direction);
                chessboard.SetChessPiece(null, rookPosition.Item1, rookPosition.Item2);
                targetImageView = chessPieceViews[selectedRow, selectedCol + direction];
                ImageView rookImageView = chessPieceViews[rookPosition.Item1, rookPosition.Item2];
                targetImageView.SetImageDrawable(rookImageView.Drawable);
                rookImageView.SetImageDrawable(null);

                chessboard.SwitchPlayerTurn();
                
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
                if (chessboard.drawByRepitition())
                {
                    createEndGameDialog();
                }
                if (chessboard.piecesOnTheBoard < 5)
                {
                    if (chessboard.insufficientMatrielDraw())
                    {
                        createEndGameDialog();
                    }
                }
                ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                targetImageView.SetImageDrawable(selectedImageView.Drawable);
                selectedImageView.SetImageDrawable(null);

                targetImageView = chessPieceViews[selectedRow, targetCol];
                targetImageView.SetImageDrawable(null);

                chessboard.SwitchPlayerTurn();
                chessboard.piecesOnTheBoard--;
            }
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