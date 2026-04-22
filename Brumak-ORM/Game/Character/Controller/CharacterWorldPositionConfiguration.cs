using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brumak_ORM.Game.Character.Controller
{
    public class CharacterWorldPositionConfiguration : IEntityTypeConfiguration<Brumak_Shared.Character.Model.CharacterWorldPosition>
    {
        public void Configure(EntityTypeBuilder<Brumak_Shared.Character.Model.CharacterWorldPosition> position)
        {
            position.HasKey(x => x.CharacterId);
            position.ToTable("CharacterWorldPositions");

            position.Property(x => x.CharacterId)
                .IsRequired()
                .HasColumnType("int");

            position.Property(x => x.MapId)
                .IsRequired()
                .HasColumnType("int");

            position.Property(x => x.PositionX)
                .IsRequired()
                .HasColumnType("int");

            position.Property(x => x.PositionY)
                .IsRequired()
                .HasColumnType("int");

            position.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        }
    }
}