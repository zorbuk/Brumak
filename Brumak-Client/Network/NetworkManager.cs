using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Client.Network
{
    public static class NetworkManager
    {
        public static TcpClientProvider AuthClientManager { get; private set; } = null!;
        public static TcpClientProvider WorldClientManager { get; private set; } = null!;

        public static event Action<int>? PingUpdated;
        public static event Action<bool>? AuthStatusChanged;
        public static event Action<bool>? WorldStatusChanged;

        public static void SetAuth(TcpClientProvider provider)
        {
            AuthClientManager?.Disconnect();
            AuthClientManager = provider;
        }

        public static async Task TransitionToWorld(string worldIp, int worldPort)
        {
            var worldClient = new TcpClientProvider();
            await worldClient.ConnectAsync(worldIp, worldPort);

            AuthClientManager?.Disconnect(false);
            AuthClientManager = null!;

            WorldClientManager = worldClient;
        }

        public static void DisconnectWorld()
        {
            WorldClientManager?.Disconnect();
            WorldClientManager = null!;
        }
    }
}
