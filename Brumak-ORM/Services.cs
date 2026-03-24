using Brumak_ORM.Database;
using Brumak_Shared.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brumak_ORM
{
    public class Services
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public static void BuildServiceProvider(Type dbContext)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("db_settings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("BrumakDb")
                ?? throw Exceptions.New("Connection string 'BrumakDb' not found in configuration");

            var serverVersion = ServerVersion.AutoDetect(connectionString);

            var dbContextTypes = new[] { dbContext };

            foreach (var contextType in dbContextTypes)
            {
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
                    .GetMethod(nameof(InitializeDatabase), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
                    .MakeGenericMethod(contextType);

                initializeMethod.Invoke(null, null);
            }
        }

        private static void InitializeDatabase<T>() where T : DbContext
        {
            DbContextExecutor.Execute<T>((context) =>
            {
                context.Database.Migrate();

                var tables = context.Model.GetEntityTypes()
                    .Select(t => t.GetTableName())
                    .ToList();

                Console.WriteLine($"Db: {string.Join(", ", tables)}");
            });
        }
    }
}
