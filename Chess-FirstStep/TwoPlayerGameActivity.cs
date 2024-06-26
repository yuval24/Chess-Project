﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Graphics.Drawables;
using Android.Content;
using Android.Media;
using System.Timers;
using AndroidX.RecyclerView.Widget;
using System.Threading;

namespace Chess_FirstStep
{
    [Activity(Label = "TwoPlayerGameActivity")]
    public class TwoPlayerGameActivity : AppCompatActivity
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


        private System.Timers.Timer whiteTimer;
        private System.Timers.Timer blackTimer;
        private TimeSpan blackTime;
        private TimeSpan whiteTime;
        private TimeSpan defaultTime = TimeSpan.FromMinutes(5); // Default time for each player
        private TextView blackPlayerTimer;
        private TextView whitePlayerTimer;
        private List<int> whiteCapturedPieceIds;
        private List<int> blackCapturedPieceIds;
        private CapturedPiecesAdapter whiteAdapter;
        private CapturedPiecesAdapter blackAdapter;
        private bool gameResigned = false;
        private SoundPool soundPool;
        private int moveSoundId;
        private int checkSoundId;
        private int castleSoundId;
        private int captureSoundId;
        private int promotionSoundId;
        private int tensecondsSoundId;
        private int gameEndSoundId;
        private int gameStartSoundId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_two_player_game);

            // Initialize your chessboard and views
            InitializeChessboard();


            // Initialize the UI elements and attach click event handlers
            InitializeUI();

            // Initialize lists to hold captured piece IDs
            whiteCapturedPieceIds = new List<int>();
            blackCapturedPieceIds = new List<int>();

            // Create adapters for the ListView
            whiteAdapter = new CapturedPiecesAdapter(this, whiteCapturedPieceIds);
            blackAdapter = new CapturedPiecesAdapter(this, blackCapturedPieceIds);

            // Find the ListViews in your layout
            RecyclerView whiteCapturedRecyclerView = FindViewById<RecyclerView>(Resource.Id.whiteCapturedRecyclerViewTwoPlayer);
            RecyclerView blackCapturedRecyclerView = FindViewById<RecyclerView>(Resource.Id.blackCapturedRecyclerViewTwoPlayer);

            // Set the adapters to the ListViews
            // Set up the RecyclerViews with a horizontal layout manager
            whiteCapturedRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
            blackCapturedRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

            // Set the adapters to the RecyclerViews
            whiteCapturedRecyclerView.SetAdapter(whiteAdapter);
            blackCapturedRecyclerView.SetAdapter(blackAdapter);

            blackPlayerTimer = FindViewById<TextView>(Resource.Id.blackPlayerTimerTwoPlayer);
            whitePlayerTimer = FindViewById<TextView>(Resource.Id.whitePlayerTimerTwoPlayer);

            // Set default time
            blackTime = defaultTime;
            whiteTime = defaultTime;

            // Update timers
            UpdateTimers();

            blackTimer = new System.Timers.Timer(1000);
            blackTimer.Elapsed += OnBlackTimedEvent;
            blackTimer.AutoReset = true;

            whiteTimer = new System.Timers.Timer(1000);
            whiteTimer.Elapsed += OnWhiteTimedEvent;
            whiteTimer.AutoReset = true;

            // Start the timer
            whiteTimer.Start();

            soundPool = new SoundPool.Builder().SetMaxStreams(2).Build();
            moveSoundId = soundPool.Load(this, Resource.Raw.move_self, 1);
            checkSoundId = soundPool.Load(this, Resource.Raw.move_check, 1);
            castleSoundId = soundPool.Load(this, Resource.Raw.castle, 1);
            captureSoundId = soundPool.Load(this, Resource.Raw.capture, 1);
            promotionSoundId = soundPool.Load(this, Resource.Raw.promote, 1);
            gameEndSoundId = soundPool.Load(this, Resource.Raw.game_end, 1);
            gameStartSoundId = soundPool.Load(this, Resource.Raw.game_start, 1);
            tensecondsSoundId = soundPool.Load(this, Resource.Raw.tenseconds, 1);
        }


        private void OnBlackTimedEvent(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (blackTime > TimeSpan.Zero)
                {
                    blackTime = blackTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateTimers();

                    if (blackTime == TimeSpan.FromSeconds(10))
                    {
                        // Perform the desired action when 10 seconds are left
                        PlayTenSecondsSound();
                    }
                }
                else
                {
                    blackTimer.Stop(); // Stop the timer when time is up
                    whiteWon = true;
                    createEndGameDialog();
                }
            });
        }

        private void OnWhiteTimedEvent(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (whiteTime > TimeSpan.Zero)
                {
                    whiteTime = whiteTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateTimers();

                    if (whiteTime == TimeSpan.FromSeconds(10))
                    {
                        // Perform the desired action when 10 seconds are left
                        PlayTenSecondsSound();
                    }
                }
                else
                {
                    whiteTimer.Stop(); // Stop the timer when time is up
                    whiteWon = false;

                    createEndGameDialog();
                }
            });
        }

        private void UpdateTimers()
        {
            blackPlayerTimer.Text = blackTime.ToString(@"mm\:ss");
            whitePlayerTimer.Text = whiteTime.ToString(@"mm\:ss");
        }

        private void PlayMoveSound()
        {
            soundPool.Play(moveSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayCheckSound()
        {
            soundPool.Play(checkSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayCastleSound()
        {
            soundPool.Play(castleSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayCaptureSound()
        {
            soundPool.Play(captureSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayPromotionSound()
        {
            soundPool.Play(promotionSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayGameEndSound()
        {
            soundPool.Play(gameEndSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayGameStartSound()
        {
            Thread.Sleep(500);
            soundPool.Play(gameStartSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        private void PlayTenSecondsSound()
        {
            soundPool.Play(tensecondsSoundId, 1.0f, 1.0f, 1, 0, 1.0f);
        }

        // Playing a sound accoring to the move
        private void PlaySound(ChessMove chessMove)
        {
            if (chessboard.IsInCheck())
            {
                PlayCheckSound();
            }
            else if (chessMove.IsKingsideCastle || chessMove.IsQueensideCastle)
            {
                PlayCastleSound();
            }
            else if (chessMove.IsCapture || chessMove.IsEnPassantCapture)
            {
                PlayCaptureSound();
            }
            else if (chessMove.IsPromotion)
            {
                PlayPromotionSound();
            }
            else
            {
                PlayMoveSound();
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            blackTimer.Dispose();
            whiteTimer.Dispose();
            soundPool.Release();
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

            ImageButton btnExit = FindViewById<ImageButton>(Resource.Id.btnExitTwoPlayer);
            btnExit.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(MainPageActivity));
                StartActivity(intent);
                Finish();
            };

            // Find the TableLayout in your XML layout
            TableLayout chessboardLayout = FindViewById<TableLayout>(Resource.Id.chessboardLayoutTwoPlayer);

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
                        // Update the UI
                        if (chessboard.isWhiteTurn)
                        {
                            whiteCapturedPieceIds.Add(chessboard.convertChessPieceToImage(chessboard.lastPieceCaptured));
                            whiteAdapter.NotifyDataSetChanged();
                        }
                        else
                        {
                            blackCapturedPieceIds.Add(chessboard.convertChessPieceToImage(chessboard.lastPieceCaptured));
                            blackAdapter.NotifyDataSetChanged();
                        }

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


                    if (chessboard.isWhiteTurn)
                    {
                        whiteTimer.Stop();
                        blackTimer.Start();
                    }
                    else
                    {
                        whiteTimer.Start();
                        blackTimer.Stop();
                    }

                    chessboard.SwitchPlayerTurn();
                    PlaySound(chessMove);
                    ResetSelection();


                } else if (chessMove.IsIllegalMove)
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
        