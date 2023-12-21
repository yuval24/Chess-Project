using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess_FirstStep
{
    public class ChessNetworkManager
    {
        private const string ServerIp = "10.0.2.2"; // Unique IP for Xamarin
        private const int ServerPort = 3000; // The port that the server is listening on

        private ChessMove userInput;
        private Socket client;
        private StreamReader reader;
        private StreamWriter writer;

        public ChessNetworkManager()
        {
            this.userInput = null;
            this.client = null;
            /*try
            {
                ConnectToServer();
                SendHelloToServer();
                ReceiveHelloFromServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }*/

            try
            {
                ConnectToServer();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

      
        public async Task CommunicationThread()
        {
            
            if (userInput != null)
            {
                ChessMove move = this.userInput;

                SendMove(move);
                Console.WriteLine($"Sending move: {move}");
                userInput = null;
            }

            ChessMove receivedMove = await ReceiveMoveAsync();

            // Debugging statement
            Console.WriteLine($"Received move: {receivedMove}");

            // Add a delay to avoid continuous looping without breaks
            await Task.Delay(100); // Adjust the delay duration as needed
        }

        public async Task<ChessMove> ReceiveMoveAsync()
        {
            try
            {
                string receivedMove = await Task.Run(() => ReceiveMessageFromServer());
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
            this.userInput = move;
        }
        private void ConnectToServer()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));

            // Initialize reader and writer after connecting to the server
            NetworkStream networkStream = new NetworkStream(client);
            reader = new StreamReader(networkStream);
            writer = new StreamWriter(networkStream) { AutoFlush = true };
        }

        private void SendHelloToServer()
        {
            string dataToSend = "Hello from C# client!";
            writer.WriteLine(dataToSend);
        }

        private void ReceiveHelloFromServer()
        {
            string receivedData = reader.ReadLine();
            Console.WriteLine($"Received from server: {receivedData}");
        }

        public void SendMove(ChessMove move)
        {
            string moveString = ConvertMoveToString(move);
            SendMessageToServer(moveString);
        }

        public ChessMove ReceiveMove()
        {
            string receivedMove = ReceiveMessageFromServer();
            return ConvertStringToMove(receivedMove);
        }

        private void SendMessageToServer(string message)
        {
            writer.WriteLine(message);
        }

        private string ReceiveMessageFromServer()
        {
            return reader.ReadLine();
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
            else if (moveString.Contains("O-O"))
            {
                move.IsKingsideCastle = true;
            }
            else if (moveString.Contains("O-O-O"))
            {
                move.IsQueensideCastle = true;
            }

            return move;
        }
    }
}