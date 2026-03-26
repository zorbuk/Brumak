using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Network.Frames
{
    public class HeartbeatFrame : BaseFrame
    {
        public override string Type => FrameType.Heartbeat;
        public long? SentAt { get; set; } = null!;
    }
}
