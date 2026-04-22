using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Network
{
    public static class FrameType
    {
        public const string Heartbeat = "Heartbeat";
        public const string Account = "Account";
        public const string Servers = "Servers";
        public const string ServerStatus = "ServerStatus";
        public const string Characters = "Characters";
    }
}
