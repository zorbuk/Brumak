using Brumak_Shared.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Brumak_ORM
{
    public class Services
    {
        private static readonly Logger _logger = new("ORM", typeof(Services), showLogs: true, saveLogs: false);
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("db_settings.json")
                .Build();

        public static void BuildServiceProvider(params Type[] dbContextTypes)
        {
            var services = new ServiceCollection();

            foreach (var contextType in dbContextTypes)
            {
                var csKey = contextType.Name.Replace("DbContext", "Db");
                var connectionString = Configuration.GetConnectionString(csKey)
                                   ?? Configuration.GetConnectionString("BrumakDb")
                                   ?? throw Exceptions.New($"No connection string found for {contextType.Name}");

                var serverVersion = ServerVersion.AutoDetect(connectionString);

                var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                   .GetMethods()
                   .First(m => m.Name == nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext) &&
                               m.GetParameters().Length == 4 &&
                               m.GetParameters()[2].ParameterType == typeof(ServiceLifetime))
                   .MakeGenericMethod(contextType);

                addDbContextMethod.Invoke(null,
                [
                    services,
                (Action<DbContextOptionsBuilder>)(options => options.UseMySql(connectionString, serverVersion)),
                ServiceLifetime.Scoped,
                ServiceLifetime.Scoped
                ]);
            }

            ServiceProvider = services.BuildServiceProvider();

            foreach (var contextType in dbContextTypes)
            {
                var initializeMethod = typeof(Services)
                    .GetMethod(nameof(InitializeDatabase), BindingFlags.Static | BindingFlags.NonPublic)!
                    .MakeGenericMethod(contextType);

                initializeMethod.Invoke(null, null);
            }
        }

        private static void InitializeDatabase<T>() where T : DbContext
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            context.Database.Migrate();

            var tables = context.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .ToList();

            if (tables.Count > 0)
                _logger.Log($"[{typeof(T).Name}] Tables: {string.Join(", ", tables)}");
        }
    }
}
