using Microsoft.EntityFrameworkCore;

namespace Brumak_ORM.Database
{
    public class AuthDbContext : AppDbContext, IDbContext
    {
        public AuthDbContext() { }
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
