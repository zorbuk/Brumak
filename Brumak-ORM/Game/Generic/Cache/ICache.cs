using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic.Cache
{
    public interface ICache<T>
    {
        T? Get(int id);
        IEnumerable<T> GetAll();
        void Set(int id, T entity);
        void Remove(int id);
        void Clear();
        bool Contains(int id);
    }
}
