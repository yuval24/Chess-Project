using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Chess_FirstStep.Data_Classes;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.BottomNavigation;



namespace Chess_FirstStep
{
    [Activity(Label = "LoginPageActivity")]
    public class MainPageActivity : Activity
    {
        
        NetworkManager networkManager;
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Create an instance of the BottomButtonsFragment
            // Set up bottom navigation view
            var fragmentContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
            var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            // Set up the menu programmatically
            bottomNavigation.Menu.Clear(); // Clear existing menu items if any

            // Add menu items
            bottomNavigation.Menu.Add(0, MainMenuIds.ActionHome, 0, "Home Page").SetIcon(Resource.Drawable.home_chess);
            bottomNavigation.Menu.Add(0, MainMenuIds.ActionHistory, 0, "History Page").SetIcon(Resource.Drawable.history_chess);
            bottomNavigation.Menu.Add(0, MainMenuIds.ActionFriends, 0, "Friends Page").SetIcon(Resource.Drawable.friends_chess);

            // Subscribe to the item selected event
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;



            networkManager = NetworkManager.Instance;
            Task.Run(() => networkManager.ReconnectAsync());
          
            Task.Run(() => CommunicationLoop());

            ReplaceFragment(new HomePageFragment());
        }
        // This loop purpose is to check if the client is still Authenticated

        void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            int itemId = e.Item.ItemId;

            switch (itemId)
            {
                case MainMenuIds.ActionHome:
                    ReplaceFragment(new HomePageFragment());
                    break;
                case MainMenuIds.ActionHistory:
                    ReplaceFragment(new HistoryPageFragment());
                    break;
                case MainMenuIds.ActionFriends:
                    ReplaceFragment(new FriendsPageFragment());
                    break;
                   
            }
        }

        void ReplaceFragment(Android.App.Fragment fragment)
        {
            var transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.fragment_container, fragment);
            transaction.Commit();
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

                    if (data.type.Equals(ActivityType.AUTHENTICATE))
                    {
                        if (!data.success)
                        {
                            Intent intent = new Intent(this, typeof(LoginActivity));
                            StartActivity(intent);
                            Finish();

                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** Error in CommunicationLoop MainPageActivity: {ex.Message}");
            }
        }
       
    }

}