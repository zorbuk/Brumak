using Brumak_Client.Forms;
using Brumak_Client.Network.Frames;
using Brumak_Client.Network.Frames.Account;
using Brumak_Client.Network.Frames.Servers;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Account;
using Brumak_Shared.Network.Frames.Servers;
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
        public static readonly Dictionary<Type, IFrameHandler<INetworkFrame>> Handlers = [];

        private class Wrapper<T>(IFrameHandler<T> inner) : IFrameHandler<INetworkFrame> where T : INetworkFrame
        {
            private readonly IFrameHandler<T> Inner = inner;
            public async Task Handle(object context, INetworkFrame frame)
                => await Inner.Handle(context, (T)frame);
        }

        public static void Register<T>(IFrameHandler<T> handler) where T : INetworkFrame
        {
            Handlers[typeof(T)] = new Wrapper<T>(handler);
        }

        public static void Dispatch(TcpClientProvider client, INetworkFrame frame)
        {
            var type = frame.GetType();

            while (type != null)
            {
                if (Handlers.TryGetValue(type, out var handler))
                {
                    handler.Handle(client, frame);
                    return;
                }
                type = type.BaseType;
            }

            _logger.Log($"Handler not found for frame type {frame.GetType().Name}");
        }

        public static void Initialize()
        {
            Register<HeartbeatFrame>(new HeartbeatFrameHandler());
            Register<AccountFrame>(new AccountFrameHandler());
            Register<ServersFrame>(new ServersFrameHandler());
        }
    }
}
