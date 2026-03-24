using Microsoft.EntityFrameworkCore;

namespace Brumak_ORM.Database
{
    public abstract class AppDbContext : DbContext
    {
        protected AppDbContext() { }
        protected AppDbContext(DbContextOptions options) : base(options) { }
    }
}
