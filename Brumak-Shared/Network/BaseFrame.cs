using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Network
{
    public abstract class BaseFrame : INetworkFrame
    {
        public abstract string Type { get; }
    }
}
