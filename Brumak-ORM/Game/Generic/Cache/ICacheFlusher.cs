using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic.Cache
{
    public interface ICacheFlusher
    {
        Task FlushAsync();
        string Name { get; }
    }
}
