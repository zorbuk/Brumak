using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Brumak_ORM.Database
{
    public static class DbContextExecutor
    {
        public static void Execute<TContext>(Action<TContext> action) where TContext : DbContext
        {
            using var scope = Services.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            action(context);
        }
        public static async Task ExecuteAsync<TContext>(Func<TContext, Task> action) where TContext : DbContext
        {
            using var scope = Services.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await action(context);
        }
        public static TResult Execute<TResult, TContext>(Func<TContext, TResult> func) where TContext : DbContext
        {
            using var scope = Services.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            return func(context);
        }
        public static async Task<TResult> ExecuteAsync<TResult, TContext>(Func<TContext, Task<TResult>> func) where TContext : DbContext
        {
            using var scope = Services.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            return await func(context);
        }
    }
}
