using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using System.Threading;
using Android.Content;
using Chess_FirstStep.Data_Classes;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System.Timers;

namespace Chess_FirstStep
{
    [Activity(Label = "OnlineGameActivity")]
    public class OnlineGameActivity : AppCompatActivity
    {
        private Chessboard chessboard;
        private ImageView selectedImageView;
        private ChessPiece selectedPiece = null;
        private ImageView[,] chessPieceViews;
        private int selectedRow = -1; // Initialize with an invalid value
        private int selectedCol = -1; // Initialize with an invalid value
        private int targetCol = -1;
        private int targetRow = -1;
        private List<int> whiteCapturedPieces = new List<int>();
        private List<int> blackCapturedPieces = new List<int>();
        private Android.Graphics.Color[,] originalSquareColors = new Android.Graphics.Color[8, 8];
        private bool whiteWon = false;
        private bool blackWon = false;
        NetworkManager networkManager;
        private bool thisPlayerIsWhite;
        private CancellationTokenSource cancellationTokenSource;
        private bool endGameToken;
        private List<int> whiteCapturedPieceIds;
        private List<int> blackCapturedPieceIds;
        private CapturedPiecesAdapter whiteAdapter;
        private CapturedPiecesAdapter blackAdapter;
        private bool gameResigned = false;
        private bool gameAborted = false;
        private TextView blackPlayerTimer;
        private TextView whitePlayerTimer;
        private System.Timers.Timer timer;
        private TimeSpan blackTime;
        private TimeSpan whiteTime;
        private TimeSpan defaultTime = TimeSpan.FromMinutes(5); // Default time for each player
        private bool isBlackTimerRunning = false;
        private bool isWhiteTimerRunning = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_online_game);
            // Initialize your chessboard and views
            InitializeChessboard();

            // Initialize the UI elements and attach click event handlers
            InitializeUI();

            // Checking which color is this player.


            // Initialize lists to hold captured piece IDs
            whiteCapturedPieceIds = new List<int>();
            blackCapturedPieceIds = new List<int>();

            // Create adapters for the ListView
            whiteAdapter = new CapturedPiecesAdapter(this, whiteCapturedPieceIds);
            blackAdapter = new CapturedPiecesAdapter(this, blackCapturedPieceIds);

            // Find the ListViews in your layout
            RecyclerView whiteCapturedRecyclerView = FindViewById<RecyclerView>(Resource.Id.whiteCapturedRecyclerView);
            RecyclerView blackCapturedRecyclerView = FindViewById<RecyclerView>(Resource.Id.blackCapturedRecyclerView);

            // Set the adapters to the ListViews
            // Set up the RecyclerViews with a horizontal layout manager
            whiteCapturedRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
            blackCapturedRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

            // Set the adapters to the RecyclerViews
            whiteCapturedRecyclerView.SetAdapter(whiteAdapter);
            blackCapturedRecyclerView.SetAdapter(blackAdapter);

            blackPlayerTimer = FindViewById<TextView>(Resource.Id.blackPlayerTimer);
            whitePlayerTimer = FindViewById<TextView>(Resource.Id.whitePlayerTimer);

            // Set default time
            blackTime = defaultTime;
            whiteTime = defaultTime;

            // Update timers
            UpdateTimers();

            // Start the timer
            timer = new System.Timers.Timer(1000); // Update every second
            timer.Elapsed += OnTimedEvent;
            timer.Start();


