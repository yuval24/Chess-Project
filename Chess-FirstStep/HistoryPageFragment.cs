using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Chess_FirstStep.Data_Classes;

namespace Chess_FirstStep
{
    public class HistoryPageFragment : Fragment, HistoryAdapter.OnItemClickListener
    {
        private RecyclerView recyclerView;
        private HistoryAdapter adapter;
        private List<GameHistory> gameHistoryList;
        private NetworkManager networkManager;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_history_page, container, false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView_history);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));

            gameHistoryList = new List<GameHistory>();
            adapter = new HistoryAdapter(gameHistoryList);
            adapter.SetOnItemClickListener(this); // Set the click listener
            recyclerView.SetAdapter(adapter);

            networkManager = NetworkManager.Instance;

            networkManager.RequestGameHistory();
            Task.Run(() => LoadGameHistory());

            return view;
        }

        private async Task LoadGameHistory()
        {
            // Fetch game history data from the server
            try
            {
              


                while (true)
                {
                    string json = await networkManager.ReceiveDataFromServer();
                    GameHistoryData gameHistoryData = GameHistoryData.Deserialize(json);
                    List<GameHistory> historyData = gameHistoryData.gameHistories;
                    

                    if (gameHistoryData.type.Equals(ActivityType.GAME_HISTORY))
                    {
                        if(historyData != null)
                        {
                            UpdateGameHistory(historyData);
                        }
                        
                        break; // Exit the loop after successful transition
                    } else if (gameHistoryData.type.Equals(ActivityType.AUTHENTICATE))
                    {
                        if (!gameHistoryData.success)
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                Intent intent = new Intent(Activity, typeof(LoginActivity));
                                StartActivity(intent);
                                Activity.Finish();
                            });

                        }
                        
                    }
                }

                
            }
            catch (Exception ex)
            {
                // Handle errors or exceptions
            }
        }

        private void UpdateGameHistory(List<GameHistory> historyData)
        {
            // Update the RecyclerView adapter with new game history data
            Activity.RunOnUiThread(() =>
            {
                gameHistoryList.Clear();
                gameHistoryList.AddRange(historyData);
                adapter.NotifyDataSetChanged();
            });
        }

        public void OnItemClick(int position)
        {
            // Get the clicked game history object
            GameHistory clickedGame = gameHistoryList[position];
            // Create an intent to start the GameMovesActivity
            Intent intent = new Intent(Activity, typeof(GameMovesActivity));

            // Pass necessary data to the GameMovesActivity
            intent.PutStringArrayListExtra("GameMoves", clickedGame.moves);

            // Start the activity
            Activity.StartActivity(intent);
        }
    }
}