using Brumak_Client.Forms;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames;
using System.Net.NetworkInformation;

namespace Brumak_Client.Network.Frames
{
    public class HeartbeatFrameHandler : IFrameHandler<HeartbeatFrame>
    {
        public void Handle(object context, HeartbeatFrame frame)
        {
            if (frame.SentAt == null)
                throw Exceptions.New("HeartbeatFrame -> SentAt can't be null");

            int pingMs = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - frame.SentAt);
            NetworkManager.AuthClientManager.RaisePingUpdated(pingMs);
        }
    }
}
