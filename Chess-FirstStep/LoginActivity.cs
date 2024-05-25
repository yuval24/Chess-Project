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
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        EditText editTextUsername;
        EditText editTextPassword;
        NetworkManager networkManager;
        CheckBox checkBoxStayLoggedIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);


            networkManager = NetworkManager.Instance;


            TextView tvTitle = FindViewById<TextView>(Resource.Id.tvTitle);
            checkBoxStayLoggedIn = FindViewById<CheckBox>(Resource.Id.checkBoxLogin);
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            Button btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);
            editTextUsername = FindViewById<EditText>(Resource.Id.editTextUsername);

            AnimateView(tvTitle, 0);
            AnimateView(editTextUsername, 200);
            AnimateView(editTextPassword, 400);
            AnimateView(checkBoxStayLoggedIn, 600);
            AnimateView(btnLogin, 800);
            AnimateView(btnSignUp, 1000);
           
            btnSignUp.Click += btnSignUp_Click;
            btnLogin.Click += btnLogin_Click;
            Task.Run(() => CommunicationLoop());
        }

        private void AnimateView(View view, int delay)
        {
            view.Animate()
                .Alpha(1f)
                .TranslationY(0)
                .SetDuration(600)
                .SetStartDelay(delay)
                .Start();
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
                    else if (data.type.Equals(ActivityType.AUTHENTICATE))
                    {
                        bool stayLoggedIn = SharedPreferencesManager.GetStayLoggedIn();
                        if (data.success && stayLoggedIn)
                        {
                            editTextUsername.Text = SharedPreferencesManager.GetUsername();
                            editTextPassword.Text = SharedPreferencesManager.GetUsername();
                            Intent intent = new Intent(this, typeof(MainPageActivity));
                            StartActivity(intent);
                            Finish();
                        }
                        else
                        {
                            SharedPreferencesManager.DeleteJwtToken();
                            SharedPreferencesManager.DeleteUsername();
                            SharedPreferencesManager.DeleteStayLoggedIn();
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
                SharedPreferencesManager.SaveStayLoggedIn(checkBoxStayLoggedIn.Checked);
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
                SharedPreferencesManager.SaveStayLoggedIn(checkBoxStayLoggedIn.Checked);
                networkManager.SendLoginInformation(username, password);
            }
        }
    }
}