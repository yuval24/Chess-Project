using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Chess_FirstStep.Data_Classes;

namespace Chess_FirstStep
{
    [Activity(Label = "WaitingForOnlineGameActivity")]
    public class WaitingForOnlineGameActivity : Activity
    {
        NetworkManager networkManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_waiting);

            // sends to the server that this user wants to play and and waits for resopond
            // when the server approves it sends the client to the online game activity to a game
            // with another user

            networkManager = NetworkManager.Instance;
            networkManager.sendWantsToPlayOnlineGame();

            Task.Run(() => CommunicationLoop());
        }

        private async Task CommunicationLoop()
        {
            try
            {
                while (true)
                {
                    string json = await networkManager.ReceiveDataFromServer();
                    Data data = Data.Deserialize(json);
                    Console.WriteLine("Received data: " + data.ToString());

                    if ((data.type.Equals(ActivityType.APPROVE_TO_PLAY)))
                    {

                        // Transition to OnlineGameActivity upon successful found of a match 
                        Intent intent = new Intent(this, typeof(OnlineGameActivity));

                        if (data.content.Equals("WHITE"))
                        {
                            intent.PutExtra("IS_WHITE", true);
                        } else if (data.content.Equals("BLACK"))
                        {
                            intent.PutExtra("IS_WHITE", false);
                        }
                       
                        StartActivity(intent);
                        Finish();
                        break; // Exit the loop after successful transition
                    }
                    else
                    {
                       

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** Error in CommunicationLoop Waiting For Game: {ex.Message}");
            }
        }
    }
}