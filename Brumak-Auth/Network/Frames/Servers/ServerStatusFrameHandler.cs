using Brumak_ORM;
using Brumak_ORM.Game.Server.Controller;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Servers;
using Brumak_Shared.Server.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Auth.Network.Frames.Servers
{
    public class ServerStatusFrameHandler : IFrameHandler<ServerStatusFrame>
    {
        public async Task Handle(object context, ServerStatusFrame frame)
        {
            var session = (AuthClientSession)context;
            var serverController = Controllers.GetServerController
                ?? throw Exceptions.New("Fatal error Controllers doesn't have ServerController.");

            if (frame.Secret != Program.Secret)
            {
                await session.Disconnect("Invalid secret");
                return;
            }

            var server = serverController.GetAllCached()
                .FirstOrDefault(s => s.Id == frame.ServerId);

            if (server == null) return;

            server.Status = (ServerStatus)frame.Status;
            server.UpdatedAt = DateTime.UtcNow;
            serverController.Save();
            await serverController.FlushAsync();

            BroadcastServers();
        }

        public static void BroadcastServers()
        {
            var servers = Controllers.GetServerController!
                .GetAllCached().ToList();

            var broadcastFrame = new ServersFrame { Servers = servers };

            foreach (var client in AuthTcpServerProvider.GetClients())
                client.Send(broadcastFrame);
        }
    }
}
