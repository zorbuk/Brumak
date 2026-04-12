    namespace Brumak_Shared.Network.Frames.Servers
    {
        public class ServersFrame : BaseFrame
        {
            public override string Type => FrameType.Servers;
            public List<Server.Model.Server>? Servers { get; set; } = [];
        }
    }
