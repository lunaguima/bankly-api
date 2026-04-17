using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bankly.Domain.Entities;

namespace Bankly.Infrastructure.Persistence.Configurations;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.HasKey(at => at.Id);

        builder.Property(at => at.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(at => at.Accounts)
            .WithOne(a => a.AccountType)
            .HasForeignKey(a => a.AccountTypeId);
    }
}