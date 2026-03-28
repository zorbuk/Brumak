using Brumak_Client.Forms;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Brumak_Client.Network
{
    public class TcpClientProvider
    {
        private static readonly Logger _logger = new("Client", typeof(TcpClientProvider), App.ShowLogs, App.SaveLogs);

        public TcpClient Client
        { get; private set; } = null!;
        public Stream Stream
        { get; private set; } = null!;
        public StreamReader Reader
        { get; private set; } = null!;
        public StreamWriter Writer
        { get; private set; } = null!;

        public bool Connected
        { get; private set; } = false;

        public int PingMs 
        { get; private set; } = -1;
        public DateTime LastPong 
        { get; private set; }

        public ConcurrentQueue<INetworkFrame> SendQueue
        { get; private set; } = new();
        public SemaphoreSlim SendSignal
        { get; private set; } = new(0);

        public ConcurrentQueue<INetworkFrame> ReceiveQueue
        { get; private set; } = new();
        public SemaphoreSlim ReceiveSignal
        { get; private set; } = new(0);

        public CancellationTokenSource CancellationTokenSource
        { get; private set; } = new();

        public event Action<int>? OnPingUpdated;
        public event Action<bool>? OnDisconnected;

        public async Task ConnectAsync(string ip, int port)
        {
            Client = new TcpClient();
            await Client.ConnectAsync(ip, port);

            Stream = Client.GetStream(); // ssl stream (not supported on client yet)

            Reader = new(Stream, Encoding.UTF8);
            Writer = new(Stream, Encoding.UTF8) { AutoFlush = true };
            Connected = true;

            _logger.Log($"Connected to {ip}:{port} using thread {Environment.CurrentManagedThreadId}");

            _ = Task.Run(ReceiveLoopAsync, CancellationTokenSource.Token);
            _ = Task.Run(ProcessReceiveQueueAsync, CancellationTokenSource.Token);
            _ = Task.Run(SendLoopAsync, CancellationTokenSource.Token);
            _ = Task.Run(HeartbeatLoopAsync, CancellationTokenSource.Token);
            _ = Task.Run(WatchdogAsync, CancellationTokenSource.Token);
        }

        private async Task HeartbeatLoopAsync()
        {
            while (Connected && !CancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(5000, CancellationTokenSource.Token);
                Send(new HeartbeatFrame { SentAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
            }
        }

        public void RaisePingUpdated(int ping)
        {
            LastPong = DateTime.UtcNow;
            OnPingUpdated?.Invoke(ping);
        }

        private async Task WatchdogAsync()
        {
            while (Connected && !CancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(15000, CancellationTokenSource.Token);

                if ((DateTime.UtcNow - LastPong).TotalSeconds > 15)
                {
                    Disconnect();
                    break;
                }
            }
        }


        private async Task ReceiveLoopAsync()
        {
            try
            {
                while (Connected && !CancellationTokenSource.IsCancellationRequested)
                {
                    var line = await Reader.ReadLineAsync();
                    if (line == null) break;

                    _logger.Log($"Received {line}");

                    var frame = FrameSerializer.Deserialize(line);
                    if (frame != null)
                    {
                        ReceiveQueue.Enqueue(frame);
                        ReceiveSignal.Release();
                    }
                }
            }
            catch (IOException ex)
            {
                _logger.Log($"Client error on receiving loop {ex.Message}");
            }
            catch (OperationCanceledException) { }
            finally
            {
                Disconnect();
            }
        }

        private async Task ProcessReceiveQueueAsync()
        {
            while (Connected && !CancellationTokenSource.IsCancellationRequested)
            {
                await ReceiveSignal.WaitAsync(CancellationTokenSource.Token);

                if (ReceiveQueue.TryDequeue(out var frame))
                {
                    try
                    {
                        ClientFrameDispatcher.Dispatch(this, frame);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Client frame dispatch error {ex.Message}");
                    }
                }
            }
        }

        public void Send(INetworkFrame frame)
        {
            if (frame == null!) return;

            if (!Connected)
            {
                _logger.Log($"Frame ({frame.Type}) can't be sent because there is no connection with the server");
                return;
            }

            SendQueue.Enqueue(frame);
            SendSignal.Release();
        }

        private async Task SendLoopAsync()
        {
            while (Connected && !CancellationTokenSource.IsCancellationRequested)
            {
                await SendSignal.WaitAsync(CancellationTokenSource.Token);

                if (SendQueue.TryDequeue(out var frame))
                {
                    var json = FrameSerializer.Serialize(frame);
                    _logger.Log($"Frame sent {frame.Type}");
                    await Writer.WriteLineAsync(json);
                }
            }
        }

        public void Disconnect(bool reconnect = true)
        {
            if (!Connected) return;

            Connected = false;
            CancellationTokenSource.Cancel();

            try { Client?.Close(); } catch { }

            OnDisconnected?.Invoke(reconnect);

            _logger.Log("Client got disconnected");
        }
    }
}