            thisPlayerIsWhite = Intent.GetBooleanExtra("IS_WHITE",false);
            networkManager = NetworkManager.Instance;
            endGameToken = false;
            //Initialize the connection between the client to the server
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => CommunicationLoop(cancellationTokenSource.Token));
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            // Update timers
            if (blackTime > TimeSpan.Zero)
                blackTime = blackTime.Subtract(TimeSpan.FromSeconds(1));
            if (whiteTime > TimeSpan.Zero)
                whiteTime = whiteTime.Subtract(TimeSpan.FromSeconds(1));

            // Update UI
            RunOnUiThread(UpdateTimers);
        }

        private void UpdateTimers()
        {
            blackPlayerTimer.Text = blackTime.ToString(@"mm\:ss");
            whitePlayerTimer.Text = whiteTime.ToString(@"mm\:ss");
        }


        protected override void OnDestroy()
        {
            // Cancel the communication loop when the activity is destroyed
            //new Thread(() => { chessNetworkManager.SendLeave(); }).Start();
            cancellationTokenSource.Cancel();
            base.OnDestroy();
            timer.Stop();
            timer.Dispose();
        }

        // This is the main loop that waits for data to arrive, mainly moves from the other player. handles the Data
        // properly.
        private async Task CommunicationLoop(CancellationToken cancellationToken)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            try
            {
                while (true)
                {
                    if (endGameToken)
                    {
                        break;
                    }
                    string json = await networkManager.ReceiveDataFromServer();
                    Console.WriteLine(json);
                    Data data = Data.Deserialize(json);
                    Console.WriteLine("Received data: " + data.ToString());

                    if ((data.type.Equals(ActivityType.MOVE)))
                    {
                        MoveData moveData = MoveData.Deserialize(json);
                        string moveString = moveData.move;
                        ChessMove receivedMove = networkManager.ConvertStringToMove(moveString);
                        RunOnUiThread(() => HandleReceivedMove(receivedMove));
                    } else if (data.type.Equals(ActivityType.END_GAME))
                    {
                        if (data.content.Equals("resign"))
                        {
                            RunOnUiThread(() => { gameResigned = true; whiteWon = thisPlayerIsWhite; }) ;
                        } else if (data.content.Equals("abort"))
                        {
                            RunOnUiThread(() => gameAborted = true);
                        }
                        RunOnUiThread(() => createEndGameDialog());
                    } else if (data.type.Equals(ActivityType.AUTHENTICATE))
                    {
                        if (!data.success)
                        {
                            SharedPreferencesManager.DeleteJwtToken();
                            SharedPreferencesManager.DeleteUsername();
                        }
                    }
                    else
                    {
                        // Continue waiting for data
                        RunOnUiThread(() =>
                        {
                            Toast.MakeText(this, "Invalid Data", ToastLength.Short).Show();
                        });

                    }
                    Task.Delay(100);

                }
            }
            catch (System.OperationCanceledException)
            {
                // Handle cancellation
                Console.WriteLine("CommunicationLoop cancelled.");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"**** Error in CommunicationLoop During Game: {ex.Message}");
            }
            finally
            {
                source.Dispose();
            }
        }

        private void HandleReceivedMove(ChessMove receivedMove)
        {
            // Synchronize access to shared resources
            lock (chessboard)
            {
                // Process the received move
                // Update your chessboard, UI, etc.
                if (receivedMove != null)
                {
                    selectedRow = receivedMove.StartRow;
                    selectedCol = receivedMove.StartCol;
                    targetRow = receivedMove.EndRow;
                    targetCol = receivedMove.EndCol;
                    selectedImageView = chessPieceViews[receivedMove.StartRow, receivedMove.StartCol];
                    HandleTargetSquareClick();
                }
            }
        }


        private void HandleChessPieceClick(object sender, EventArgs e)
        {
            if(thisPlayerIsWhite == chessboard.isWhiteTurn && !endGameToken)
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

                                
                                
                                if (selectedPiece != null)
                                {
                                    ChessMove chessMove = new ChessMove(selectedRow, selectedCol, targetRow, targetCol);
                                    if (chessboard.IsMoveValid(chessMove))
                                    {
                                        networkManager.SendMoveToServer(chessMove);
                                        HandleTargetSquareClick();
                                    } else
                                    {
                                        ResetSelection();
                                    }
                                }
                                
                            }
                        }
                    }
                }
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


        // Initialize the UI elements and attach click event handlers
        private void InitializeUI()
        {
            // Find the TableLayout in your XML layout
            TableLayout chessboardLayout = FindViewById<TableLayout>(Resource.Id.chessboardLayout);
            ImageButton exitButton = FindViewById<ImageButton>(Resource.Id.btnExit);
            exitButton.Click += (s, e) =>
            {
                //new Thread(() => { chessNetworkManager.SendLeave(); }).Start();
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

        public void InitializeChessboardViewsForBlack()
        {
            // Set Rooks
            chessPieceViews[7, 7].SetImageResource(Resource.Drawable.Chess_rlt60);
            chessPieceViews[7, 0].SetImageResource(Resource.Drawable.Chess_rlt60);
            chessPieceViews[0, 0].SetImageResource(Resource.Drawable.Chess_rdt60);
            chessPieceViews[0, 7].SetImageResource(Resource.Drawable.Chess_rdt60);

            // Set Knights
            chessPieceViews[0, 1].SetImageResource(Resource.Drawable.Chess_ndt60);
            chessPieceViews[0, 6].SetImageResource(Resource.Drawable.Chess_ndt60);
            chessPieceViews[7, 1].SetImageResource(Resource.Drawable.Chess_nlt60);
            chessPieceViews[7, 6].SetImageResource(Resource.Drawable.Chess_nlt60);

            // Set Bishops
            chessPieceViews[0, 2].SetImageResource(Resource.Drawable.Chess_bdt60);
            chessPieceViews[0, 5].SetImageResource(Resource.Drawable.Chess_bdt60);
            chessPieceViews[7, 2].SetImageResource(Resource.Drawable.Chess_blt60);
            chessPieceViews[7, 5].SetImageResource(Resource.Drawable.Chess_blt60);

            // Set Queens
            chessPieceViews[0, 3].SetImageResource(Resource.Drawable.Chess_qdt60);
            chessPieceViews[7, 3].SetImageResource(Resource.Drawable.Chess_qlt60);

            // Set Kings
            chessPieceViews[0, 4].SetImageResource(Resource.Drawable.Chess_kdt60);
            chessPieceViews[7, 4].SetImageResource(Resource.Drawable.Chess_klt60);

            // Set Pawns
            for (int i = 0; i < 8; i++)
            {
                chessPieceViews[1, i].SetImageResource(Resource.Drawable.Chess_pdt60);
                chessPieceViews[6, i].SetImageResource(Resource.Drawable.Chess_plt60);
            }
        }


        // Get the background color of a square
        private Android.Graphics.Color GetSquareBackgroundColor(int row, int col)
        {
            return ((ColorDrawable)chessPieceViews[row, col].Background).Color;
        }



        private void HandleTargetSquareClick()
        {
            if (selectedImageView != null)
            {
                selectedPiece = chessboard.GetChessPieceAt(selectedRow, selectedCol);
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
            //chessboard.SwitchPlayerTurn();
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
            endGameToken = true;
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
                } else
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
                whyEndedTextView.Text = "by checkmate";
                if(thisPlayerIsWhite)
                {
                    networkManager.SendEndGameToServer("white");
                }
            }
            else if (blackWon)
            {
                gameSummaryTextView.Text = "Black Won";
                whyEndedTextView.Text = "by checkmate";
                if (!thisPlayerIsWhite)
                {
                    networkManager.SendEndGameToServer("black");
                }
            }
            else
            {
                gameSummaryTextView.Text = "Draw";
                if (thisPlayerIsWhite)
                {
                    networkManager.SendEndGameToServer("draw");
                }
            }

            // Create the AlertDialog and show it
            Android.App.AlertDialog alertDialog = builder.Create();
            alertDialog.Show();

            // Set button click listeners if needed
            newGameButton.Click += (sender, args) =>
            {
                Task.Run(() =>networkManager.ReconnectAsync());
                Intent intent = new Intent(this, typeof(WaitingForOnlineGameActivity));
                StartActivity(intent);
                Finish();
            };

            cancelButton.Click += (sender, args) =>
            {
                alertDialog.Dismiss();
            };


        }

        // When A player wants to exit it show him this pop up. and he need choose either to exit(abort / resign) or stay.
        private void ShowExitGameDialog()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            string builderTitle = string.Empty;
            if(chessboard.moveCount == 0)
            {
                builderTitle = "Abort Game";
                builder.SetMessage("Are you sure you want to abort the game?");
            } else if(chessboard.moveCount == 1 && chessboard.isWhiteTurn == thisPlayerIsWhite)
            {
                builderTitle = "Abort Game";
                builder.SetMessage("Are you sure you want to abort the game?");
            } else
            {
                builderTitle = "Resign";
                builder.SetMessage("Are you sure you want to resign?");
            }
            builder.SetTitle(builderTitle);
            builder.SetPositiveButton("Yes", (sender, args) =>
            {
                // Handle the "Yes" button click (exit game)
                // Close the activity and exit the game
                if(builderTitle.Equals("Resign")) {
                    networkManager.SendLeavePlayerToServer("resign", thisPlayerIsWhite);
                    whiteWon = !thisPlayerIsWhite;
                    gameResigned = true;
                }
                else if(builderTitle.Equals("Abort Game"))
                {
                    networkManager.SendLeavePlayerToServer("abort", thisPlayerIsWhite);
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