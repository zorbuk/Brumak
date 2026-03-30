using Brumak_ORM.Game.Account.Controller;
using Brumak_ORM.Game.Generic;
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

        #region "Controllers"
        private readonly AccountController? _accountController = Controllers.Get<Game.Account.Controller.AccountController>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _accountController?.OnModelCreating(modelBuilder.Entity<Account>());

            base.OnModelCreating(modelBuilder);
        }
    }
}
