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

namespace Brumak_Auth.Network
{
    public class AuthTcpServerProvider
    {
        private static readonly Logger _logger = new("Auth", typeof(AuthTcpServerProvider));
        public TcpListener Listener { get; private set; } = null!;
        public bool Running { get; private set; } = false;

        public static readonly ConcurrentDictionary<int, AuthClientSession> activeAccounts = new();
        private static readonly ConcurrentDictionary<string, AuthClientSession> clients = [];

        public static IEnumerable<AuthClientSession> GetClients()
        {
            return clients.Values;
        }

        public static void AddClient(AuthClientSession session)
        {
            if (!string.IsNullOrEmpty(session.Username))
            {
                clients.TryAdd(session.Username, session);
            }
        }

        public static void RemoveClient(AuthClientSession session)
        {
            if (!string.IsNullOrEmpty(session.Username))
            {
                clients.TryRemove(session.Username, out _);
                _logger.Log($"Client session removed: {session.Username}");
            }
        }

        public Action<Exception> OnError { get; private set; } = ex =>
        {
            _logger.Log("Error on accepting connection " + ex.Message);
        };
        public Action<AuthClientSession> OnCreateClientSession { get; private set; } = null!;
        public Action<TcpClient, AuthTcpServerProvider> OnConnection { get; private set; } = null!;

        private readonly string AuthIp = Services.Configuration.GetConnectionString("AuthServerIp")
            ?? throw Exceptions.New("'AuthServerIp' is not correctly defined on ConnectionStrings.");

        private readonly int AuthPort = int.Parse(Services.Configuration.GetConnectionString("AuthServerPort")
                ?? throw Exceptions.New("'AuthServerPort' is not correctly defined on ConnectionStrings."));

        public async void Setup()
        {
            this.OnCreateClientSession += CreateClientSession;
            this.OnConnection += Connection;

            this.Listener = new TcpListener(System.Net.IPAddress.Parse(AuthIp), AuthPort);
            this.Listener.Start();
            this.Running = true;

            _logger.Log($"AuthServer started {AuthIp}:{AuthPort}");

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

        private void Connection(TcpClient client, AuthTcpServerProvider provider)
        {
            OnCreateClientSession.Invoke(new AuthClientSession(client, provider));
        }

        private void CreateClientSession(AuthClientSession session)
        {
            _logger.Log("New client connected from " + session.Client.Client.RemoteEndPoint);
            session.Initialize();
        }
    }
}
