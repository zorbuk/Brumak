using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic.Cache
{
    public class DirtyTracker<T>
    {
        private readonly ConcurrentDictionary<int, T> _dirty = new();

        public void MarkDirty(int id, T entity) => _dirty[id] = entity;
        public void MarkClean(int id) => _dirty.TryRemove(id, out _);
        public IEnumerable<KeyValuePair<int, T>> GetDirty() => _dirty.ToArray();
        public bool HasDirty => !_dirty.IsEmpty;
    }
}
