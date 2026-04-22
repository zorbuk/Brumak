using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brumak_ORM.Game.Character.Controller
{
    public class CharacterExperiencesConfiguration : IEntityTypeConfiguration<Brumak_Shared.Character.Model.CharacterExperiences>
    {
        public void Configure(EntityTypeBuilder<Brumak_Shared.Character.Model.CharacterExperiences> experiencies)
        {
            experiencies.HasKey(x => x.CharacterId);
            experiencies.ToTable("CharacterExperiences");

            experiencies.Property(x => x.CharacterId)
                .IsRequired()
                .HasColumnType("int");

            experiencies.Property(x => x.Experience)
                .IsRequired()
                .HasDefaultValue(0)
                .HasColumnType("bigint");
        }
    }
}