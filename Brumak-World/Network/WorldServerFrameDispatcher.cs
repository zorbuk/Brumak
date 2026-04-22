using Brumak_Shared.Metrics;
using Brumak_Shared.Network;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Account;
using Brumak_Shared.Network.Frames.Characters;
using Brumak_World.Network.Frames;
using Brumak_World.Network.Frames.Characters;

namespace Brumak_World.Network
{
    public static class WorldServerFrameDispatcher
    {
        private static readonly Logger _logger = new("World", typeof(WorldServerFrameDispatcher), Program.ShowLogs, Program.SaveLogs);
        public static readonly Dictionary<Type, IFrameHandler<INetworkFrame>> handlers = [];

        static WorldServerFrameDispatcher()
        {
            Register<HeartbeatFrame>(new HeartbeatFrameHandler());
            Register<CharacterFrame>(new CharacterFrameHandler());
        }

        public static void Register<T>(IFrameHandler<T> handler) where T : INetworkFrame
        {
            handlers[typeof(T)] = new Wrapper<T>(handler);
        }


        public static void Dispatch(WorldClientSession client, INetworkFrame frame)
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
            public async Task Handle(object context, INetworkFrame frame)
                => await inner.Handle(context, (T)frame);
        }
    }
}
