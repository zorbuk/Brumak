using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Metrics
{
    public static class Ip
    {
        public static string? Get()
        {
            using Socket socket =
                new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint?.Address.ToString();
        }
    }
}
