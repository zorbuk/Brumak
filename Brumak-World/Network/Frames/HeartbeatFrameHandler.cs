using Brumak_ORM;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Servers;
using System.Net.NetworkInformation;

namespace Brumak_World.Network.Frames
{
    public class HeartbeatFrameHandler : IFrameHandler<HeartbeatFrame>
    {
        public async Task Handle(object context, HeartbeatFrame frame)
        {
            var session = (WorldClientSession)context;
            session.Send(new HeartbeatFrame() { SentAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
        }
    }
}
