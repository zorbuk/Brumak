using Brumak_ORM;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Servers;

namespace Brumak_Auth.Network.Frames
{
    public class HeartbeatFrameHandler : IFrameHandler<HeartbeatFrame>
    {
        public async Task Handle(object context, HeartbeatFrame frame)
        {
            var session = (AuthClientSession)context;
            session.Send(new HeartbeatFrame() { SentAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });

            var serverController = Controllers.GetServerController
                ?? throw Exceptions.New("Fatal error Controllers doesn't have ServerController.");

            var servers = serverController.GetAllCached().ToList();

            var hash = string.Join("|", servers.Select(s => $"{s.UpdatedAt.Ticks}"));

            if (session.LastServersHash == hash) return;

            session.LastServersHash = hash;
            session.Send(new ServersFrame() { Servers = servers });
        }
    }
}
