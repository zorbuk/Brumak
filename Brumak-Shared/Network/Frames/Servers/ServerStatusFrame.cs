using Brumak_Shared.Server.Enum;

namespace Brumak_Shared.Network.Frames.Servers
{
    public class ServerStatusFrame : BaseFrame
    {
        public override string Type => FrameType.ServerStatus;
        public int ServerId { get; set; }
        public ServerStatus Status { get; set; }
        public string Secret { get; set; } = "";
    }
}
