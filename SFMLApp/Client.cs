using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SFMLApp
{
    public class Client
    {
        public PlayerClient Player { get; private set; }
        
        public Client()
        {
            Player = new PlayerClient();
        }

        private Task<Socket> TryConnect(string IP)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), 11000);
            TaskCompletionSource<Socket> tcs = new TaskCompletionSource<Socket>();
            sock.BeginConnect(ip, iar =>
            {
                sock.EndConnect(iar);
                tcs.SetResult(sock);
            }, sock);
            return tcs.Task;
        }

        public async Task Connect(string IP)
        {
            Socket sock = await TryConnect(IP);
            Player.SetOnline(sock);
        }
    }

    public class PlayerClient
    {
        public Queue<Tuple<int, TypeKeyDown> > KeyDown { get; private set; }
        public bool IsOnline { get; private set; }
        public string Names { get; set; }
        public string DataFromServer { get; private set; }
        
        public Socket Socket { get; private set; }
        private Stopwatch ReceiveTimer;

        const int MaxBufferSize = 4096;
        const int MaxWaitTime = 5000;
        public void SetOnline(Socket sock)
        {
            Socket = sock;
            IsOnline = true;
            ReceiveTimer.Start();
        }
        public void CheckOnline()
        {
            if (ReceiveTimer.ElapsedMilliseconds > MaxWaitTime)
                IsOnline = false;
        }
        public void AddKey(int key)
        {
            KeyDown.Enqueue(Utily.MakePair<int, TypeKeyDown>(key, TypeKeyDown.KeyDown));
        }
        public void MouseDown(int button)
        {
            KeyDown.Enqueue(Utily.MakePair<int, TypeKeyDown>(button, TypeKeyDown.MouseDown));
        }
        public void KeyUp(int key)
        {
            KeyDown.Enqueue(Utily.MakePair<int, TypeKeyDown>(key, TypeKeyDown.KeyUp));
        }
        public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Socket.BeginReceive(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(Socket.EndReceive(iar));
            }, Socket);
            return tcs.Task;
        }
        public async Task<string> ReceiveAsync()
        {
            int size = 0;
            byte[] data = new byte[MaxBufferSize];
            while (true)
            {
                var received = await TryReceiveAsync(data, size, MaxBufferSize - size, SocketFlags.None);
                size += received;
                ReceiveTimer.Restart();
                if (data[size - 1] == '\n')
                    return Encoding.ASCII.GetString(data, 0, size);
                Console.WriteLine(received);
            }
        }
        public async Task InfReceive()
        {
            while (IsOnline)
            {
                string message = await ReceiveAsync();
                var arr = message.Split('\n');
                for (int i = 0; i < arr.Length; ++i)
                    ApplyString(arr[i]);
            }
        }
        public void ApplyString(string s)
        {
            DataFromServer = s;
        }
        public Task<int> TrySendAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Socket.BeginSend(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(Socket.EndSend(iar));
            }, Socket);
            return tcs.Task;
        }
        public async Task SendAsync()
        {
            if (!IsOnline)
                return;
            StringBuilder sb = new StringBuilder();
            while (KeyDown.Count > 0)
            {
                var ob = KeyDown.Dequeue();
                sb.AppendFormat("{0} {1} ", ob.Item1, (int)ob.Item2);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append('\n');
            byte[] data = Encoding.ASCII.GetBytes(sb.ToString());
            int itt = 0;
            int left = data.Length;
            while (left > 0)
            {
                var sended = await TrySendAsync(data, itt, left, SocketFlags.None);
                itt += sended;
                left -= sended;
            }
        }
        public PlayerClient()
        {
            IsOnline = false;
            DataFromServer = Names = "";
            ReceiveTimer = new Stopwatch();
        }
    }

    public enum TypeKeyDown
    {
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp,
        MouseMove
    }
}
