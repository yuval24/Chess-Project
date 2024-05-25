using System;
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

        private SocketManager socketManager;
        public StreamReader reader;
        private StreamWriter writer;
        public string currentUsername { get; set; }

        private NetworkManager()
        {
            currentUsername = "";
            socketManager = new SocketManager();  // Use SocketManager for connection management
        }

        public static NetworkManager Instance
        {
            get
            {
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

        private async Task ConnectToServerAsync()
        {
            try
            {
                System.Console.WriteLine("****** Trying to Connect");
                if (await socketManager.Connect(ServerIp, ServerPort))
                {
                    NetworkStream networkStream = new NetworkStream(socketManager.socket);
                    reader = new StreamReader(networkStream);
                    writer = new StreamWriter(networkStream) { AutoFlush = true };
                    System.Console.WriteLine("****** Connection Successful");
                    ConnectionToTheServer();
                } else 
                {
                    System.Console.WriteLine("The server is not up");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        public async Task StartConnectionAsync()
        {
            await ConnectToServerAsync();
        }

        public async Task ReconnectAsync()
        {
            await CloseConnectionAsync();  // Close existing connection before reconnecting
            await ConnectToServerAsync();   // Attempt to reconnect
            System.Console.WriteLine("Attempting reconnection...");
        }

        private Task CloseConnectionAsync()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
            socketManager.Close();
            System.Console.WriteLine("Connection closed.");
            return Task.CompletedTask;
        }

        // When the connection is estblished the client is Authenticated by the server
        public void ConnectionToTheServer()
        {
            try
            {
                string JWTtoken = SharedPreferencesManager.GetJwtToken();
                string username = SharedPreferencesManager.GetUsername();
                Data data = new Data
                {
                    type = ActivityType.AUTHENTICATE,
                    sender = username,
                    recipient = "server",
                    content = "OK",
                    token = JWTtoken
                };
                string jsonData = data.Serialize();
                writer.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
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
                System.Console.WriteLine($"*** Error: {ex.ToString()}");
            }
        }

        // Sends a request to the server that this current client wants to play 
        public void sendWantsToPlayOnlineGame()
        {

            try
            {
                string JWTtoken = SharedPreferencesManager.GetJwtToken();
                string username = SharedPreferencesManager.GetUsername();
                System.Console.WriteLine($"*** Wants to play");
                Data requestData = new Data
                {
                    type = ActivityType.REQUEST_TO_PLAY,
                    sender = username,
                    recipient = "server",
                    content = "OK",
                    token = JWTtoken
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

        // Gets a move and sending it to the server
        public void SendMoveToServer(ChessMove move)
        {
            try
            {
                string JWTtoken = SharedPreferencesManager.GetJwtToken();
                string username = SharedPreferencesManager.GetUsername();
                string moveString = ConvertMoveToString(move);
                MoveData requestData = new MoveData
                {
                    type = ActivityType.MOVE,
                    sender = username,
                    recipient = "server",
                    content = "OK",
                    move = moveString,
                    token = JWTtoken
                };
                string jsonData = requestData.Serialize();
                writer.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"**** Error while sending move: {ex.Message}");
            }
            
        }

        // Gets a game end state which could be - white, black or draw. 
        // sends the data to the server.
        public void SendEndGameToServer(String gameEndState)
        {
            try
            {
                string JWTtoken = SharedPreferencesManager.GetJwtToken();
                string username = SharedPreferencesManager.GetUsername();
                Data requestData = new Data
                {
                    type = ActivityType.END_GAME,
                    sender = username,
                    recipient = "server",
                    content = gameEndState,
                    token = JWTtoken
                };
                string jsonData = requestData.Serialize();
                writer.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"**** Error while sending end game data: {ex.Message}");
            }

        }

        // Gets why the players leaved - abort, resign. 
        // sends it to the server
        public void SendLeavePlayerToServer(String reasonToLeave, bool isCurrPlayerWhite)
        {
            try
            {
                string username = SharedPreferencesManager.GetUsername();   
                String JWTtoken = SharedPreferencesManager.GetJwtToken();
                string contentToSend = string.Empty;
                if (reasonToLeave.Equals("resign"))
                {
                    if (isCurrPlayerWhite)
                    {
                        contentToSend = "black";
                    }
                    else
                    {
                        contentToSend = "white";
                    }
                } else if (reasonToLeave.Equals("abort"))
                {
                    contentToSend = "draw";
                }

                Data requestData = new Data
                {
                    type = ActivityType.LEAVE_GAME,
                    sender = username,
                    recipient = "server",
                    content = contentToSend,
                    token = JWTtoken
                };
                string jsonData = requestData.Serialize();
                writer.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"**** Error while sending end game data: {ex.Message}");
            }

        }

        // retreiving the history of game for certain player
        public void RequestGameHistory()
        {
            try
            {
                string JWTtoken = SharedPreferencesManager.GetJwtToken();
                string username = SharedPreferencesManager.GetUsername();
                Data requestData = new Data
                {
                    type = ActivityType.GAME_HISTORY,
                    sender = username,
                    recipient = "server",
                    content = "OK",
                    token = JWTtoken
                };
                string jsonData = requestData.Serialize();
                writer.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"**** Error while sending end game data: {ex.Message}");
            }
        }

        // Gets a move and converts it to a string
        public string ConvertMoveToString(ChessMove move)
        {
            return move.ToString(); 
        }

        // Gets a string and converts it to a chess move.
        public static ChessMove ConvertStringToMove(string moveString)
        {
            // Implement the conversion logic
            int startCol = moveString[0] - 'a';    // Subtract 'a' to convert character to integer
            int startRow = moveString[1] - '1';    // Subtract '1' to convert character to integer
            int endCol = moveString[2] - 'a';      // Subtract 'a' to convert character to integer
            int endRow = moveString[3] - '1';      // Subtract '1' to convert character to integer

            ChessMove move = new ChessMove(startRow, startCol, endRow, endCol);
            if (moveString.Contains("X"))
            {
                move.IsCapture = true;
            }
            if (moveString.Contains("Q"))
            {
                move.IsPromotion = true;
            }
            else if (moveString.Contains("CK"))
            {
                move.IsKingsideCastle = true;
            }
            else if (moveString.Contains("CN"))
            {
                move.IsQueensideCastle = true;
            }

            return move;
        }

       
    }
}