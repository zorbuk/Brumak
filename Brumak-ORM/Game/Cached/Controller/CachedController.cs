using Brumak_ORM.Game.Generic;
using Brumak_ORM.Game.Generic.Cache;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Cached.Controller
{
    public abstract class CachedController<TEntity, TContext> : GenericController<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly ICache<TEntity> Cache;
        protected readonly DirtyTracker<TEntity> DirtyTracker = new();

        protected CachedController(TContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
            Cache = new MemoryCache<TEntity>();
        }

        protected CachedController(TContext context, IServiceProvider serviceProvider, ICache<TEntity> cache)
            : base(context, serviceProvider)
        {
            Cache = cache;
        }

        public virtual void LoadAll()
        {
            var entities = base.GetAll();
            foreach (var entity in entities)
            {
                var id = GetEntityId(entity);
                Cache.Set(id, entity);
            }
        }

        public virtual TEntity? GetCached(int id)
        {
            var cached = Cache.Get(id);
            if (cached is not null) return cached;

            var entity = base.GetById(id);
            if (entity is not null) Cache.Set(id, entity);

            return entity;
        }

        public virtual IEnumerable<TEntity> GetAllCached()
        {
            var cached = Cache.GetAll().ToList();
            if (cached.Count > 0) return cached;

            LoadAll();
            return Cache.GetAll();
        }

        public override bool Create(TEntity entity)
        {
            var result = base.Create(entity);
            if (result) Cache.Set(GetEntityId(entity), entity);
            return result;
        }

        public override bool Update(TEntity entity)
        {
            var id = GetEntityId(entity);
            Cache.Set(id, entity);
            DirtyTracker.MarkDirty(id, entity);
            return true;
        }

        public override bool Delete(int id)
        {
            var result = base.Delete(id);
            if (result) Cache.Remove(id);
            return result;
        }

        public virtual async Task FlushAsync()
        {
            if (!DirtyTracker.HasDirty) return;

            foreach (var (id, entity) in DirtyTracker.GetDirty())
            {
                var success = base.Update(entity);
                if (success) DirtyTracker.MarkClean(id);
            }

            await Task.CompletedTask;
        }

        protected abstract int GetEntityId(TEntity entity);
    }
}
