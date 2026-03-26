using Brumak_Shared.Network.Frames;

namespace Brumak_Auth.Network.Frames
{
    public class HeartbeatFrameHandler : IFrameHandler<HeartbeatFrame>
    {
        public async void Handle(object context, HeartbeatFrame frame)
        {
            var session = (AuthClientSession)context;
            session.Send(new HeartbeatFrame() { SentAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
        }
    }
}
