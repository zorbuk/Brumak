using Brumak_ORM;
using Brumak_Shared.Metrics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_World.Network
{
    public class WorldTcpServerProvider
    {
        private static readonly Logger _logger = new("World", typeof(WorldTcpServerProvider), Program.ShowLogs, Program.SaveLogs);
        public TcpListener Listener { get; private set; } = null!;
        public bool Running { get; private set; } = false;

        public static readonly ConcurrentDictionary<int, WorldClientSession> activeAccounts = new();
        private static readonly ConcurrentDictionary<string, WorldClientSession> clients = [];

        public static IEnumerable<WorldClientSession> GetClients()
        {
            return clients.Values;
        }

        public static void AddClient(WorldClientSession session)
        {
            if (!string.IsNullOrEmpty(session.Username))
            {
                clients.TryAdd(session.Username, session);
            }
        }

        public static void RemoveClient(WorldClientSession session)
        {
            if (!string.IsNullOrEmpty(session.Username))
            {
                clients.TryRemove(session.Username, out _);
                _logger.Log($"Client session removed: {session.Username}");
            }
        }

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _loginLocks = new();

        public static SemaphoreSlim GetLoginLock(string username)
            => _loginLocks.GetOrAdd(username.ToLower(), _ => new SemaphoreSlim(1, 1));

        public static void CleanLoginLock(string username)
            => _loginLocks.TryRemove(username.ToLower(), out _);

        public Action<Exception> OnError { get; private set; } = ex =>
        {
            _logger.Log("Error on accepting connection " + ex.Message);
        };
        public Action<WorldClientSession> OnCreateClientSession { get; private set; } = null!;
        public Action<TcpClient, WorldTcpServerProvider> OnConnection { get; private set; } = null!;

        private readonly string WorldIp = Services.Configuration.GetConnectionString("WorldServerIp")
            ?? throw Exceptions.New("'WorldServerIp' is not correctly defined on ConnectionStrings.");

        private readonly int WorldPort = int.Parse(Services.Configuration.GetConnectionString("WorldServerPort")
                ?? throw Exceptions.New("'WorldServerPort' is not correctly defined on ConnectionStrings."));

        public async void Setup()
        {
            this.OnCreateClientSession += CreateClientSession;
            this.OnConnection += Connection;

            this.Listener = new TcpListener(System.Net.IPAddress.Parse(WorldIp), WorldPort);
            this.Listener.Start();
            this.Running = true;

            _logger.Log($"WorldServer started {WorldIp}:{WorldPort}");

            while (this.Running)
            {
                try
                {
                    OnConnection.Invoke(await Listener.AcceptTcpClientAsync(), this);
                }
                catch (Exception ex)
                {
                    OnError.Invoke(ex);
                }
            }
        }

        private void Connection(TcpClient client, WorldTcpServerProvider provider)
        {
            OnCreateClientSession.Invoke(new WorldClientSession(client, provider));
        }

        private void CreateClientSession(WorldClientSession session)
        {
            _logger.Log("New client connected from " + session.Client.Client.RemoteEndPoint);
            session.Initialize();
        }
    }
}
