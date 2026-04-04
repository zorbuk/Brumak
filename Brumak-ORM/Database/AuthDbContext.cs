using Brumak_ORM.Game.Account.Controller;
using Brumak_Shared.Account.Model;
using Microsoft.EntityFrameworkCore;

namespace Brumak_ORM.Database
{
    public class AuthDbContext : AppDbContext, IDbContext
    {
        public AuthDbContext() { }
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        #region "DbSet Properties"
        public DbSet<Account> Accounts { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
