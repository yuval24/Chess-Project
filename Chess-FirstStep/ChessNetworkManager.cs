using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chess_FirstStep
{
    public class ChessNetworkManager
    {
        private const string ServerIp = "10.0.2.2"; // unique IP for Xamrin
        private const int ServerPort = 3000; // The port that the server listening on

        private Socket client;
        private StreamReader reader;
        private StreamWriter writer;
         
        public ChessNetworkManager()
        {
           

            try
            {
                // Create a TCP socket
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the server
                client.Connect(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));

                // Sending data to the server
                string dataToSend = "Hello from C# client!";
                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);
                client.Send(dataBytes);

                // Receiving data from the server
                byte[] receivedBytes = new byte[1024];
                int bytesRead = client.Receive(receivedBytes);
                string receivedData = Encoding.UTF8.GetString(receivedBytes, 0, bytesRead);

                Console.WriteLine($"Received from server: {receivedData}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                
            }
            finally
            {
                CloseConnection();
            }
        }

        public void SendMove(ChessMove move)
        {
            // Convert ChessMove to a string or a specific format
            string moveString = ConvertMoveToString(move);

            // Send the move to the server
            writer.WriteLine(moveString);
        }

        public ChessMove ReceiveMove()
        {
            try
            {
                // Receive the move from the server
                string receivedMove = reader.ReadLine();

                // Convert the received string to ChessMove
                return ConvertStringToMove(receivedMove);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                // Handle the exception appropriately
                return null; // Placeholder, replace with actual logic
            }
        }

        public void CloseConnection()
        {
            try
            {
                // Close the resources
                reader?.Close();
                writer?.Close();
                client?.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                // Handle the exception appropriately
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
            int startRow = moveString[1] - '1';    // Subtract '1' to convert character to integer
            int startCol = moveString[0] - 'a';    // Subtract 'a' to convert character to integer
            int endRow = moveString[3] - '1';      // Subtract '1' to convert character to integer
            int endCol = moveString[2] - 'a';      // Subtract 'a' to convert character to integer

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