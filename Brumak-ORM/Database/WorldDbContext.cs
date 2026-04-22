using Brumak_ORM.Game.Character.Controller;
using Microsoft.EntityFrameworkCore;

namespace Brumak_ORM.Database
{
    public class WorldDbContext : AppDbContext, IDbContext
    {
        public WorldDbContext() { }
        public WorldDbContext(DbContextOptions<WorldDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CharacterConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterExperiencesConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterWorldPositionConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
