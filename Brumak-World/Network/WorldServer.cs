using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_World.Network
{
    public class WorldServer
    {
        public static WorldTcpServerProvider TcpWorldServer { get; private set; } = new();

        public static void Start()
        {
            TcpWorldServer.Setup();
        }
    }
}
