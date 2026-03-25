using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Auth.Network
{
    public class AuthServer
    {
        public static AuthTcpServerProvider TcpAuthServer { get; private set; } = new();

        public static void Start()
        {
            TcpAuthServer.Setup();
        }
    }
}
