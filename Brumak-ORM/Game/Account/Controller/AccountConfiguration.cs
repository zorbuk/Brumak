using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Account.Controller
{
    public class AccountConfiguration : IEntityTypeConfiguration<Brumak_Shared.Account.Model.Account>
    {
        public void Configure(EntityTypeBuilder<Brumak_Shared.Account.Model.Account> account)
        {
            account.HasKey(x => x.Id);
            account.ToTable("Accounts");

            account.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("varchar(32)");

            account.Property(x => x.Nickname)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("varchar(32)");

            account.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(191)
                .HasColumnType("varchar(191)");

            account.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnType("varchar(254)");

            account.Property(x => x.RegisteredIp)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnType("varchar(45)");

            account.Property(x => x.LastIp)
                .IsRequired(false)
                .HasMaxLength(45)
                .HasColumnType("varchar(45)");

            account.Property(x => x.PremiumExpirationDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            account.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            account.HasIndex(x => x.Username)
                .IsUnique();

            account.HasIndex(x => x.Nickname)
                .IsUnique();

            account.HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}
