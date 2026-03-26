using Brumak_ORM;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace Brumak_Auth.Network
{
    public class AuthClientSession(TcpClient client, AuthTcpServerProvider server)
    {
        private Logger _logger = new("Auth", typeof(AuthClientSession), Program.ShowLogs, Program.SaveLogs);

        public TcpClient Client { get; private set; } = client;
        public AuthTcpServerProvider Server { get; private set; } = server;

        public string? Username { get; set; }

        private Stream Stream = null!;
        private StreamReader Reader = null!;
        private StreamWriter Writer = null!;
        private bool Connected;

        private readonly ConcurrentQueue<INetworkFrame> SendQueue = new();
        private readonly SemaphoreSlim SendSignal = new(0);
        private readonly ConcurrentQueue<INetworkFrame> ReceiveQueue = new();
        private readonly SemaphoreSlim ReceiveSignal = new(0);

        public void Initialize()
        {
            //SslStream ssl = new(
            //    Client.GetStream(),
            //    false
            //);

            //ssl.AuthenticateAsServer(
            //    CertificateManager.GetCertificate(),
            //    clientCertificateRequired: false,
            //    SslProtocols.Tls13,
            //    checkCertificateRevocation: true
            //);

            Stream = Client.GetStream();//ssl;
            Reader = new StreamReader(Stream, Encoding.UTF8);
            Writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
            Connected = true;

            _ = Task.Run(ReceiveLoopAsync);
            _ = Task.Run(ProcessReceiveQueueAsync);
            _ = Task.Run(SendLoopAsync);
        }

        private async Task ReceiveLoopAsync()
        {
            try
            {
                while (Connected)
                {
                    var line = await Reader.ReadLineAsync();
                    if (line == null) break;

                    _logger.Log($"Recieved to server {line}");

                    var frame = FrameSerializer.Deserialize(line);
                    if (frame != null)
                    {
                        ReceiveQueue.Enqueue(frame);
                        ReceiveSignal.Release();
                    }
                }
            }
            catch (IOException) { }
            finally { Disconnect(); }
        }

        private async Task ProcessReceiveQueueAsync()
        {
            while (Connected)
            {
                await ReceiveSignal.WaitAsync();

                if (ReceiveQueue.TryDequeue(out var frame))
                {
                    AuthServerFrameDispatcher.Dispatch(this, frame);
                }
            }
        }

        public void Send(INetworkFrame frame)
        {
            if (!Connected) return;

            SendQueue.Enqueue(frame);
            SendSignal.Release();
        }

        private async Task SendLoopAsync()
        {
            while (Connected)
            {
                await SendSignal.WaitAsync();

                if (SendQueue.TryDequeue(out var frame))
                {
                    var json = FrameSerializer.Serialize(frame);

                    _logger.Log($"Sent from server {json}");

                    await Writer.WriteLineAsync(json);
                }
            }
        }

        public void Disconnect(string reason = "Connection finished")
        {
            if (!Connected) return;

            Connected = false;

            this.Client?.Close();
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();

            AuthTcpServerProvider.RemoveClient(this);
        }
    }
}
