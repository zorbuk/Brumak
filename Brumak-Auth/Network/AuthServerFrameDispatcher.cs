using Brumak_Auth.Network.Frames;
using Brumak_Auth.Network.Frames.Account;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Auth.Network
{
    public static class AuthServerFrameDispatcher
    {
        private static readonly Logger _logger = new("Auth", typeof(AuthServerFrameDispatcher), Program.ShowLogs, Program.SaveLogs);
        public static readonly Dictionary<Type, IFrameHandler<INetworkFrame>> handlers = [];

        static AuthServerFrameDispatcher()
        {
            Register<HeartbeatFrame>(new HeartbeatFrameHandler());
            Register<AccountFrame>(new AccountFrameHandler());
        }

        public static void Register<T>(IFrameHandler<T> handler) where T : INetworkFrame
        {
            handlers[typeof(T)] = new Wrapper<T>(handler);
        }


        public static void Dispatch(AuthClientSession client, INetworkFrame frame)
        {
            var type = frame.GetType();

            while (type != null)
            {
                if (handlers.TryGetValue(type, out var handler))
                {
                    handler.Handle(client, frame);
                    return;
                }
                type = type.BaseType;
            }

            _logger.Log($"Handler not found for frame type {frame.GetType().Name}");
        }

        private class Wrapper<T>(IFrameHandler<T> inner) : IFrameHandler<INetworkFrame> where T : INetworkFrame
        {
            public void Handle(object context, INetworkFrame frame)
                => inner.Handle(context, (T)frame);
        }
    }
}
