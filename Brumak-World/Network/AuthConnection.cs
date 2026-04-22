using Brumak_ORM;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames.Servers;
using Brumak_Shared.Server.Enum;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Text;

namespace Brumak_World.Network
{
    public static class AuthConnection
    {
        private static readonly Logger _logger = new("World", typeof(AuthConnection),
            Program.ShowLogs, Program.SaveLogs);

        private static TcpClient? _client;
        private static StreamWriter? _writer;
        private static bool _connected;

        private static readonly string AuthIp = Services.Configuration
            .GetConnectionString("AuthServerIp")
            ?? throw Exceptions.New("'AuthServerIp' is not correctly defined.");

        private static readonly int AuthPort = int.Parse(Services.Configuration
            .GetConnectionString("AuthServerPort")
            ?? throw Exceptions.New("'AuthServerPort' is not correctly defined."));

        private static readonly int WorldServerId = int.Parse(Services.Configuration
            .GetConnectionString("WorldServerId")
            ?? throw Exceptions.New("'WorldServerId' is not correctly defined."));

        private static readonly string Secret = Services.Configuration
            .GetConnectionString("Secret")
            ?? throw Exceptions.New("'Secret' is not correctly defined.");

        public static async Task ConnectAsync()
        {
            while (true)
            {
                try
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(AuthIp, AuthPort);

                    var stream = _client.GetStream();
                    _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    _connected = true;

                    _logger.Log($"Connected to AuthServer {AuthIp}:{AuthPort}");

                    SendStatus(ServerStatus.Online);

                    _ = Task.Run(ReceiveLoopAsync);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not connect to AuthServer: {ex.Message}. Retrying in 3s...");
                    await Task.Delay(3000);
                }
            }
        }

        public static void SendStatus(ServerStatus status)
        {
            if (!_connected || _writer == null) return;

            try
            {
                var frame = new ServerStatusFrame
                {
                    ServerId = WorldServerId,
                    Status = status,
                    Secret = Secret
                };

                var json = FrameSerializer.Serialize(frame);
                _writer.WriteLine(json);

                _logger.Log($"Status sent to AuthServer: {status}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error sending status: {ex.Message}");
            }
        }

        private static async Task ReceiveLoopAsync()
        {
            try
            {
                var reader = new StreamReader(_client!.GetStream(), Encoding.UTF8);
                while (_connected)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;
                }
            }
            catch (IOException) { }
            finally
            {
                _connected = false;
                _logger.Log("Disconnected from AuthServer. Reconnecting...");
                await Task.Delay(3000);
                await ConnectAsync();
            }
        }

        public static void Disconnect()
        {
            SendStatus(ServerStatus.Offline);
            _connected = false;
            _writer?.Close();
            _client?.Close();
        }
    }
}