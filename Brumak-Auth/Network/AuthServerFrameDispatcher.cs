using Brumak_Auth.Network.Frames;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames;
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
        private static readonly Dictionary<string, IFrameHandler<INetworkFrame>> handlers = [];

        static AuthServerFrameDispatcher()
        {
            Register<HeartbeatFrame>(new HeartbeatFrameHandler());
        }

        public static void Register<T>(IFrameHandler<T> handler) where T : INetworkFrame
        {
            handlers[Activator.CreateInstance<T>().Type] = new Wrapper<T>(handler);
        }

        public static void Dispatch(AuthClientSession session, INetworkFrame frame)
        {
            if (handlers.TryGetValue(frame.Type, out var handler))
                handler.Handle(session, frame);
            else
                _logger.Log($"Handler not found for type {frame.Type}");
        }

        private class Wrapper<T>(IFrameHandler<T> inner) : IFrameHandler<INetworkFrame> where T : INetworkFrame
        {
            public void Handle(object context, INetworkFrame frame)
                => inner.Handle(context, (T)frame);
        }
    }
}
