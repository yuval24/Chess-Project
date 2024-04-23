using System;
using System.Collections.Generic;
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
using System.Threading.Tasks;

namespace Chess_FirstStep
{
    public class SocketManager
    {
        public Socket socket { get; set; }

        public bool IsConnected => socket != null && socket.Connected;

        public async Task<bool> Connect(string ipAddress, int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(IPAddress.Parse(ipAddress), port);
                return IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public void Close()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);  // Use Shutdown instead of ShutdownAsync
                socket.Close();
            }
            socket = null;
        }
    }
}