﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Chess_FirstStep.Data_Classes;
using System.Threading.Tasks;
using System.Threading;

namespace Chess_FirstStep
{
    public class NetworkManager
    {

        private static NetworkManager NetworkManagerInstance;
        private static readonly object lockObject = new object();

        private const string ServerIp = "10.0.2.2";
        private const int ServerPort = 3001;

        private Socket client;
        private StreamReader reader;
        private StreamWriter writer;
        public string currentUsername {  get; set; }

        // Private constructor to prevent instantiation from outside the class
        private NetworkManager()
        {
            try
            {
                ConnectToServer();
            } 
            catch (Exception ex) 
            {
                System.Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        
        public static NetworkManager Instance
        {
            get
            {
                // Double-checked locking for thread safety
                if (NetworkManagerInstance == null)
                {
                    lock (lockObject)
                    {
                        if (NetworkManagerInstance == null)
                        {
                            NetworkManagerInstance = new NetworkManager();
                        }
                    }
                }
                return NetworkManagerInstance;
            }
        }


        // setting up the connection between the client and the server
        private void ConnectToServer()
        {
            currentUsername = "";
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));

            NetworkStream networkStream = new NetworkStream(client);
            reader = new StreamReader(networkStream);
            writer = new StreamWriter(networkStream) { AutoFlush = true };
        }

        // sending the sign up data to the server to check if the username is not exists
        public void SendSignUpInformation(string username, string password)
        {
            try
            {
                LoginData loginData = new LoginData { 
                    type = ActivityType.SIGNUP, 
                    sender = username,
                    recipient = "server",
                    content = "OK",
                    username = username,
                    password = password
                };
                string jsonData = loginData.Serialize();
                writer.WriteLine(jsonData);
            } catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // sending the login data to the server to check if the username is matching to the password
        public void SendLoginInformation(string username, string password)
        {
            try
            {
                System.Console.WriteLine($"*** Login");
                LoginData loginData = new LoginData
                {
                    type = ActivityType.LOGIN,
                    sender = username,
                    recipient = "server",
                    content = "OK",
                    username = username,
                    password = password
                };
                string jsonData = loginData.Serialize();
                writer.WriteLine(jsonData);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"*** Error: {ex.Message}");
            }
        }

        // Sends a request to the server that this current client wants to play 
        public void sendWantsToPlayOnline()
        {

            try
            {
                System.Console.WriteLine($"*** Wants to play");
                Data requestData = new Data
                {
                    type = ActivityType.REQUEST_TO_PLAY,
                    sender = currentUsername,
                    recipient = "server",
                    content = "OK",
                };
                string jsonData = requestData.Serialize();
                writer.WriteLine(jsonData);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"*** Error: {ex.Message}");
            }
        }
    
        // waits for data from the server
        public async Task<string> ReceiveDataFromServer()
        {
            try
            {
                return await reader.ReadLineAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"**** Error while receiving messages: {ex.Message}");
            }
            return null;
        }

    }
}