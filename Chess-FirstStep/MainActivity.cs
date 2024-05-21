using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using Android.Graphics.Drawables;
using System.Collections.Generic;
using Android.Content;
using Chess_FirstStep.Data_Classes;
using System.Threading.Tasks;
using System.Threading;

namespace Chess_FirstStep
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {


        EditText editTextUsername;
        EditText editTextPassword;
        NetworkManager networkManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);


            networkManager = NetworkManager.Instance;
            
            

            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            Button btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);
            editTextUsername = FindViewById<EditText>(Resource.Id.editTextUsername);

            btnSignUp.Click += btnSignUp_Click;
            btnLogin.Click += btnLogin_Click;
            Task.Run(() => CommunicationLoop());
        }

        // This is the main CommunicationLoop, waits for the user to enter a username and password to either
        // login or signup. When the user enters a correct input it moves to the next activity
        private async Task CommunicationLoop()
        {

            try
            {
                await networkManager.StartConnectionAsync();
                while (true)
                {
                    
                    string json = await networkManager.ReceiveDataFromServer();
                    Data data = Data.Deserialize(json);
                    Console.WriteLine("Received data: " + data.ToString());

                    if ((data.type.Equals(ActivityType.SIGNUP) || data.type.Equals(ActivityType.LOGIN)) && data.content.Equals("OK"))
                    {
                        networkManager.currentUsername = data.recipient;
                        // Transition to MainPageActivity upon successful signup/login
                        SharedPreferencesManager.SaveUsername(data.recipient);
                        SharedPreferencesManager.SaveJwtToken(data.token);
                        Intent intent = new Intent(this, typeof(MainPageActivity));
                        StartActivity(intent);
                        Finish();
                        break; // Exit the loop after successful transition
                    }
                    else if(data.type.Equals(ActivityType.AUTHENTICATE))
                    {
                        if(data.success)
                        {
                            Intent intent = new Intent(this, typeof(MainPageActivity));
                            StartActivity(intent);
                            Finish();
                        } else
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
                            
                        });
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** Error in CommunicationLoop: {ex.Message}");
            }
        }

        // When pressed it sends to the server the username and password in order to signup if they're valid.
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string username = editTextUsername.Text.ToString();
            string password = editTextPassword.Text.ToString();
            if (!username.Equals("") && !password.Equals(""))
            {
                networkManager.SendSignUpInformation(username, password);
            }
        }

        // When pressed it sends to the server the username and password in order to login if they're valid.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = editTextUsername.Text.ToString();
            string password = editTextPassword.Text.ToString();
            if (!username.Equals("") && !password.Equals(""))
            {
                networkManager.SendLoginInformation(username, password);
            }
        }
    }
}