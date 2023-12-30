using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;

namespace Chess_FirstStep
{
    public class ChessNetworkManager : IDisposable
    {
        private const string ServerIp = "10.0.2.2";
        private const int ServerPort = 3000;

        private ChessMove userInput;
        private Socket client;
        private StreamReader reader;
        private StreamWriter writer;
        public bool otherClientsConnected;
        private bool communicationStarted;
        private CancellationTokenSource cancellationTokenSource;
        public bool otherClientsConnectedIsSet;

        public ChessNetworkManager()
        {
            try
            {
                Initialize();
                ConnectToServer();
                SendChecker();
                //Task.Run(() => ReceiveMessagesFromServerAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        private void Initialize()
        {
            userInput = null;
            client = null;
            communicationStarted = false;
            otherClientsConnected = false;
            otherClientsConnectedIsSet = false;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task CommunicationThread()
        {
            try
            {
                if (userInput != null)
                {
                    ChessMove move = userInput;
                    await Task.Run(() => SendMove(move));
                    Console.WriteLine($"Sending move: {move}");
                    userInput = null;
                }

                ChessMove receivedMove = await ReceiveMoveAsync();
                Console.WriteLine($"Received move: {receivedMove}");

                // Add a delay to avoid continuous looping without breaks
                await Task.Delay(100); // Adjust the delay duration as needed
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CommunicationThread: {ex.Message}");
            }
        }

        public async Task<ChessMove> ReceiveMoveAsync()
        {
            try
            {
                string receivedMove = await ReceiveMessageFromServerAsync();
                return ConvertStringToMove(receivedMove);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while receiving move: {ex.Message}");
                return null;
            }
        }

        public void SetUserInputChessMove(ChessMove move)
        {
            userInput = move;
        }

        private void ConnectToServer()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));

            NetworkStream networkStream = new NetworkStream(client);
            reader = new StreamReader(networkStream);
            writer = new StreamWriter(networkStream) { AutoFlush = true };
        }

        public async Task<bool> GetInitalStartingIsWhite()
        {
            try
            {
                
                while(true)
                {
                    string receivedMessage = await ReceiveMessageFromServerAsync();
                    if (receivedMessage == "NO")
                    {
                        return true;
                    }
                    else if(receivedMessage == "YES")
                    {
                        this.otherClientsConnected = true;
                        return false;
                    }
                    Thread.Sleep(1000);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** Error while receiving messages: {ex.Message}");
                return false;
            }
        }
        private void SendChecker()
        {
            string dataToSend = "User Entered";
            writer.WriteLine(dataToSend);
        }

        public bool CheckServerOutput(string check)
        {
            return check.Equals("YES", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ReceiveMessagesFromServerAsync()
        {
            try
            {
                while (!otherClientsConnected && !cancellationTokenSource.Token.IsCancellationRequested)
                {
                    
                    string receivedMessage = await ReceiveMessageFromServerAsync();
                    otherClientsConnected = CheckServerOutput(receivedMessage);
                    Thread.Sleep(1000);
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** Error while receiving messages: {ex.Message}");
            }
        }

        public void SendMove(ChessMove move)
        {
            string moveString = ConvertMoveToString(move);
            SendMessageToServer(moveString);
        }


        private void SendMessageToServer(string message)
        {
            writer.WriteLine(message);
        }

        public async Task<string> ReceiveMessageFromServerAsync()
        {
            try
            {
                string receivedMessage = await reader.ReadLineAsync();
                return receivedMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while receiving message from server: {ex.Message}");
                return string.Empty;
            }
        }

        private void CloseConnection()
        {
            try
            {
                reader?.Close();
                writer?.Close();
                client?.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while closing connection: {e.Message}");
            }
        }


        // Add methods to convert ChessMove to/from String
        private string ConvertMoveToString(ChessMove move)
        {
            // Implement the conversion logic
            return move.ToString(); // Placeholder, replace with actual logic
        }

        private ChessMove ConvertStringToMove(string moveString)
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
            if(moveString.Contains("Q"))
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

        public void Dispose()
        {
            CloseConnection();
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
    }
}