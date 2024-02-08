using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Chess_FirstStep
{
    [Activity(Label = "WaitingForOnlineGameActivity")]
    public class WaitingForOnlineGameActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // sends to the server that this user wants to play and and waits for resopond
            // when the server approves it sends the client to the online game activity to a game
            // with another user

            NetworkManager networkManager = NetworkManager.Instance;

            
        }
    }
}