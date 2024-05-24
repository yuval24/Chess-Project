using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;



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
        private bool humanTurn = true;

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
        private bool endGameToken = false;
        private bool gameAborted = false;
        private bool gameWonOnTime = false;

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
            SetContentView(Resource.Layout.activity_ai_game);

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
            RecyclerView whiteCapturedRecyclerView = FindViewById<RecyclerView>(Resource.Id.whiteCapturedRecyclerViewAI);
            RecyclerView blackCapturedRecyclerView = FindViewById<RecyclerView>(Resource.Id.blackCapturedRecyclerViewAI);

            // Set the adapters to the ListViews
            // Set up the RecyclerViews with a horizontal layout manager
            whiteCapturedRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
            blackCapturedRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

            // Set the adapters to the RecyclerViews
            whiteCapturedRecyclerView.SetAdapter(whiteAdapter);
            blackCapturedRecyclerView.SetAdapter(blackAdapter);

            blackPlayerTimer = FindViewById<TextView>(Resource.Id.blackPlayerTimerAI);
            whitePlayerTimer = FindViewById<TextView>(Resource.Id.whitePlayerTimerAI);

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
            chessboard = new Chessboard();
            chessPieceViews = new ImageView[8, 8];
            chessboard.InitializeChessboard(this);
        }

        // Initialize the UI elements and attach click event handlers
        private void InitializeUI()
        {
            // Find the TableLayout in your XML layout
            TableLayout chessboardLayout = FindViewById<TableLayout>(Resource.Id.chessboardLayoutAI);
            ImageButton btnExit = FindViewById<ImageButton>(Resource.Id.btnExitAI);
            btnExit.Click += (s, e) =>
            {
                if (endGameToken)
                {
                    Intent intent = new Intent(this, typeof(MainPageActivity));
                    StartActivity(intent);
                    Finish();
                } else
                {
                    ShowExitGameDialog();
                }
            };

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
            if (humanTurn && !endGameToken)
            {
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


        // This function handles the move that was made and displays it on the screen
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
                        // handles en passent
                        ImageView targetImageView = chessPieceViews[targetRow, targetCol];
                        targetImageView.SetImageDrawable(selectedImageView.Drawable);
                        selectedImageView.SetImageDrawable(null);

                        // we can see here that the captured piece is deleted
                        targetImageView = chessPieceViews[selectedRow, targetCol];
                        targetImageView.SetImageDrawable(null);
                    }
                    else if (chessMove.IsKingsideCastle || chessMove.IsQueensideCastle)
                    {
                        // handles castling
                        MoveKingAndRookForCastle();
                    }
                    else if (chessMove.IsPromotion)
                    {
                        // handles promotion
                        ImageView targetImageView = chessPieceViews[targetRow, targetCol];

                        if (chessboard.isWhiteTurn)
                        {
                            targetImageView.SetImageResource(Resource.Drawable.Chess_qlt60);
                        } else
                        {
                            targetImageView.SetImageResource(Resource.Drawable.Chess_qdt60);
                        }
                        
                        selectedImageView.SetImageDrawable(null);
                    }
                    else if (chessMove.IsCapture)
                    {
                        // Update the UI
                        ImageView targetImageView = chessPieceViews[targetRow, targetCol];
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

                    chessboard.SwitchPlayerTurn();
                    ResetSelection();
                    humanTurn = false;
                    whiteTimer.Stop();
                    blackTimer.Start();
                    PlaySound(chessMove);
                    HandleAIMoveAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine($"Error: {task.Exception.GetBaseException().Message}");
                        }
                        else
                        {
                            handlesAIMove(task.Result);
                            whiteTimer.Start();
                            blackTimer.Stop();
                            humanTurn = true;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());



                }
                else if (chessMove.IsIllegalMove)
                {
                    // The king is in danger, altering the player.
                    Toast.MakeText(this, "You will lose your king moron", ToastLength.Short).Show();
                }
               

            }
            ResetSelection();
        }

        private async Task<ChessMove> HandleAIMoveAsync()
        {
            ChessAI chessAI = new ChessAI(chessboard.isWhiteTurn, 3);
            ChessMove bestMove = await Task.Run(() => chessAI.GetBestMove(chessboard));
            return bestMove;

        }

        private void handlesAIMove(ChessMove AImove)
        {
            
            
            if (AImove != null)
            {
                selectedImageView = chessPieceViews[AImove.StartRow, AImove.StartCol];
                ImageView targetImageView = chessPieceViews[AImove.EndRow, AImove.EndCol];
                selectedRow = AImove.StartRow;
                selectedCol = AImove.StartCol;
                selectedPiece = chessboard.GetChessPieceAt(selectedRow, selectedCol);
                targetRow = AImove.EndRow;
                targetCol = AImove.EndCol;
                chessboard.ApplyMove(AImove);
                if (AImove.IsEnPassantCapture)
                { 
                    targetImageView.SetImageDrawable(selectedImageView.Drawable);
                    selectedImageView.SetImageDrawable(null);

                    targetImageView = chessPieceViews[selectedRow, targetCol];
                    targetImageView.SetImageDrawable(null);
                }
                else if (AImove.IsKingsideCastle || AImove.IsQueensideCastle)
                {
                    MoveKingAndRookForCastle();
                }
                else if (AImove.IsPromotion)
                {
                    //ImageView targetImageView = chessPieceViews[targetRow, targetCol];
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
                else if (AImove.IsCapture)
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
                ResetSelection();

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


        // Displays to the screen the end game dialog.
        public void createEndGameDialog()
        {
            endGameToken = true;
            whiteTimer.Stop();
            blackTimer.Stop();
            PlayGameEndSound();
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);


            builder.SetCancelable(true);

            // Set the dialog content using a layout resource
            LayoutInflater inflater = LayoutInflater.From(this);
            View dialogView = inflater.Inflate(Resource.Layout.dialog_endgame, null);

            builder.SetView(dialogView);

            // Set dialog content and other properties as needed
            TextView gameSummaryTextView = dialogView.FindViewById<TextView>(Resource.Id.gameSummaryTextView);
            TextView whyEndedTextView = dialogView.FindViewById<TextView>(Resource.Id.whyEndedTextView);
            Button newGameButton = dialogView.FindViewById<Button>(Resource.Id.newGameButton);
            Button cancelButton = dialogView.FindViewById<Button>(Resource.Id.cancelButton);


            if (gameResigned)
            {
                if (whiteWon)
                {
                    gameSummaryTextView.Text = "White Won";
                }
                else
                {
                    gameSummaryTextView.Text = "Black Won";
                }
                whyEndedTextView.Text = "by resignation";
            }
            else if (gameAborted)
            {
                gameSummaryTextView.Text = "Game Aborted";
                whyEndedTextView.Text = "";
            }
            else if (whiteWon)
            {
                gameSummaryTextView.Text = "White Won";
                if (gameWonOnTime)
                {
                    whyEndedTextView.Text = "on time";
                }
                else
                {
                    whyEndedTextView.Text = "by checkmate";
                }

            }
            else if (blackWon)
            {
                gameSummaryTextView.Text = "Black Won";
                if (gameWonOnTime)
                {
                    whyEndedTextView.Text = "on time";
                }
                else
                {
                    whyEndedTextView.Text = "by checkmate";
                }

            }
            else
            {
                gameSummaryTextView.Text = "Draw";
            }

            // Create the AlertDialog and show it
            Android.App.AlertDialog alertDialog = builder.Create();
            alertDialog.Show();

            // Set button click listeners if needed
            newGameButton.Click += (sender, args) =>
            {

            };

            cancelButton.Click += (sender, args) =>
            {
                alertDialog.Dismiss();
            };


        }

        private void ShowExitGameDialog()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            string builderTitle = string.Empty;
            if (chessboard.moveCount == 0)
            {
                builderTitle = "Abort Game";
                builder.SetMessage("Are you sure you want to abort the game?");
            }
            else if (chessboard.moveCount == 1)
            {
                builderTitle = "Abort Game";
                builder.SetMessage("Are you sure you want to abort the game?");
            }
            else
            {
                builderTitle = "Resign";
                builder.SetMessage("Are you sure you want to resign?");
            }
            builder.SetTitle(builderTitle);
            builder.SetPositiveButton("Yes", (sender, args) =>
            {
                // Handle the "Yes" button click (exit game)
                // Close the activity and exit the game
                if (builderTitle.Equals("Resign"))
                {
                    gameResigned = true;
                }
                else if (builderTitle.Equals("Abort Game"))
                {
                    gameAborted = true;
                }
                createEndGameDialog();
            });
            builder.SetNegativeButton("No", (sender, args) =>
            {
                // Handle the "No" button click (cancel exit)
                // Do nothing, dismiss the dialog
            });

            Android.App.AlertDialog dialog = builder.Create();

            dialog.Show();
        }
    }
}