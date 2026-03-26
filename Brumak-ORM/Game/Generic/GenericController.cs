using Brumak_Shared.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic
{
    public abstract class GenericController<TEntity, TContext>(IServiceProvider serviceProvider)
    : IGenericController<TEntity>
    where TEntity : class
    where TContext : DbContext
    {
        private readonly Logger _logger = new("ORM", typeof(GenericController<TEntity, TContext>), showLogs: false, saveLogs: true);
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public virtual bool Create(TEntity entity)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error creating {typeof(TEntity).Name}: {ex.Message}");
                return false;
            }
        }

        public virtual TEntity? GetById(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                return context.Set<TEntity>().Find(id);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error getting {typeof(TEntity).Name} by Id {id}: {ex.Message}");
                return null;
            }
        }

        public virtual List<TEntity> GetAll()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                return [.. context.Set<TEntity>()];
            }
            catch (Exception ex)
            {
                _logger.Log($"Error getting all {typeof(TEntity).Name}: {ex.Message}");
                return [];
            }
        }

        public virtual void Save()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Log($"Error saving changes: {ex.Message}");
            }
        }

        public virtual bool Update(TEntity entity)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                if (entity == null)
                {
                    _logger.Log($"Error updating {typeof(TEntity).Name}: entity is null");
                    return false;
                }

                context.Set<TEntity>().Update(entity);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error updating {typeof(TEntity).Name}: {ex.Message}");
                return false;
            }
        }

        public virtual bool Delete(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                var entity = context.Set<TEntity>().Find(id);
                if (entity == null)
                {
                    _logger.Log($"Entity {typeof(TEntity).Name} with Id {id} not found for deletion");
                    return false;
                }

                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error deleting {typeof(TEntity).Name} with Id {id}: {ex.Message}");
                return false;
            }
        }
    }
}
