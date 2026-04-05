using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic.Cache
{
    public class MemoryCache<T> : ICache<T>
    {
        private readonly ConcurrentDictionary<int, T> _cache = new();

        public T? Get(int id)
            => _cache.TryGetValue(id, out var entity) ? entity : default;

        public IEnumerable<T> GetAll()
            => _cache.Values;

        public void Set(int id, T entity)
            => _cache[id] = entity;

        public void Remove(int id)
            => _cache.TryRemove(id, out _);

        public void Clear()
            => _cache.Clear();

        public bool Contains(int id)
            => _cache.ContainsKey(id);
    }
}
