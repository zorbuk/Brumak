using Brumak_Client.Forms;
using Brumak_Client.Network.Frames;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Client.Network
{
    public static class ClientFrameDispatcher
    {
        private static readonly Logger _logger = new("Client", typeof(ClientFrameDispatcher), App.ShowLogs, App.SaveLogs);
        public static readonly Dictionary<string, IFrameHandler<INetworkFrame>> Handlers = [];

        private class Wrapper<T>(IFrameHandler<T> inner) : IFrameHandler<INetworkFrame> where T : INetworkFrame
        {
            private readonly IFrameHandler<T> Inner = inner;
            public void Handle(object context, INetworkFrame frame)
                => Inner.Handle(context, (T)frame);
        }

        public static void Register<T>(IFrameHandler<T> handler) where T : INetworkFrame
        {
            Handlers[Activator.CreateInstance<T>().Type] = new Wrapper<T>(handler);
        }

        public static void Dispatch(TcpClientProvider client, INetworkFrame frame)
        {
            if (Handlers.TryGetValue(frame.Type, out var handler))
                handler.Handle(client, frame);
            else
                _logger.Log($"Handler not found for frame type {frame.Type}");
        }

        public static void Initialize()
        {
            Register<HeartbeatFrame>(new HeartbeatFrameHandler());
        }
    }
}
