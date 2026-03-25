using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Network.Frames
{
    public interface IFrameHandler<T> where T : INetworkFrame
    {
        void Handle(object context, T frame);
    }
}
