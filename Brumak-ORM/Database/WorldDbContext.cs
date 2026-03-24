using Microsoft.EntityFrameworkCore;

namespace Brumak_ORM.Database
{
    public class WorldDbContext : AppDbContext, IDbContext
    {
        public WorldDbContext() { }
        public WorldDbContext(DbContextOptions<WorldDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
