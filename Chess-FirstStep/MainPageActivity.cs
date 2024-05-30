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
using Android.Views.Animations;
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

            // Subscribe to the item selected event
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;


            
            networkManager = NetworkManager.Instance;
            
          
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
                await networkManager.ReconnectAsync();
                while (true)
                {

                    string json = await networkManager.ReceiveDataFromServer();
                    if(json != null)
                    {
                        Data data = Data.Deserialize(json);
                        Console.WriteLine("Received data: " + data.ToString());

                        if (data.type.Equals(ActivityType.AUTHENTICATE))
                        {
                            if (!data.success)
                            {
                                Intent intent = new Intent(this, typeof(LoginActivity));
                                StartActivity(intent);
                                Finish();
                                break;
                            }

                            break;

                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** Error in CommunicationLoop MainPageActivity: {ex.Message}");
            }
        }

        public void ShowGameRequestNotification(string message)
        {
            LayoutInflater inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
            View popupView = inflater.Inflate(Resource.Layout.popup_notification, null);

            TextView messageTextView = popupView.FindViewById<TextView>(Resource.Id.message);
            messageTextView.Text = message;

            Button acceptButton = popupView.FindViewById<Button>(Resource.Id.acceptButton);
            Button rejectButton = popupView.FindViewById<Button>(Resource.Id.rejectButton);

            acceptButton.Click += (s, e) =>
            {
                // Handle accept action
                Toast.MakeText(this, "Game accepted", ToastLength.Short).Show();
                HideGameRequestNotification(popupView);
            };

            rejectButton.Click += (s, e) =>
            {
                // Handle reject action
                Toast.MakeText(this, "Game rejected", ToastLength.Short).Show();
                HideGameRequestNotification(popupView);
            };

            // Set initial position above the top of the screen
            popupView.TranslationY = -popupView.MeasuredHeight;

            // Add the view to a FrameLayout in the activity's root view
            FrameLayout rootView = new FrameLayout(this);
            rootView.LayoutParameters = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);

            FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);
            layoutParams.Gravity = GravityFlags.Top;
            rootView.AddView(popupView, layoutParams);

            ViewGroup mainLayout = (ViewGroup)Window.DecorView.FindViewById(Android.Resource.Id.Content);
            mainLayout.AddView(rootView);

            // Animate the view sliding down from the top
            popupView.Animate()
                .TranslationY(0)
                .SetDuration(500)
                .SetInterpolator(new DecelerateInterpolator())
                .Start();
        }

        private void HideGameRequestNotification(View popupView)
        {
            // Animate the view sliding up out of view
            popupView.Animate()
                .TranslationY(-popupView.Height)
                .SetDuration(500)
                .SetInterpolator(new AccelerateInterpolator())
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    ViewGroup rootView = (ViewGroup)Window.DecorView.FindViewById(Android.Resource.Id.Content);
                    rootView.RemoveView(popupView);
                }))
                .Start();
        }

    }



}