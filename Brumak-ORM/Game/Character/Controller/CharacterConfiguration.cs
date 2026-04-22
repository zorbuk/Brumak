using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brumak_ORM.Game.Character.Controller
{
    public class CharacterConfiguration : IEntityTypeConfiguration<Brumak_Shared.Character.Model.Character>
    {
        public void Configure(EntityTypeBuilder<Brumak_Shared.Character.Model.Character> character)
        {
            character.HasKey(x => x.Id);
            character.ToTable("Characters");

            character.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("varchar(32)");

            character.Property(x => x.ServerId)
                .IsRequired()
                .HasColumnType("int");

            character.Property(x => x.AccountId)
                .IsRequired()
                .HasColumnType("int");

            character.Property(x => x.Skin)
                .IsRequired()
                .HasColumnType("int");

            character.Property(x => x.Class)
                .IsRequired()
                .HasColumnType("int");

            character.Property(x => x.SkinHexColors)
                .IsRequired()
                .HasColumnType("json");

            character.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            character.HasIndex(x => new { x.ServerId, x.Name })
                .IsUnique();

            character.HasOne<Brumak_Shared.Character.Model.CharacterExperiences>()
                .WithOne()
                .HasForeignKey<Brumak_Shared.Character.Model.CharacterExperiences>(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            character.HasOne<Brumak_Shared.Character.Model.CharacterWorldPosition>()
                .WithOne()
                .HasForeignKey<Brumak_Shared.Character.Model.CharacterWorldPosition>(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
