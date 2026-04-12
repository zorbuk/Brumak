using Brumak_Client.Forms;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Servers;

namespace Brumak_Client.Network.Frames.Servers
{
    public class ServersFrameHandler : IFrameHandler<ServersFrame>
    {
        public void Handle(object context, ServersFrame frame)
        {
            ServerSelection.Instance?.RaiseServersFrameMessage(frame);
        }
    }
}
